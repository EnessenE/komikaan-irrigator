using Dapper;
using Npgsql;
using ProtoBuf;
using System.Data;
using System.Diagnostics;
using TransitRealtime;
using static Dapper.SqlMapper;

namespace komikaan.Irrigator.Services
{
    public class GTFSRealtimeRetriever : BackgroundService
    {
        private readonly ILogger<GTFSRealtimeRetriever> _logger;
        private readonly string? _connectionString;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public GTFSRealtimeRetriever(ILogger<GTFSRealtimeRetriever> logger, IConfiguration config, HttpClient httpClient)
        {
            _logger = logger;
            _connectionString = config.GetConnectionString("gtfs");
            _httpClient = httpClient;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            var interval = _config.GetValue<TimeSpan>("WorkInterval", TimeSpan.FromSeconds(10));
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Starting a process cycle");
                //await RunImportAsync("https://gtfs.ovapi.nl/nl/alerts.pb");
                await RunImportAsync("https://gtfs.ovapi.nl/nl/vehiclePositions.pb");
                _logger.LogInformation("Finished, waiting for the interval of {time}", interval);
                await Task.Delay(interval, stoppingToken);
            }

        }

        private async Task RunImportAsync(string url)
        {
            //TODO: Figure out scaling how to factor in supplierconfigurations for many feeds
            //TODO: Actually scale this in all directions
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Downloaded pb.");
                FeedMessage feed = Serializer.Deserialize<FeedMessage>(response.Content.ReadAsStream());
                _logger.LogInformation("Parsed pb.");
                _logger.LogInformation("Entities: {cnt}", feed.Entities.Count);

                var count = feed.Entities.Where(entity => entity.Alert != null).Count();
                _logger.LogInformation("Alert: {cnt}", count);
                var stop = Stopwatch.StartNew();
                foreach (FeedEntity entity in feed.Entities)
                {
                    var alert = entity.Alert;
                    if (alert != null)
                    {
                        await ProcessAlertAsync(entity.Id, alert);
                    }
                    var vehicleUpdate = entity.Vehicle;
                    if (vehicleUpdate != null)
                    {
                        await ProcessVehiclePositionAsync(entity.Id, vehicleUpdate);
                        _logger.LogInformation("{x} , {y}, - trip {trip}", entity.Vehicle?.Position?.Longitude, entity.Vehicle?.Position?.Latitude, entity.Vehicle?.Trip?.TripId);
                    }
                }
            }
            else
            {
                _logger.LogError("Failed to call target api: {reason} - {msg}", response.StatusCode, response.ReasonPhrase);
            }
        }

        private async Task ProcessAlertAsync(string entityId, Alert alert)
        {
            //Todo: batching
            _logger.LogInformation("Alert: {1}, {2}, {3}, {4}, {5}", alert.HeaderText?.Translations.FirstOrDefault()?.Language, alert.DescriptionText?.Translations.FirstOrDefault()?.Language, alert.TtsDescriptionText?.Translations.FirstOrDefault()?.Language, alert.cause, alert.effect);
            using var dbConnection = new NpgsqlConnection(_connectionString);

            await dbConnection.ExecuteAsync(
                @"CALL public.upsert_alert(@data_origin, @internal_id, @id, @last_updated, @active_periods, @cause, @effect, @url, @header_text, @description_text, @tts_header_text, @tts_description_text, @severity_level)",
                new
                {
                    data_origin = "OpenOV",
                    internal_id = Guid.NewGuid(),
                    id = entityId,
                    last_updated = DateTimeOffset.UtcNow,
                    active_periods = GetActivePeriod(alert.ActivePeriods),
                    cause = alert.cause.ToString(),
                    effect = alert.effect.ToString(),
                    url = alert.Url?.Translations?.FirstOrDefault()?.Text,
                    header_text = alert.HeaderText?.Translations?.FirstOrDefault()?.Text,
                    description_text = alert.DescriptionText?.Translations?.FirstOrDefault()?.Text,
                    tts_header_text = alert.TtsHeaderText?.Translations?.FirstOrDefault()?.Text,
                    tts_description_text = alert.TtsDescriptionText?.Translations?.FirstOrDefault()?.Text,
                    severity_level = ((Alert.SeverityLevel?)alert.severity_level)?.ToString() ?? null,
                },
                commandType: CommandType.Text
            );

            await InformEntitiesAsync(alert.InformedEntities, dbConnection);
        }

        private async Task ProcessVehiclePositionAsync(string entityId, VehiclePosition vehiclePosition)
        {
            //Todo: batching
            using var dbConnection = new NpgsqlConnection(_connectionString);
            double? latitude = null;
            double? longitude = null;

            if (vehiclePosition.Position != null)
            {
                latitude = (double)vehiclePosition.Position?.Latitude;
                longitude = (double)vehiclePosition.Position?.Longitude;
            }
            else
            {
                _logger.LogDebug("Vehicle position is empty for {id}", entityId);
            }

            await dbConnection.ExecuteAsync(
                @"CALL public.upsert_position(@data_origin, @internal_id, @last_updated, @id, @trip_id, @latitude, @longitude, @stop_id, @current_status, @measurement_time, @congestion_level, @occupancy_status, @occupancy_percentage )",
                new
                {
                    data_origin = "OpenOV",
                    internal_id = Guid.NewGuid(),
                    last_updated = DateTimeOffset.UtcNow,

                    id = entityId,

                    trip_id = vehiclePosition.Trip?.TripId,
                    latitude = latitude,
                    longitude = longitude,

                    stop_id = string.IsNullOrWhiteSpace(vehiclePosition.StopId) ? null : vehiclePosition.StopId,
                    current_status = vehiclePosition.CurrentStatus.ToString(),
                    measurement_time = GetDTCTime(vehiclePosition.Timestamp),
                    congestion_level = vehiclePosition.congestion_level.ToString(),
                    occupancy_status = vehiclePosition.occupancy_status.ToString(),
                    occupancy_percentage = 0
                },
                commandType: CommandType.Text
            );
        }
        static DateTimeOffset GetDTCTime(ulong nanoseconds, ulong ticksPerNanosecond)
        {
            var pointOfReference = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromMicroseconds(0));
            long ticks = (long)(nanoseconds / ticksPerNanosecond);
            return pointOfReference.AddTicks(ticks);
        }

        static DateTimeOffset? GetDTCTime(ulong? nanoseconds)
        {
            if (nanoseconds != null)
            {
                return GetDTCTime(nanoseconds!.Value, 100);
            }
            return null;
        }

        private async Task InformEntitiesAsync(List<EntitySelector> informedEntities, NpgsqlConnection dbConnection)
        {
            //Todo: batching
            _logger.LogInformation("entities to inform: {cnt}", informedEntities.Count);
            foreach (EntitySelector entity in informedEntities)
            {

                await dbConnection.ExecuteAsync(
                    @"CALL public.upsert_alert_entities(@data_origin, @internal_id, @last_updated, @agency_id, @route_id, @trip_id, @stop_id)",
                    new
                    {
                        data_origin = "OpenOV",
                        internal_id = Guid.NewGuid(),
                        last_updated = DateTimeOffset.UtcNow,
                        agency_id = !string.IsNullOrWhiteSpace(entity.AgencyId) ? entity.AgencyId : null,
                        route_id = !string.IsNullOrWhiteSpace(entity.RouteId) ? entity.RouteId : null,
                        trip_id = entity.Trip?.TripId,
                        stop_id = !string.IsNullOrWhiteSpace(entity.StopId) ? entity.StopId : null
                    },
                    commandType: CommandType.Text
                );
            }
        }

        private Guid GetActivePeriod(List<TimeRange> activePeriods)
        {
            return Guid.Empty;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

    }
}
