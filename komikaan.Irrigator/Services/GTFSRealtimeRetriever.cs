using Dapper;
using NetTopologySuite.Utilities;
using Npgsql;
using ProtoBuf;
using System.Data;
using System.Diagnostics;
using System.Net;
using TransitRealtime;
using static Dapper.SqlMapper;

namespace komikaan.Irrigator.Services
{
    public class GTFSRealtimeRetriever : IHostedService
    {
        private readonly ILogger<GTFSRealtimeRetriever> _logger;
        private readonly string? _connectionString;
        private readonly HttpClient _httpClient;

        public GTFSRealtimeRetriever(ILogger<GTFSRealtimeRetriever> logger, IConfiguration config, HttpClient httpClient)
        {
            _logger = logger;
            _connectionString = config.GetConnectionString("gtfs2");
            _httpClient = httpClient;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //TODO: Figure out scaling how to factor in supplierconfigurations for many feeds
            //TODO: Actually scale this in all directions

            var response = await _httpClient.GetAsync("https://gtfs.ovapi.nl/nl/alerts.pb");
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Downloaded pb.");
                FeedMessage feed = Serializer.Deserialize<FeedMessage>(response.Content.ReadAsStream());
                _logger.LogInformation("Parsed pb.");
                var count = feed.Entities.Where(entity => entity.Alert != null).Count();
                _logger.LogInformation("Alert: {cnt}", count);
                var stop = Stopwatch.StartNew();
                foreach (FeedEntity entity in feed.Entities)
                {
                    var alert = entity.Alert;
                    if (alert != null)
                    {
                        await ProcessMessageAsync(entity, alert);
                    }
                }
            }
            else
            {
                _logger.LogError("Failed to call target api: {reason} - {msg}", response.StatusCode, response.ReasonPhrase);
            }
        }


        private async Task ProcessMessageAsync(FeedEntity entity, Alert alert)
        {
            _logger.LogInformation("Alert: {1}, {2}, {3}, {4}, {5}", alert.HeaderText?.Translations.FirstOrDefault()?.Language, alert.DescriptionText?.Translations.FirstOrDefault()?.Language, alert.TtsDescriptionText?.Translations.FirstOrDefault()?.Language, alert.cause, alert.effect);
            using var dbConnection = new NpgsqlConnection(_connectionString);

            await dbConnection.ExecuteAsync(
                @"CALL public.upsert_alert(@data_origin, @internal_id, @id, @last_updated, @active_periods, @cause, @effect, @url, @header_text, @description_text, @tts_header_text, @tts_description_text, @severity_level)",
                new
                {
                    data_origin = "OpenOV",
                    internal_id = Guid.NewGuid(),
                    id = entity.Id,
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
}
