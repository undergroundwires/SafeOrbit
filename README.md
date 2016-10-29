#**SafeOrbit** - Protect your data and detect injections

> It must not be required to be secret, and it must be able to fall into the hands of the enemy without inconvenience.
> -[Auguste Kerckhoffs](https://en.wikipedia.org/wiki/Kerckhoffs%27s_principle)

<img align="left" src="https://raw.githubusercontent.com/undergroundwires/SafeOrbit/master/docs/img/icon.png">
## SafeOrbit is easy-to-use and strong security toolkit for `.NET` 

### The nuget package  [![NuGet Status](https://img.shields.io/nuget/v/SafeOrbit.svg?style=flat)](https://www.nuget.org/packages/SafeOrbit/)

While **SafeOrbit**'s primarly focus is [**strong memory protection**](#memory-security), it also provides a bunch of tools to implement strong and high performance algorithms for [encryption, hashing and random](#others). It can protect any data in the memory for you: you have [SafeBytes](#safebytes#) to protect binaries, [SafeString](#safestring#) to protect texts [and even more to protect your application against injections](#protect-your-classes#).

**SafeOrbit** is **easy to use** as it does not require you to have a big knowledge of cryptology to take advantage of higher security.

**SafeOrbit** is **performance friendly**. Most of the abstractions has a way to disable its protection. Some of them have `Safe` or `Fast` prefixes. `Fast` classes strive for both performance and security, but `Safe` classes focuses the security over performance. **For example** while [SafeEncryptor](#aes-the-ISafeEncrpytor) uses lots of iterations, salts, and IV, [FastEncryptor](#blowfish-the-IFastEncryptor) uses a faster encryption alghoritm without any key deriving function. **Furthermore** some classes and lets you change/disable the security level of the protection dynamically to gain more performance.

**SafeOrbit** is **well tested** as it should be for a security library. It has more than 3.000 green tests for around 3.000 lines of code (v0.1).

**SafeOrbit** is still under an **active development**. **``NET Core``** support is planned and will come out soon in the next release.

## Want to say thanks? :beer:

Hit the :star: star :star: button

## Contribute
Feel free to contribute by joining the coding process or opening [issues](https://github.com/undergroundwires/safeOrbit/issues). [Read more on wiki](https://github.com/undergroundwires/SafeOrbit/wiki/Contribute).

### Donate
You can also support the project by buying me a coffee

[![](https://raw.githubusercontent.com/undergroundwires/SafeOrbit/master/docs/img/paypal_donate.png)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=CXXENGW8QMABC)


## License
[This project is MIT Licensed](LICENSE).

It means that you're free to use **SafeOrbit** freely in any application, copy, and modify its code.

It's much appreciated if you name the library in credits section of your application.

# Documentation

* [Visit wiki for full documentation](https://github.com/undergroundwires/SafeOrbit/wiki)
* [Check html help file from repository](./docs/Help.chm)

## Quick documentation
**For better performance**, it's **highly recommended** to start the application early in your application start with this line :
```C#
 LibraryManagement.StartEary();
```
You can as well change the inner security settings of library. [Read more on wiki](https://github.com/undergroundwires/SafeOrbit/wiki/LibraryManagement#change-security-settings).

## Memory security

### SafeString : Protect your strings
`SafeString` represents an encrypted string that's not leaked in the memory. It's modifiable and can work with [different encodings](#supported-encodings#).

**Example usage:**

```C#
 var safeString = new SafeString();
 safeString.Append('t');
 safeString.Append('e');
 safeString.Append("st");
 //safeString will hold encrypted "test" text
  ```

`SafeString` uses [`SafeBytes`](#safebytes-protect-every-single-byte) as its internal data storage. That makes `SafeString` safe to both modify and compare with other `string` and `SafeString` instances.

#### SafeString vs [System.Security.SecureString](https://msdn.microsoft.com/en-us/library/system.security.securestring(v=vs.110).aspx) 

##### Safe to modify
`SecureString` is only char appendable and you need to reveal the sensitive information in order to be able to modify it. But `SafeString` can be safely modified by inserting chars/strings/bytes, deleting, replacing and more.

##### Safe to compare
`SecureString`'s are not comparable with each other. But the security design and architecture of `SafeString` makes it safe to compare with another sensitive information in a secure context.

##### Retrieve the protected data
`SecureString` is not retrievable, but `SafeString` is. You can use a built-in `SafeStringToStringMarshaler` class to do this. The marshaler is a disposable `string`, it promises the total removal of the data from the memory after disposing it. **Important :** `SafeStringToStringMarshaler` should be used very carefully it's probably not a good idea to reveal information in the memory even though it appears in milliseconds.

**Example usage:**
 ```C#
 using(var sm = new SafeStringToStringMarshaler(safeString))
    {
      // Use sm.String here.  While in the 'using' block, the string is accessible
      // but pinned in memory.  When the 'using' block terminates, the string is zeroed
      // out for security, and garbage collected as usual.
    }
```

#### Supported encodings
* Ansii
* Utf16LittleEndian
* Utf16BigEndian


### SafeBytes : Protect every single byte
SafeBytes encryption has different levels. The first level is to have cryptologically strong (hashed with a session key that's unique for each application start) unique identifiers for bytes. Those codes are used during the modification, and this way not a single byte becomes visible in the memory during modification of SafeString.

**Example usage:**

```C#
 var safeBytes = new SafeBytes();
 safeBytes.Append(0);
 safeBytes.Append(5);
 safeBytes.Append(10);
 //safeBytes will hold encrypted 0,5,10 as inner binary
  ```

#### SafeByte ####

`SafeByte` represents a single byte protected in the memory. They're hashed with a key that's different for each application start. The result of hash with a session key that's created by `SafeRandom`.

`SafeByte` is the first security layer of both `SafeString` and `SafeBytes`. 

`SafeByte` holds instructions to decrpyt the encryption of a single `byte`. The encrypted data (representing a single byte) is not visible in the memory.

Those id's allow equality checks without revealing the real data in the memory (pretty common memory leak, see **.NET**'s [`System.Security.SecureString`](https://msdn.microsoft.com/en-us/library/system.security.securestring%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396) or [`ProtectedData`](https://github.com/huahang/KeePass/blob/master/KeePassLibSD/ProtectedData.cs) from [**KeePass**](https://github.com/huahang/KeePass)). 

Creation of a new `SafeByte` instance is costly, therefore SafeOrbit caches them in the memory (using their id's as keys) and protect their code and state against memory injections. It's **recommended** to call **StartEarly** in your application to do this caching at application start.

## Detect injections

You can detect injections for the state (data in the memory), and/or code of any C# class. You can scan and alert the injections using [`InjectionDetector`](#injection-detector) in lower levels. However, it's  **recommended** to use [`SafeObject`](#safeobject#) for the object you want to track.

You can also use [`SafeContainer`](#safecontainer#) to have a factory that tracks all of its instances.

Internal protection for C# code and classes will be **disabled as default** for the SafeOrbit's inner code. To get notified by memory injections to SafeOrbit's library, you can enable it [by changing SafeOrbit's security settings](#change-security-settings).

### SafeObject

**`SafeObject`** is a class that provides locking pattern to access object in a thread safe matter. It uses `IInjectionProtector` internally that checks if the object is safe (not injected in the memory) on each access to the object. The security mode can be changed dynamically or in the constructor.

**Caution** : This class should not be used for strings or binaries (`byte[]`). Use the better classes that's specificialyl designed for this puproses : `SafeString` and `SafeBytes`.

#### Usage
```C#
            var safeObject = new SafeObject<Customer>(/*leaving default instance empty in constructor to get a new instance*/);
            //you can alternatively use an existing instance to protect: new SafeObject&lt;TObject&gt;(new InitialSafeObjectSettings(initialValue, true));
            //each change to the object's state or code must be using ApplyChanges
            safeObject.ApplyChanges((customer) => customer.Id = 5);
            //retrieve safe data
            var customerId = safeObject.Object.Id; //returns 5 or alerts if any injection is detected
```

#### Advanced usage
```C#
            //if the object's Id property becomes 0 by any non-applied change,
            //the SafeObject instance will alert a memory injection depending on its protection mode.
            safeObject.SetProtectionMode(SafeObjectProtectionMode.StateAndCode); //changes its protection mode to no protection
            //you can change the alert channel:
            safeObject.AlertChannel = InjectionAlertChannel.DebugWrite; //any detected injections will be alerted using the alert channel
            safeObject.SetProtectionMode(SafeObjectProtectionMode.NoProtection); //stops the protection of object,
            //SafeObject will never alert when it's not protected.
            var willAlert = safeObject.CanAlert; //returns false as the instance will only alert when it's protected.
```

### SafeContainer
**`SafeContainer`** is a dependency container that uses `InjectionProtector` to protect its instances and instance creation logic. It holds its data in an inner `SafeObject`. 

The security mode any of the `SafeContainer` can be disabled via calling `container.SetProtectionMode(SafeContainerProtectionMode.NonProtection)` for the instance. It can also be re-enabled by calling `container.SetProtectionMode(SafeContainerProtectionMode.FullProtection)`.

This class is used widely in `SafeOrbit` for dependency injection pattern, [you can change the inner injection protection](#change-security-settings) of `SafeOrbit` as well.

### InjectionDetector
**`InjectionProtector`** is the lowest level of injection detection. It's consumed by both `SafeObject` and `SafeContainer`. It keeps track of the state or IL-code of any class, stamps them regularly and validates that the class/object is not injected by comparing last stamps.

#### Example
```C#
            var obj = new[] {5, 10};
            var protector = new InjectionProtector(protectCode: true, protectState:true);
            protector.NotifyChanges(obj);
            protector.AlertChannel = InjectionAlertChannel.ThrowException;
            protector.AlertUnnotifiedChanges(obj); //does not throw
            obj[1] = 5;
            protector.AlertUnnotifiedChanges(obj); //throws as the change is not notified.
```


#### Example

Let's say that we have `UserCreditials` that holds sensitive data, and we want to secure this class in the memory.

```C#
var safeUserCreditials = new SafeObject<UserCreditials>(); //Creates a new instance of UserCreditials
safeUserCreditials.Object.Username = "newUserName"; // modify the inner object
safeUserCreditials.Object.Password = "safePassword";
safeUserCreditials.VerifyChanges(); //verifies that the changes are made by the application
safeUserCreditials.CloseToAllChanges(); //closes the object to further modifications (note that VerifyChanges cannot be used after this method is called)
```

You can use the `InitialSafeObjectSettings` class to set an existing instance of `UserCreditials` or more:

```C#
           var safeUserCreditials = new SafeObject<UserCreditials>(new InitialSafeObjectSettings
            {
                InitialValue = existingUserCreditials,
                IsModifiable = true,
                ProtectionMode = SafeObjectProtectionMode.StateAndCode
            }); //secures the "existingUserCreditials"
```

You can change the protection level depending on an object. For example if your object is stateless, there is no reason to protect the state and you can set the protection mode initially to `SafeObjectProtectionMode.JustCode` or use `safeObject.SetProtectionMode(SafeObjectProtectionMode.JustCode)` dynamically.

## Others

### Encryption

There are **Aes** and **Blowfish** implementations to provide stronger and faster security. They are  are pretty easy-to use and are implemented in a secure way. They can be used synchronously and asynchronously.

#### Aes the `ISafeEncrpytor`

**`ISafeEncryptor`** abstracts an encryption operation which is designed to be slower to be more secure.
Use it when you choose more security over performance (for most sensitive data).

**`AesEncryptor`** is the implementation of `ISafeEncryptor`. It uses =>
 - 256 bit key
 - random IV for each encryption
 - secures the key with with 100 iterations of Pbkdf2 key derivation function.

#### Blowfish the `IFastEncryptor`

**`IFastEncryptor`** abstracts a secure and faster encryption alghoritm.

Use it when the performance is also important as security.

It's implementation,**`Blowfish**`**, passes vector tests and can use different cipher modes. The default `CBC` mode uses a random IV while `ECB` has no IV protection.

#### Asynchronous encryption
Encryption algorithms implement `ICryptoTransform` and write a sequence of bytes to the memory stream asynchronously in the [cryptostream](https://msdn.microsoft.com/en-us/library/hh472379(v=vs.110).aspx).

### Hashing

Supported :
 - [MurmurHash (Murmur32)](https://en.wikipedia.org/wiki/MurmurHash) for better performance.
 - [SHA512](https://en.wikipedia.org/wiki/SHA-2) for higher security.
 
 [Read more on wiki](https://github.com/undergroundwires/SafeOrbit/wiki/Hashers).
 

### Random

> What if your OS crypto random has in any way been undermined (for example, by a nefarious government agency, or simple incompetence)?

`SafeOrbit` guarantees not to reduce the strength of your crypto random. It has the ability to improve the strength of your crypto random. [Read more on wiki](https://github.com/undergroundwires/SafeOrbit/wiki/Random).
