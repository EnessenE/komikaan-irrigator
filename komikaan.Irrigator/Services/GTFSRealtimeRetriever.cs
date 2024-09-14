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
        private readonly NpgsqlDataSourceBuilder _dataSourceBuilder;
        private readonly NpgsqlDataSource _dataSource;

        public GTFSRealtimeRetriever(ILogger<GTFSRealtimeRetriever> logger, IConfiguration config, HttpClient httpClient)
        {
            _logger = logger;
            _connectionString = config.GetConnectionString("gtfs");
            _httpClient = httpClient;
            _config = config;
            _dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
            _dataSourceBuilder.MapComposite<PsqlTripUpdate>("trip_update_type");
            _dataSourceBuilder.MapComposite<PsqlStopTimeUpdate>("trip_update_stop_time_type");
            _dataSourceBuilder.MapComposite<PsqlPositionUpdate>("position_entity_type");
            _dataSource = _dataSourceBuilder.Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            var interval = _config.GetValue<TimeSpan>("WorkInterval", TimeSpan.FromSeconds(30));
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Starting a process cycle");
                //await RunImportAsync("https://gtfs.ovapi.nl/nl/alerts.pb");
                await RunImportAsync("https://gtfs.ovapi.nl/nl/vehiclePositions.pb");
                await RunImportAsync("https://gtfs.ovapi.nl/nl/tripUpdates.pb");
                await RunImportAsync("https://gtfs.ovapi.nl/nl/trainUpdates.pb");
                _logger.LogInformation("Finished, waiting for the interval of {time}", interval);
                await Task.Delay(interval, stoppingToken);
            }

        }

        private async Task RunImportAsync(string url)
        {
            //TODO: Figure out scaling how to factor in supplierconfigurations for many feeds
            //TODO: Actually scale this in all directions
            //TODO: what is this mess i made
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Downloaded pb.");
                FeedMessage feed = Serializer.Deserialize<FeedMessage>(response.Content.ReadAsStream());
                _logger.LogInformation("Parsed pb.");
                _logger.LogInformation("Entities: {cnt}", feed.Entities.Count);

                var count = feed.Entities.Where(entity => entity.Alert != null).Count();
                _logger.LogInformation("Alert: {cnt}", count);
                _logger.LogInformation("VehicleUpdates: {cnt}", feed.Entities.Where(entity => entity.Vehicle != null).Count());
                var stop = Stopwatch.StartNew();
                var dbConnection = await _dataSource.OpenConnectionAsync();
                if (feed.Entities.Any(x => x.TripUpdate != null))
                {
                    await ProcessTripUpdateAsync(dbConnection, feed.Entities);
                    foreach (var batch in feed.Entities.Select(x => new Tuple<FeedEntity, List<TripUpdate.StopTimeUpdate>>(x, x.TripUpdate.StopTimeUpdates)).Chunk(500))
                    {
                        await UpdateStopTimeUpdates(batch, dbConnection);
                    }
                }
                if (feed.Entities.Any(x => x.Vehicle != null))
                {
                    await ProcessTripUpdateAsync(dbConnection, feed.Entities);
                    var vehiclePositonsToUpdate = feed.Entities.Select(x => new Tuple<FeedEntity, VehiclePosition>(x, x.Vehicle)).Chunk(500);
                    _logger.LogInformation("Total of {x} updates", vehiclePositonsToUpdate.Count());
                    foreach (var batch in vehiclePositonsToUpdate.Chunk(5))
                    {
                        await ProcessVehiclePositionAsync(batch.ToList());
                    }
                }
                //foreach (FeedEntity entity in feed.Entities)
                //{
                //    _logger.LogDebug("Processing entity {id}", entity.Id);
                //    var alert = entity.Alert;
                //    if (alert != null)
                //    {
                //        await ProcessAlertAsync(entity.Id, alert);
                //    }
                //}
            }
            else
            {
                _logger.LogError("Failed to call target api: {reason} - {msg}", response.StatusCode, response.ReasonPhrase);
            }
        }


        private async Task UpdateStopTimeUpdates(IEnumerable<Tuple<FeedEntity, List<TripUpdate.StopTimeUpdate>>> updates, NpgsqlConnection dbConnection)
        {
            _logger.LogInformation("Collecting stop time updates data");
            using var transaction = await dbConnection.BeginTransactionAsync();
            var stopTimeUpdates = new List<PsqlStopTimeUpdate>();

            foreach (var tripBundle in updates)
            {
                var updatesArray = tripBundle.Item2.Select(update => new PsqlStopTimeUpdate()
                {
                    TripId = tripBundle.Item1.TripUpdate.Trip.TripId,
                    DataOrigin = "OpenOV",
                    InternalId = Guid.NewGuid(),
                    LastUpdated = DateTimeOffset.UtcNow,
                    StopSequence = (int)update.StopSequence,
                    StopId = update.StopId,
                    ArrivalDelay = update.Arrival?.Delay,
                    ArrivalTime = TimeOnly.FromTimeSpan(GetDTCTime(update.Arrival?.Time)?.TimeOfDay ?? TimeSpan.FromSeconds(0)),
                    ArrivalUncertainty = update.Arrival?.Uncertainty,
                    DepartureDelay = update.Departure?.Delay,
                    DepartureTime = TimeOnly.FromTimeSpan(GetDTCTime(update.Departure?.Time)?.TimeOfDay ?? TimeSpan.FromSeconds(0)),
                    DepartureUncertainty = update.Departure?.Uncertainty,
                    ScheduleRelationship = update.schedule_relationship.ToString()
                }).ToArray();
                stopTimeUpdates.AddRange(updatesArray);

            }

            _logger.LogInformation("Inserting {amount} updates", stopTimeUpdates.Count);
            // Call the stored procedure once with the DataTable
            var command = new NpgsqlCommand("CALL public.upsert_trip_update_array_stop_time(@updates)", dbConnection)
            {
                CommandType = CommandType.Text,
                Transaction = transaction
            };

            var parameter = command.Parameters.AddWithValue("@updates", stopTimeUpdates);

            await command.ExecuteNonQueryAsync();
            await transaction.CommitAsync();
        }

        private async Task ProcessTripUpdateAsync(NpgsqlConnection dbConnection, IEnumerable<FeedEntity> tripUpdates)
        {
            _logger.LogInformation("Inserting trip updates");
            using var transaction = await dbConnection.BeginTransactionAsync();

            var updatesArray = tripUpdates.Select(tripUpdate => new PsqlTripUpdate
            {
                Id = tripUpdate.Id,
                DataOrigin = "OpenOV",
                InternalId = Guid.NewGuid(),
                LastUpdated = DateTimeOffset.UtcNow,
                Delay = tripUpdate.TripUpdate?.Delay,
                MeasurementTime = GetDTCTime(tripUpdate.TripUpdate?.Timestamp)
            }).ToArray();

            var command = new NpgsqlCommand("CALL public.upsert_trip_update_array(@updates)", dbConnection)
            {
                CommandType = CommandType.Text,
                Transaction = transaction
            };

            var parameter = command.Parameters.AddWithValue("@updates", updatesArray);
            //parameter.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Array | NpgsqlDbType.Box;

            await command.ExecuteNonQueryAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Finished inserting trip updates");
        }



        private async Task ProcessVehiclePositionAsync(List<Tuple<FeedEntity, VehiclePosition>[]> vehicleUpdates)
        {
            _logger.LogInformation("Inserting vehicle positions {items}", vehicleUpdates.Count);
            var dbConnection = await _dataSource.OpenConnectionAsync();
            using var transaction = await dbConnection.BeginTransactionAsync();
            var items = new List<PsqlPositionUpdate>();

            foreach (var vehiclePositions in vehicleUpdates)
            {
                // Prepare the array of positions to be inserted
                var positionArray = vehiclePositions.Select(vehiclePosition =>
                {
                    double? latitude = null;
                    double? longitude = null;
                    
                    if (vehiclePosition.Item2.Position != null)
                    {
                        latitude = (double)vehiclePosition.Item2.Position?.Latitude;
                        longitude = (double)vehiclePosition.Item2.Position?.Longitude;
                    }

                    return new PsqlPositionUpdate
                    {
                        id = vehiclePosition.Item1.Id,
                        data_origin = "OpenOV",
                        internal_id = Guid.NewGuid(),
                        last_updated = DateTimeOffset.UtcNow,
                        trip_id = vehiclePosition.Item2.Trip?.TripId,
                        latitude = latitude,
                        longitude = longitude,
                        stop_id = string.IsNullOrWhiteSpace(vehiclePosition.Item2.StopId) ? null : vehiclePosition.Item2.StopId,
                        current_status = vehiclePosition.Item2.CurrentStatus.ToString(),
                        measurement_time = GetDTCTime(vehiclePosition.Item2.Timestamp),
                        congestion_level = vehiclePosition.Item2.congestion_level.ToString(),
                        occupancy_status = vehiclePosition.Item2.occupancy_status.ToString(),
                        occupancy_percentage = null
                    };
                }).ToArray();
                items.AddRange(positionArray);
            }

            // Define the command for batched upsert
            var command = new NpgsqlCommand("CALL public.upsert_position_array(@positions)", dbConnection)
            {
                CommandType = CommandType.Text,
                Transaction = transaction
            };

            // Add array parameter to the command
            var parameter = command.Parameters.AddWithValue("@positions", items.ToArray());
            // Set appropriate NpgsqlDbType if necessary, e.g., NpgsqlDbType.Array

            await command.ExecuteNonQueryAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Finished inserting vehicle positions");
        }


        static DateTimeOffset? GetDTCTime(ulong? seconds)
        {
            return GetDTCTime((long?)seconds);
        }

        static DateTimeOffset? GetDTCTime(long? seconds)
        {
            if (seconds != null)
            {
                DateTimeOffset dateTime = new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, TimeSpan.FromMicroseconds(0));
                dateTime = dateTime.AddSeconds((long)seconds).ToLocalTime();
                return dateTime;
            }
            return null;
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

    public class PsqlTripUpdate
    {
        public string Id { get; set; }
        public string DataOrigin { get; set; }
        public Guid InternalId { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
        public int? Delay { get; set; }
        public DateTimeOffset? MeasurementTime { get; set; }
    }

    public class PsqlStopTimeUpdate
    {
        public string DataOrigin { get; set; }
        public Guid InternalId { get; set; } = Guid.NewGuid();
        public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.UtcNow;
        public int StopSequence { get; set; }
        public string StopId { get; set; }
        public required string TripId { get; set; }
        public int? ArrivalDelay { get; set; }
        public TimeOnly? ArrivalTime { get; set; }
        public int? ArrivalUncertainty { get; set; }
        public int? DepartureDelay { get; set; }
        public TimeOnly? DepartureTime { get; set; }
        public int? DepartureUncertainty { get; set; }
        public string ScheduleRelationship { get; set; }
    }

}
