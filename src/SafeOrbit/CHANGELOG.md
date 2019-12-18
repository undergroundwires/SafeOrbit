# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
this project uses does not use semantic versioning as it keeps the major to 0 to underline that it's not production ready.

## [Unreleased]
### Changed
- Using `.ConfigureAwait(false)` in async methods to better support UI applications.

### Fixed
- Issue with SafeBytes causing some logic to fail is solved (wrong arbitrary byte logic after 0.3.0).

## [0.3.0] - 2019-03-30
### Security
- IFastRandom is now seeded with strong random data.
- Fixed memory leak problem in SafeStringToStringMarshaler.
- Protection against timing attacks in SafeByte/SafeString comparisons.

### Added
- Added support for netstandard2.0, net462, net47, net471, net472

### Fixed
- Sanity check hash size function returning wrong value.

### Changed
- More small optimizations & fixes
- Updated framework dependencies to the latest available for different platforms

### Removed
- Removed unnecessary code from `StartEarly`.

## Performance
- Optimized serialization and encryption with async operations.
- Small optimizations in random generators.
- More parallel implementation in inner classes.

## [0.2.2] - 2016-12-29
### Added
- All .NET platforms above 4.0 are supported : 4.0, 4.5, 4.5.1, 4.5.2, 4.6, 4.6.1

### Fixed
- Fixed MemoryProtector: MemoryProtector now uses different strategies to protect data depending on the platform.

## [0.2.1] - 2016-11-27
### Changed
  - Xml documentation are included in nuget package.

### Fixed
  - .NET CORE only fix :  Not being able to read IL-bytes
  - Minor bug fixes

## [0.2.0] - 2016-11-24
### Security
- [Security] FastRandom generates secure numbers.

### Added
- .NET Core is supported.

### Changed
- More tests, more/better documentations and refactorings.

[Unreleased]: https://github.com/undergroundwires/SafeOrbit/compare/0.3.0...HEAD
[0.3.0]: https://github.com/undergroundwires/SafeOrbit/compare/0.2.2...0.3.0
[0.2.2]: https://github.com/undergroundwires/SafeOrbit/compare/0.2.1...0.2.2
[0.2.1]: https://github.com/undergroundwires/SafeOrbit/compare/0.2.0...0.2.1
[0.2.0]: https://github.com/undergroundwires/SafeOrbit/releases/tag/0.2