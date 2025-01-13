using Dapper;
using Npgsql;
using ProtoBuf;
using System.Data;
using System.Diagnostics;
using TransitRealtime;
using komikaan.Irrigator.Extensions;
using static Dapper.SqlMapper;
using komikaan.Irrigator.Models;
using System;
using static TransitRealtime.TranslatedString;

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
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            _logger = logger;
            _connectionString = config.GetConnectionString("gtfs");
            _httpClient = httpClient;
            _config = config;
            _dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
            _dataSourceBuilder.MapComposite<PsqlTripUpdate>("trip_update_type");
            _dataSourceBuilder.MapComposite<PsqlAlertUpdate>("alert_update");
            _dataSourceBuilder.MapComposite<PsqlStopTimeUpdate>("trip_update_stop_time_type");
            _dataSourceBuilder.MapComposite<PsqlPositionUpdate>("position_entity_type");
            _dataSource = _dataSourceBuilder.Build();

            _httpClient.DefaultRequestHeaders.Add("User-Agent", "irrigator/reasulus.nl");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var interval = _config.GetValue<TimeSpan>("WorkInterval", TimeSpan.FromSeconds(30));
                while (!stoppingToken.IsCancellationRequested)
                {
                    var realtimeFeeds = await GetFeedsAsync();
                    _logger.LogInformation("Starting a process cycle for {amount} feeds", realtimeFeeds.Count);

                    foreach (var feed in realtimeFeeds)
                    {
                        using (_logger.BeginScope(feed.SupplierConfigurationName))
                        {
                            if (feed.Enabled)
                            {
                                _logger.LogInformation("Started an import");
                                await RunFeedImport(feed, stoppingToken);
                            }
                            else
                            {
                                _logger.LogInformation("Skipped as its disabled");
                            }

                        }
                    }
                    _logger.LogInformation("Finished, waiting for the interval of {time}", interval);
                    await Task.Delay(interval, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went horribly wrong, force halted processing and not continueing due to poor implentation");
            }
            _logger.LogInformation("This line indicated the program will stop processing from this point forward");
        }

        private async Task RunFeedImport(RealTimeFeed feed, CancellationToken stoppingToken)
        {
            try
            {
                await FeedImport(feed);
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "Failed to write away exception");
                if (exception.StatusCode != null)
                {
                    var code = (int)exception.StatusCode;
                    if (code >= 400 && code <= 499)
                    {
                        _logger.LogError("Unaccepted error, crashing loop");
                        throw;
                    }
                }
            }
            catch (NpgsqlException exception)
            {
                _logger.LogError(exception, "Database failure while importing");
            }
            catch (IOException exception)
            {
                _logger.LogError(exception, "Failure while trying to process the file");
            }
            catch (TaskCanceledException exception)
            {
                _logger.LogError(exception, "Failure while trying to process the file");
            }
            catch(ProtoException protoException)
            {
                _logger.LogError(protoException, "Failed to parse file, a corrupted file may have been processed");
            }
            catch (OverflowException overflowException)
            {
                _logger.LogError(overflowException, "Failed to parse file, a corrupted file may have been processed");
            }
        }

        private async Task<List<RealTimeFeed>> GetFeedsAsync()
        {
            try
            {
                await using var connection = await (_dataSourceBuilder.Build()).OpenConnectionAsync();
                var data = await connection.QueryAsync<RealTimeFeed>(
                @"select * from get_all_realtime_feeds()",
                    commandType: CommandType.Text
                );
                return data.ToList();
            }
            catch (NpgsqlException exception)
            {
                _logger.LogError(exception, "Failed to lookup realtime feeds");
                return new List<RealTimeFeed>();
            }
        }

        private async Task FeedImport(RealTimeFeed feed)
        {
            //TODO: Figure out scaling how to factor in supplierconfigurations for many feeds
            //TODO: Actually scale this in all directions
            //TODO: what is this mess i made

            var request = new HttpRequestMessage(HttpMethod.Get, feed.Url);

            if (!string.IsNullOrWhiteSpace(feed.Header))
            {
                request.Headers.Add(feed.Header, feed.HeaderSecret);
                _logger.LogInformation("Added header {name} to the request", feed.Header);
            }

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Downloaded pb.");
                FeedMessage feedMessage = Serializer.Deserialize<FeedMessage>(response.Content.ReadAsStream());
                _logger.LogInformation("Parsed pb.");
                _logger.LogInformation("Entities: {cnt}", feedMessage.Entities.Count);
                _logger.LogInformation("Alert: {cnt}", feedMessage.Entities.Where(entity => entity.Alert != null).Count());
                _logger.LogInformation("VehicleUpdates: {cnt}", feedMessage.Entities.Where(entity => entity.Vehicle != null).Count());
                _logger.LogInformation("TripUpdates: {cnt}", feedMessage.Entities.Where(entity => entity.TripUpdate != null).Count());

                var stop = Stopwatch.StartNew();
                var dbConnection = await _dataSource.OpenConnectionAsync();

                await DetectTripUpdate(feed, feedMessage, dbConnection);
                await DetectVehicleUpdate(feed, feedMessage, dbConnection);
                await DetectAlertUpdate(feed, feedMessage, dbConnection);

                await dbConnection.CloseAsync();
            }
            else
            {
                _logger.LogError("Failed to call target api: {reason} - {msg}", response.StatusCode, response.ReasonPhrase);
            }
        }
        private async Task DetectAlertUpdate(RealTimeFeed realtimeFeed, FeedMessage feed, NpgsqlConnection dbConnection)
        {
            if (feed.Entities.Any(x => x.Alert != null))
            {
                await ProcessAlertUpdateAsync(realtimeFeed, dbConnection, feed.Entities);
            }
        }

        private async Task DetectTripUpdate(RealTimeFeed realtimeFeed, FeedMessage feed, NpgsqlConnection dbConnection)
        {
            if (feed.Entities.Any(x => x.TripUpdate != null))
            {
                await ProcessTripUpdateAsync(realtimeFeed, dbConnection, feed.Entities);
                foreach (var batch in feed.Entities.Select(x => new Tuple<FeedEntity, List<TripUpdate.StopTimeUpdate>?>(x, x.TripUpdate?.StopTimeUpdates)).ToList().ChunkBy(500))
                {
                    await UpdateStopTimeUpdates(realtimeFeed, batch, dbConnection);
                }
            }
        }

        private async Task DetectVehicleUpdate(RealTimeFeed realtimeFeed, FeedMessage feed, NpgsqlConnection dbConnection)
        {
            if (feed.Entities.Any(x => x.Vehicle != null))
            {
                var vehiclePositonsToUpdate = feed.Entities.Where(entity=> entity.Vehicle != null).Select(entity => new Tuple<FeedEntity, VehiclePosition>(entity, entity.Vehicle)).ToList();
                _logger.LogInformation("Total of {x} updates", vehiclePositonsToUpdate.Count());
                foreach (var batch in vehiclePositonsToUpdate.ChunkBy(500))
                {
                    await ProcessVehiclePositionAsync(realtimeFeed, dbConnection, batch);
                }
            }
        }

        private async Task UpdateStopTimeUpdates(RealTimeFeed feed, IEnumerable<Tuple<FeedEntity, List<TripUpdate.StopTimeUpdate>?>> updates, NpgsqlConnection dbConnection)
        {
            _logger.LogInformation("Collecting stop time updates data");
            using var transaction = await dbConnection.BeginTransactionAsync();
            var stopTimeUpdates = new List<PsqlStopTimeUpdate>();

            foreach (var tripBundle in updates)
            {
                if (tripBundle.Item2 != null)
                {
                    var updatesArray = tripBundle.Item2.Select(update => new PsqlStopTimeUpdate()
                    {
                        TripId = tripBundle.Item1.TripUpdate.Trip.TripId,
                        DataOrigin = feed.SupplierConfigurationName,
                        InternalId = Guid.NewGuid(),
                        LastUpdated = DateTimeOffset.UtcNow,
                        StopSequence = (int)update.StopSequence,
                        StopId = update.StopId,
                        ArrivalDelay = update.Arrival?.Delay,
                        ArrivalTime = GetTime(update.Arrival?.Time),
                        ArrivalUncertainty = update.Arrival?.Uncertainty,
                        DepartureDelay = update.Departure?.Delay,
                        DepartureTime = GetTime(update.Departure?.Time),
                        DepartureUncertainty = update.Departure?.Uncertainty,
                        ScheduleRelationship = update.schedule_relationship.ToString(),
                    }).ToArray();
                    stopTimeUpdates.AddRange(updatesArray);
                }

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

        private static DateTimeOffset? GetTime(long? update)
        {
            if (update.HasValue)
            {
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                origin = origin.AddSeconds(update!.Value);
                return origin;
            }
            return null;
        }

        private async Task ProcessTripUpdateAsync(RealTimeFeed feed, NpgsqlConnection dbConnection, IEnumerable<FeedEntity> tripUpdates)
        {
            _logger.LogInformation("Inserting trip updates");
            using var transaction = await dbConnection.BeginTransactionAsync();

            var updatesArray = tripUpdates.Select(tripUpdate => new PsqlTripUpdate
            {
                Id = tripUpdate.Id,
                DataOrigin = feed.SupplierConfigurationName,
                InternalId = Guid.NewGuid(),
                LastUpdated = DateTimeOffset.UtcNow,
                Delay = tripUpdate.TripUpdate?.Delay,
                MeasurementTime = tripUpdate.TripUpdate?.Timestamp.ToDateTime()
            }).ToArray();

            var command = new NpgsqlCommand("CALL public.upsert_trip_update_array(@updates)", dbConnection)
            {
                CommandType = CommandType.Text,
                Transaction = transaction
            };

            var parameter = command.Parameters.AddWithValue("@updates", updatesArray);

            await command.ExecuteNonQueryAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Finished inserting trip updates");
        }
        private async Task ProcessAlertUpdateAsync(RealTimeFeed feed, NpgsqlConnection dbConnection, IEnumerable<FeedEntity> alertUpdates)
        {
            _logger.LogInformation("Inserting alert updates");
            using var transaction = await dbConnection.BeginTransactionAsync();

            // Map FeedEntity to PsqlAlertUpdate
            var updatesArray = alertUpdates.Select(tripUpdate => new PsqlAlertUpdate
            {
                Id = tripUpdate.Id,
                DataOrigin = feed.SupplierConfigurationName,
                InternalId = Guid.NewGuid(),
                LastUpdated = DateTimeOffset.UtcNow,
                Effect = (tripUpdate.Alert?.effect ?? Alert.Effect.UnknownEffect).ToString(),
                Cause = (tripUpdate.Alert?.cause ?? Alert.Cause.UnknownCause).ToString(),
                SeverityLevel = (tripUpdate.Alert?.severity_level ?? Alert.SeverityLevel.UnknownSeverity).ToString(),
                Url = tripUpdate.Alert?.Url?.ToString(),
                HeaderText = tripUpdate.Alert?.HeaderText?.Translations?.FirstOrDefault()?.Text?.ToString(),
                DescriptionText = tripUpdate.Alert?.DescriptionText?.Translations?.FirstOrDefault()?.Text?.ToString()
            }).ToArray();

            // Insert PsqlAlertUpdate array using a stored procedure
            var alertUpdateCommand = new NpgsqlCommand("CALL public.upsert_alert_update_array(@updates)", dbConnection)
            {
                CommandType = CommandType.Text,
                Transaction = transaction
            };

            var alertUpdateParam = alertUpdateCommand.Parameters.AddWithValue("@updates", updatesArray);
            //alertUpdateParam.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Composite;

            await alertUpdateCommand.ExecuteNonQueryAsync();

            // Insert active periods for each alert
            var activePeriodInserts = new List<PsqlActivePeriod>();
            foreach (var tripUpdate in alertUpdates)
            {
                if (tripUpdate.Alert?.ActivePeriods?.Count > 0)
                {
                    foreach (var period in tripUpdate.Alert.ActivePeriods)
                    {
                        activePeriodInserts.Add(new PsqlActivePeriod
                        {
                            alert_internal_id = updatesArray.First(u => u.Id == tripUpdate.Id).InternalId,
                            data_origin = feed.SupplierConfigurationName,
                            start_time = GetTime((long?)period.Start),
                            end_time = GetTime((long?)period.End)
                        });
                    }
                }
            }

            // Insert active periods into alert_active_periods table
            if (activePeriodInserts.Any())
            {
                var activePeriodCommand = new NpgsqlCommand(
                    "INSERT INTO public.alert_active_periods (alert_internal_id, data_origin, start_time, end_time) VALUES (@alert_internal_id, @data_origin, @start_time, @end_time)",
                    dbConnection)
                {
                    Transaction = transaction
                };

                foreach (var ap in activePeriodInserts)
                {
                    activePeriodCommand.Parameters.Clear();
                    activePeriodCommand.Parameters.AddWithValue("@alert_internal_id", ap.alert_internal_id);
                    activePeriodCommand.Parameters.AddWithValue("@data_origin", ap.data_origin);
                    activePeriodCommand.Parameters.AddWithValue("@start_time", ap.start_time.HasValue ? (object)ap.start_time.Value : DBNull.Value);
                    activePeriodCommand.Parameters.AddWithValue("@end_time", ap.end_time.HasValue ? (object)ap.end_time.Value : DBNull.Value);

                    await activePeriodCommand.ExecuteNonQueryAsync();
                }
            }

            // Process informed entities
            var entities = alertUpdates.Where(tripUpdate => tripUpdate.Alert != null && tripUpdate.Alert.InformedEntities != null).SelectMany(tripUpdate => tripUpdate.Alert.InformedEntities).ToList();
            var removed = entities.RemoveAll(entity => entity == null);
            _logger.LogInformation("Removed {count} informed empty alert entities", removed);
            //foreach (var chunk in entities.Chunk(500))
            //{
            //    await InformEntitiesAsync(feed, chunk.ToList(), dbConnection);
            //}

            await transaction.CommitAsync();
            _logger.LogInformation("Finished inserting trip updates, active periods, and informed entities");
        }




        private async Task ProcessVehiclePositionAsync(RealTimeFeed feed, NpgsqlConnection dbConnection, List<Tuple<FeedEntity, VehiclePosition>> vehicleUpdates)
        {
            _logger.LogInformation("Inserting vehicle positions {items}", vehicleUpdates.Count);
            using var transaction = await dbConnection.BeginTransactionAsync();
            var items = new List<PsqlPositionUpdate>();

            // Prepare the array of positions to be inserted
            var positionArray = vehicleUpdates.Select(vehiclePosition =>
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
                    data_origin = feed.SupplierConfigurationName,
                    internal_id = Guid.NewGuid(),
                    last_updated = DateTimeOffset.UtcNow,
                    trip_id = vehiclePosition.Item2.Trip?.TripId,
                    latitude = latitude,
                    longitude = longitude,
                    stop_id = string.IsNullOrWhiteSpace(vehiclePosition.Item2.StopId) ? null : vehiclePosition.Item2.StopId,
                    current_status = vehiclePosition.Item2.CurrentStatus.ToString(),
                    measurement_time = vehiclePosition.Item2.Timestamp.ToDateTime(),
                    congestion_level = vehiclePosition.Item2.congestion_level.ToString(),
                    occupancy_status = vehiclePosition.Item2.occupancy_status.ToString(),
                };
            }).ToArray();
            items.AddRange(positionArray);

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


        private async Task InformEntitiesAsync(RealTimeFeed feed, List<EntitySelector> informedEntities, NpgsqlConnection dbConnection)
        {
            //Todo: batching
            _logger.LogInformation("entities to inform: {cnt}", informedEntities.Count);
            foreach (EntitySelector entity in informedEntities)
            {

                await dbConnection.ExecuteAsync(
                    @"CALL public.upsert_alert_entities(@data_origin, @internal_id, @last_updated, @agency_id, @route_id, @trip_id, @stop_id)",
                    new
                    {
                        data_origin = feed.SupplierConfigurationName,
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
