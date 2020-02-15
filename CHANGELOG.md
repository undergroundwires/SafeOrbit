# Changelog

- All notable changes to this project will be documented in this file.
- **Versioning**: This project uses does not use semantic versioning as it keeps the major to 0 to underline that it's not production ready.
- Changelog format is modified variant of [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [Unreleased]

## [0.4.0] - 2020-02-15

- **Performance** 😲
  - Huge performance improvements.
  - Most of the public methods are now only async: if it can be async, it's async.
  - Adding byte to SafeBytes is now at least 2100% faster
    - Adding single character to SafeString is at least 8200% faster.
  - `GetHashCode()` in `SafeString` and `SafeBytes` is faster than light (0 ms) compared to older implementation (6s for 1000 char with a linear increase per char)
  - Decrypting all bytes in `SafeBytes` is now 214x faster for 10KB, much faster for bigger sizes (1 MB = 900 ms).
  - `AppendAsync(ISafeBytes)` and `DeepCloneAsync` are optimized in `SafeBytes`
    - It is much faster (from 170 seconds to 37 ms for 10KB data)
    - Leads to much faster `ToSafeBytes` in `SafeString`
  - Faster equality checks for bytes and strings without compromising security.
    - `SafeBytes.EqualsAsync(bytes)` for 100 KB; before: 93032ms, after: 20ms
    - `SafeBytes.EqualsAsync(ISafeBytes)` for 100 KB before: 183346ms, after: 15ms
    - `SafeString.EqualsAsync` gains huge performance from inner byte equality, also it does not encrypt & decrypt plain strings anymore.
- **Fixed**
  - `SafeMemoryStream` ends up in infinite loop while reading when its empty.
  - Cannot change `AlertChannel` on `SafeContainer`
- **Added**
  - Encryption
    - Support for padding in Blowfish algorithm.
    - Both fast & safe (AES + Blowfish) supports setting padding mode.
  - `AppendMany()` to append multiple bytes to `SafeBytes`
  - Added constructor that writes the given binary to `SafeMemoryStream`
  - Added `RevealDecryptedBytesAsync` in `SafeString`
  - Added immutable interfaces `IReadOnlySafeString` and `IReadOnlySafeBytes`
  - Injection message includes the type of the injected objects.
- **Changed**
  - ❗ Replaced sync methods with async variants.
  - Simplified builds: targets only `netstandard2.0`, `netstandard1.6` and `NET4.5`.
  - Namespace changes for better modular structure
  - Refactorings & better documentation.
  - Renamed decrypting methods in `SafeBytes`, `SafeByte` and `SafeString` with *RevealDecrypted* prefix.
  - Simplified revealing SafeString, now its through `RevealDecryptedStringAsync` on `SafeString`.
- **Removed**
  - `SafeString.ToSafeBytesAsync` is removed because of its inefficiency.

## [0.3.1] - 2019-12-24

- **Changed**
  - Using `.ConfigureAwait(false)` in async methods to better support UI applications.
  - More strict disposed checks
  - SafeString & SafeBytes can be revealed truly thread-safe.
- **Fixed**
  - Issue with `SafeBytes` causing some logic to fail is solved (wrong arbitrary byte logic after 0.3.0).
  - `ToByteArray()` in `SafeBytes` leads to corrupted memory.
  - `SafeMemoryStream`: Fixed stack overflow in .NET Framework and .NET Standard 2.0
  - `SafeString` / `SafeBytes` cannot be revealed twice: bug with encryption of inner bytes

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

- [Unreleased] : https://github.com/undergroundwires/SafeOrbit/compare/0.4.0...HEAD
- [0.4.0] : https://github.com/undergroundwires/SafeOrbit/compare/0.4.0...0.3.1
- [0.3.1] : https://github.com/undergroundwires/SafeOrbit/compare/0.3.1...0.3.0
- [0.3.0] : https://github.com/undergroundwires/SafeOrbit/compare/0.2.2...0.3.0
- [0.2.2] : https://github.com/undergroundwires/SafeOrbit/compare/0.2.1...0.2.2
- [0.2.1] : https://github.com/undergroundwires/SafeOrbit/compare/0.2.0...0.2.1
- [0.2.0] : https://github.com/undergroundwires/SafeOrbit/releases/tag/0.2
