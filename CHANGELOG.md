# Changelog

All notable changes to this project will be documented in this file.

[0.8.0] - 15-11-2026
### Changed
- Map the GTFS-RT `VehicleDescriptor` (vehicle id, label, license plate) from vehicle positions into the database, so the API can expose which vehicle is driving a trip
- Bump komikaan.GTFS to 4.5.0

[0.7.2] - 14-11-2026
### Fixed
- geops.ch alerts feed failing to parse with `Invalid wire-type (String)`: opentransportdata.swiss sends `TripDescriptor.modified_trip` (field 7) as a plain trip-id string instead of a `ModifiedTripSelector` message, which protobuf-net cannot parse. Bumped komikaan.GTFS to 4.4.2, which captures field 7 as raw bytes (`ModifiedTripRaw` / `ModifiedTripId`).

[0.7.1] - 14-11-2026
### Fixed
- GTFS-RT feeds failing to parse with ProtoBuf errors (`Invalid field in source data: 0`, `Invalid wire-type`) because a manually added `Accept-Encoding: gzip, deflate, br` header made servers return compressed bytes that the HttpClient never decompressed. The default client now uses a `SocketsHttpHandler` with `AutomaticDecompression = All`, and the non-protobuf sanity check detects gzip/zlib/zstd payloads by magic bytes.

[0.7.0] - 14-11-2026
### Changed
- Bump all NuGet dependencies to their latest versions
- Remove unused DapperExtensions and NetTopologySuite.Core dependencies (drops the vulnerable transitive Oracle.ManagedDataAccess.Core flagged by NU1903)

[0.6.2]
[0.6.1]
### Changed
- Include 'Type' in dictionary key

[0.6.0]
### Added
- ETag caching and conditional GET (If-None-Match) for feed downloads.
### Changed
- Request compressed responses via `Accept-Encoding` (gzip, deflate, br).
- Process suppliers concurrently with individual `PollingRate` settings.
- Default `PollingRate` to 10 minutes when not set and log a warning.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

[0.5.7]
[0.5.6]
[0.5.5]
[0.5.4]
### Changed
- Attempt to fix vehicle parsing

[0.5.3]
### Changed
- Vehicle related metrics

[0.5.2]
### Added
- Support type for clearly distinguishing feed types
### Changed
- Dont show counter metrics for irrelevant feeds

[0.5.1]
### Added
- Support type for clearly distinguishing feed types
### Changed
- Dont show counter metrics for irrelevant feeds

[0.5.0]
### Changed
- Upgrade to .NET 10
- Add metrics support

[0.4.4]
### Changed
- Fix DBConnections not properly being closed

[0.4.2]
### Changed
- Nullables in TripUpdates
- Filter alerts by not removing them from the existing list

[0.4.1]
[0.4.0]
### Added
- Proper support for alerts entities
### Changed
- Remove bloated
- Changed to Komikaan.GTFS instead of gtfsrealtimebindings
- Refactored some code
- Remove GUID internal IDs for some entities

[0.3.14]
### Added
- Log important things
### Changes
- Cancel realtime requests in the foreach if we are stopping

[0.3.12]
### Changed
- Catch OverflowException

[0.3.11]
### Changed
- Some basic alert support rewritten
- Catch ProtoExceptions

[0.3.10]
[0.3.9]
### Changed
- Catch exceptions thrown during lookup of realtime feeds

[0.3.8]
### Changed
- Get times from UTC (8)
- 
[0.3.7]
### Changed
- Get times from UTC (2)
- Dont crash on HTTP timeout

[0.3.6]
[0.3.5]
### Changed
- Get times from UTC

[0.3.4]
### Changed
- Actaully publish the /health endpoint

[0.3.3]
### Added
- Catch database exception
- Database healthchecks
### Changed
- Allow empty stop time updates for a trip update preventing a crash

[0.3.2]
### Changed
- Fix updates being written away under the same supplier

[0.3.1]
### Changed
- Fix time parsing if value is empty

[0.3.0]
### Added
- Scope logging
- Retrieve realtime data feeds from database

[0.2.0]
### Added
- Basic retrieval of positions and tripupdates and saving into the database
- Temporarily removed alerts code
- Add user agent

[0.1.0]
### Added
- Basic retrieval of positions and tripupdates and saving into the database