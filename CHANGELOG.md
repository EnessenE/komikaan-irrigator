# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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