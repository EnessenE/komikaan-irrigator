using Dapper;
using ProtoBuf;
using System.Data;
using System.Diagnostics;
using System.Net;
using TransitRealtime;

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
            //TODO: Take actually scale this


            WebRequest req = HttpWebRequest.Create("https://gtfs.ovapi.nl/nl/alerts.pb");
            _logger.LogInformation("Downloaded pb.");
            FeedMessage feed = Serializer.Deserialize<FeedMessage>(req.GetResponse().GetResponseStream());
            var count = feed.Entities.Where(entity => entity.Alert != null).Count();
            _logger.LogInformation("Alert: {cnt}", count);
            var stop = Stopwatch.StartNew();
            foreach (FeedEntity entity in feed.Entities)
            {

                var alert = entity.Alert;

                if (alert != null)
                {
                    await ProcessMessageAsync(alert);
                }
            }
        }


        private async Task ProcessMessageAsync(Alert alert)
        {
            _logger.LogInformation("Alert: {1}, {2}, {3}, {4}, {5}", alert.HeaderText?.Translations.FirstOrDefault()?.Language, alert.DescriptionText?.Translations.FirstOrDefault()?.Language, alert.TtsDescriptionText?.Translations.FirstOrDefault()?.Language, alert.cause, alert.effect);
            using var dbConnection = new Npgsql.NpgsqlConnection(_connectionString);

            await dbConnection.ExecuteAsync(
            @"CALL public.upsert_alert(@data_origin, @internal_id, @last_updated, @active_periods, @cause, @effect, @url, @header_text, @description_text, @tts_header_text, @tts_description_text, @severity_level)",
            new
            {
                data_origin = "OpenOV",
                internal_id = Guid.NewGuid(),
                last_updated = DateTimeOffset.UtcNow,
                active_periods = GetActivePeriod(alert.ActivePeriods),
                cause = alert.cause.ToString(),
                effect = alert.effect.ToString(),
                url = GetStringUuid(alert.Url),
                header_text = GetStringUuid(alert.HeaderText),
                description_text = GetStringUuid(alert.DescriptionText),
                tts_header_text = GetStringUuid(alert.TtsHeaderText),
                tts_description_text = GetStringUuid(alert.TtsDescriptionText),
                severity_level = ((Alert.SeverityLevel?) alert.severity_level)?.ToString() ?? null,
            },
            commandType: CommandType.Text
            );
        }

        private Guid? GetStringUuid(TranslatedString? url)
        {
            if (url != null)
            {
                return Guid.Empty;
            }
            else return null;
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
