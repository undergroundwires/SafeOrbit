


<img align="left" src="https://raw.githubusercontent.com/undergroundwires/SafeOrbit/master/docs/img/logo/logo_60x60.png"> 

# **SafeOrbit** - Protect your data and detect injections

## SafeOrbit is cryptographic security toolkit for .NET
[![NuGet Status](https://img.shields.io/nuget/v/SafeOrbit.svg?style=flat)](https://www.nuget.org/packages/SafeOrbit/) ![Build status](https://github.com/undergroundwires/AsyncWindowsClipboard/workflows/Build%20&%20test/badge.svg) [![contributions welcome](https://img.shields.io/badge/contributions-welcome-brightgreen.svg?style=flat)](https://github.com/undergroundwires/SafeOrbit/issues)

 **SafeOrbit** is a security toolset including different high performance algorithms and easy to use classes for advanced memory protection.

**SafeOrbit**'s primarly focus is [**strong memory protection**](#memory-security). It protects every byte in your application with encryption in transit and at rest. It safeguards your application against memory injections and timing attacks.

* You have [SafeBytes](#safebytes#) to protect binaries,
* [SafeString](#safestring#) to protect strings,
* [and even more to detect memory injections](#protect-your-classes#).

**SafeOrbit** is **easy to use** as it does not require you to have any knowledge of cryptology to take advantage of high security with simple abstractions that's implemented with security best-practices.

**SafeOrbit** provides also bunch of crypto tools to use strong and high performance algorithms for [encryption, hashing and random](#cryptography).


**SafeOrbit** is **performance friendly**. It's up to you to decide for trade-off between speed and more security. Services have `Safe` or `Fast` prefixes. `Fast` classes strive for both performance and security, but `Safe` classes focuses the security over performance. **For example** while [SafeEncryptor](#aes-the-ISafeEncrpytor) uses lots of iterations, salts, and IV, [FastEncryptor](#blowfish-the-IFastEncryptor) uses a faster encryption algorithm without any key deriving function. **Furthermore** most of the classes has a way to disable its protection. They let you change/disable the security level of the protection dynamically to gain more performance.

## Want to say thanks? :beer:

Hit the :star: star :star: button

## Contribute

Feel free to contribute by joining the coding process or opening [issues](https://github.com/undergroundwires/safeOrbit/issues). [Read more on wiki](https://github.com/undergroundwires/SafeOrbit/wiki/Contribute).

## License

[This project is MIT Licensed](LICENSE). It means that you're free to use **SafeOrbit** freely in any application, copy, and modify its code.

> It must not be required to be secret, and it must be able to fall into the hands of the enemy without inconvenience.
> -[Auguste Kerckhoffs](https://en.wikipedia.org/wiki/Kerckhoffs%27s_principle)

# Quick Documentation

[Visit wiki for full documentation](https://github.com/undergroundwires/SafeOrbit/wiki)

## Memory security

### SafeString <sub><sup>[(wiki)](https://github.com/undergroundwires/SafeOrbit/wiki/SafeBytes)</sub></sup>

* `SafeString` represents an encrypted string that guarantees to not leak your data in the memory while allowing modifications and comparisons.
* It has more advantages over `System.Security.SecureString` because of the security design of the **SafeOrbit**.

#### SafeString vs [System.Security.SecureString](https://msdn.microsoft.com/en-us/library/system.security.securestring(v=vs.110).aspx)

|                              | SecureString | SafeString |
|-----------------------------:|:------------:|:----------:|
|  Supports multiple encodings |       ✖     |     ✔      |
|      Safely character insert |       ✖     |     ✔      |
|      Safely character remove |       ✖     |     ✔      |
|                Safely equals |       ✖     |     ✔      |
|              Safely retrieve |       ✖     |     ✔      |
|      Reveal only single char |       ✖     |     ✔      |
|         Unlimited characters |       ✖     |     ✔      |
|     Timing attack protection |       ✖     |     ✔      |

### SafeBytes <sub><sup>[(wiki)](https://github.com/undergroundwires/SafeOrbit/wiki/SafeBytes)</sub></sup>

* `SafeBytes` is protected sequence of bytes in memory.
* It's a lower level module used by `SafeString`.
* You can hide any data from the memory, then modify and compare them safely without revealing the bytes.

## Detect injections

* You can detect injections for any of your `.NET` class including their
  * the state (data in the memory)
  * code that's loaded in memory
* Internal protection for `SafeOrbit` library be **enabled as default**.
  * You can disable it to gain more performance [by changing SafeOrbit's security settings](https://github.com/undergroundwires/SafeOrbit/wiki/Library-settings#change-security-settings).

### SafeObject <sub><sup>[(wiki)](https://github.com/undergroundwires/SafeOrbit/wiki/SafeObject)</sub></sup>

An object that can detect memory injections to itself.

```C#
    var safeObject = new SafeObject<Customer>();
    // Each change to the object's state or code must be using ApplyChanges
    safeObject.ApplyChanges((customer) => customer.SensitiveInfo = "I'm protected!");
    // Retrieve safe data
    var safeInfo = safeObject.Object.SensitiveInfo; // returns "I'm protected!" or alerts if any injection is detected
```

### SafeContainer <sub><sup>[(wiki)](https://github.com/undergroundwires/SafeOrbit/wiki/SafeContainer)</sub></sup>

* **`SafeContainer`** is a dependency container that detects and notifies injections to its instances.
* It's security mode can be changed dynamically.

### InjectionDetector <sub><sup>[(wiki)](https://github.com/undergroundwires/SafeOrbit/wiki/InjectionDetector)</sub></sup>

* A service that's consumed by `SafeContainer` and `SafeObject`.
* Lowest level of the injection detection and alerting mechanism.

## Cryptography

### Encryption <sub><sup>[(wiki)](https://github.com/undergroundwires/SafeOrbit/wiki/Encryption)</sub></sup>

Supported:

* Asynchronous encryption
* `ISafeEncryptor` a.k.a. **AES-256**
  * Considered as one of the strongest encryption algorithms.
  * Easy-to-use interface using best-practices such as PBKDF2 key derivation, random IV, salt and PKCS7 padding.
* `IFastEncryptor` a.k.a. **Blowfish**
  * Considered as one of the fastest encryption algorithms.
  * ECB & CBC (with IV) implementation that passes the vector tests.

### Hashers <sub><sup>[(wiki)](https://github.com/undergroundwires/SafeOrbit/wiki/Hashers)</sub></sup>

Supported :

* `ISafeHasher` a.k.a. **SHA512** for higher security.
* `IFastHasher` a.k.a. **MurmurHash (Murmur32)** for better performance, it should be seeded and salted.

### Random <sub><sup>[(wiki)](https://github.com/undergroundwires/SafeOrbit/wiki/Random)</sub></sup>

> What if your OS crypto random has in any way been undermined (for example, by a nefarious government agency, or simple incompetence)?

`SafeOrbit` guarantees not to reduce the strength of your crypto random. It has the ability to improve the strength of your crypto random.

## Speed up

* **For better performance**, it's **highly recommended** to start the application early in your application start with this line :

    ```C#
    SafeOrbitCore.Current.StartEarly();
    ```

* Memory injection is enabled as default.
  * It provides self security on client side applications, but on a protected server disabling the memory injection for more performance is recommended. [Read more on wiki](https://github.com/undergroundwires/SafeOrbit/wiki/Library-settings#change-security-settings).
