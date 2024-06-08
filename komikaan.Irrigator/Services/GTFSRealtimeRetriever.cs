using ProtoBuf;
using System.Net;
using TransitRealtime;

namespace komikaan.Irrigator.Services
{
    public class GTFSRealtimeRetriever : IHostedService
    {
        private ILogger<GTFSRealtimeRetriever> _logger;

        public GTFSRealtimeRetriever(ILogger<GTFSRealtimeRetriever> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            WebRequest req = HttpWebRequest.Create("https://gtfs.ovapi.nl/nl/tripUpdates.pb");
            _logger.LogInformation("Downloaded pb.");
            FeedMessage feed = Serializer.Deserialize<FeedMessage>(req.GetResponse().GetResponseStream());
            foreach (FeedEntity entity in feed.Entities)
            {
                _logger.LogInformation("ID: {id}, {alert}, {effect}, {veh}", entity.Id, entity.Alert?.cause, entity.Alert?.effect, entity.Vehicle?.CurrentStatus);
                _logger.LogInformation("Trip: {id}, delay: {delay}, {stop}", entity.TripUpdate?.Trip.TripId, entity.TripUpdate?.Delay, entity.TripUpdate?.StopTimeUpdates.FirstOrDefault()?.StopSequence);
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
