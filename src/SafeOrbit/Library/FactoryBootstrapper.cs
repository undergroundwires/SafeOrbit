
/*
MIT License

Copyright (c) 2016 Erkin Ekici - undergroundwires@safeorb.it

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using SafeOrbit.Cryptography.Hashers;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Cryptography.Encryption;
using SafeOrbit.Cryptography.Encryption.Kdf;
using SafeOrbit.Infrastructure.Serialization;
using SafeOrbit.Memory;
using SafeOrbit.Memory.SafeBytesServices;
using SafeOrbit.Memory.SafeBytesServices.Collection;
using SafeOrbit.Memory.SafeBytesServices.DataProtection;
using SafeOrbit.Memory.SafeBytesServices.Factory;
using SafeOrbit.Memory.SafeBytesServices.Id;
using SafeOrbit.Text;

namespace SafeOrbit.Library
{
    public class FactoryBootstrapper
    {
        public static void Bootstrap(ISafeContainer safeContainer)
        {
            //Converters
            safeContainer.Register(() => safeContainer, LifeTime.Singleton);
            ;
            //Hash
            safeContainer.Register<IFastHasher, Murmur32>(() => Murmur32.StaticInstance, LifeTime.Singleton);
            safeContainer.Register<ISafeHasher, Sha512Hasher>(LifeTime.Singleton);
            //Encryption
            safeContainer.Register<IKeyDerivationFunction, Pbkdf2KeyDeriver>();
            safeContainer.Register<IFastEncryptor, BlowfishEncryptor>();
            safeContainer.Register<ISafeEncryptor, AesEncryptor>();
            //Memory
            safeContainer.Register<ISerializer, Serializer>(LifeTime.Singleton);
            //SafeObject
            safeContainer.Register(() => SafeObjectFactory.StaticInstance, LifeTime.Singleton);
            //SafeBytes
            safeContainer.Register<ISafeBytes, SafeBytes>();
            safeContainer.Register<ISafeByte, SafeByte>();
            safeContainer.Register<ISafeByteFactory, MemoryCachedSafeByteFactory>(LifeTime.Singleton);
            safeContainer.Register<IByteArrayProtector, MemoryProtector>(LifeTime.Singleton);
            safeContainer.Register<IByteIdGenerator, HashedByteIdGenerator>(LifeTime.Singleton);
            safeContainer.Register<ISafeByteCollection, EncryptedSafeByteCollection>();
            safeContainer.Register<IFactory<ISafeBytes>, SafeContainerWrapper<ISafeBytes>>();
            safeContainer.Register<IFactory<ISafeByte>, SafeContainerWrapper<ISafeByte>>(LifeTime.Singleton);
            safeContainer.Register<IFactory<ISafeByteCollection>, SafeContainerWrapper<ISafeByteCollection>>();
            //SafeString
            safeContainer.Register<ISafeString, SafeString>();
            safeContainer.Register<IFactory<ISafeString>, SafeContainerWrapper<ISafeString>>();
            safeContainer.Register<ISafeStringToStringMarshaler, SafeStringToStringMarshaler>();
            //Random
            safeContainer.Register<IFastRandom, FastRandom>(LifeTime.Singleton);
            safeContainer.Register<ISafeRandom, SafeRandom>(LifeTime.Singleton);
            //Text
            safeContainer.Register<ITextService, TextService>();
        }
    }
}