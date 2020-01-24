# Changelog

- All notable changes to this project will be documented in this file.
- **Versioning**: This project uses does not use semantic versioning as it keeps the major to 0 to underline that it's not production ready.
- Changelog format is modified variant of [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [Unreleased]

- **Added**
  - Encryption
    - Support for padding in Blowfish algorithm.
    - Both fast & safe (AES + Blowfish) supports setting padding mode.
  - Append multiple bytes (in a `SafeMemoryStream`) to `SafeBytes`
- **Changed**
  - Simplified builds: targets only `netstandard2.0`, `netstandard1.6` and `NET4.5`.
  - Renamed "Infrastructure" to "Core"
  - Refactorings & better documentation
- **Fixed**
    - `SafeMemoryStream` ends up in infinite loop while reading when its empty.

## [0.3.1] - 2019-12-24

- **Changed**
  - Using `.ConfigureAwait(false)` in async methods to better support UI applications.
  - More strict disposed checks
  - SafeString & SafeBytes can be revealed truly thread-safe.
- **Fixed**
  - Issue with `SafeBytes` causing some logic to fail is solved (wrong arbitrary byte logic after 0.3.0).
  - ToByteArray in `SafeBytes` leads to corrupted memory.
  - `SafeMemoryStream`: Fixed stack overflow in .NET Framework and .NET Standard 2.0
  - SafeString / Bytes cannot be revealed twice: bug with encryption of inner bytes

## [0.3.0] - 2019-03-30

- **Security**
  - `IFastRandom` is now seeded with strong random data.
  - Fixed memory leak problem in `SafeStringToStringMarshaler`.
  - Protection against timing attacks in `SafeByte`/`SafeString` comparisons.
- **Added**
  - Added support for `netstandard2.0`, `net462`, `net47`, `net471`, `net472`
- **Fixed**
  - Sanity check hash size function returning wrong value.
  - More minor fixes
- **Changed**
  - Updated framework dependencies to the latest available for different platforms
- **Removed**
  - Removed unnecessary code from `StartEarly`.
- **Performance**
  - Optimized serialization and encryption with async operations.
  - Minor performance optimizations in random generators.
  - More parallel implementation in inner classes.
  - Other minor performance optimizations

## [0.2.2] - 2016-12-29

- **Security**
  - Fixed `MemoryProtector` not protecting data in .NET Core.
- **Added**
  - All .NET platforms above 4.0 are supported : 4.0, 4.5, 4.5.1, 4.5.2, 4.6, 4.6.1
  - `MemoryProtector` now uses Blowfish encryption to protect data depending on .NET Core platform.

## [0.2.1] - 2016-11-27

- **Added**
  - XML documentation is included in NuGet package.
- **Fixed**
  - Not being able to read IL-bytes in .NET Core
  - Minor bug fixes

## [0.2.0] - 2016-11-24

- **Security**
  - `FastRandom` generates secure numbers.
- **Added**
  - .NET Core is supported.
- **Changed**
  - More tests, more/better documentation and refactorings.

## [0.1.0] - 2016-11-24

- Initial release

## All releases

- [Unreleased] : https://github.com/undergroundwires/SafeOrbit/compare/0.3.1...HEAD
- [0.3.1] : https://github.com/undergroundwires/SafeOrbit/compare/0.3.1...0.3.0
- [0.3.0] : https://github.com/undergroundwires/SafeOrbit/compare/0.2.2...0.3.0
- [0.2.2] : https://github.com/undergroundwires/SafeOrbit/compare/0.2.1...0.2.2
- [0.2.1] : https://github.com/undergroundwires/SafeOrbit/compare/0.2.0...0.2.1
- [0.2.0] : https://github.com/undergroundwires/SafeOrbit/releases/tag/0.2
