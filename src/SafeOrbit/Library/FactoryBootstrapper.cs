using SafeOrbit.Cryptography.Encryption;
using SafeOrbit.Cryptography.Encryption.Kdf;
using SafeOrbit.Cryptography.Encryption.Padding.Factory;
using SafeOrbit.Cryptography.Encryption.Padding.Padders;
using SafeOrbit.Cryptography.Hashers;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Memory;
using SafeOrbit.Memory.InjectionServices.Stampers.Serialization;
using SafeOrbit.Memory.SafeBytesServices;
using SafeOrbit.Memory.SafeBytesServices.Collection;
using SafeOrbit.Memory.SafeBytesServices.DataProtection;
using SafeOrbit.Memory.SafeBytesServices.DataProtection.Protector;
using SafeOrbit.Memory.SafeBytesServices.Factory;
using SafeOrbit.Memory.SafeBytesServices.Id;
using SafeOrbit.Memory.SafeStringServices.Text;

namespace SafeOrbit.Library
{
    public static class FactoryBootstrapper
    {
        public static void Bootstrap(ISafeContainer safeContainer)
        {
            // Converters
            safeContainer.Register(() => safeContainer, LifeTime.Singleton);
            // Hash
            safeContainer.Register<IFastHasher, Murmur32>(() => Murmur32.StaticInstance, LifeTime.Singleton);
            safeContainer.Register<ISafeHasher, Sha512Hasher>(LifeTime.Singleton);
            // Encryption
            safeContainer.Register<IKeyDerivationFunction, Pbkdf2KeyDeriver>();
            safeContainer.Register<IPadderFactory, PadderFactory>(LifeTime.Singleton);
            safeContainer.Register<IPkcs7Padder, Pkcs7Padder>(LifeTime.Singleton);
            safeContainer.Register<IFactory<IPkcs7Padder>, SafeContainerWrapper<IPkcs7Padder>>(LifeTime.Singleton);
            safeContainer.Register<IFastEncryptor, BlowfishEncryptor>(LifeTime.Transient);
            safeContainer.Register<ISafeEncryptor, AesEncryptor>();
            // Memory
            safeContainer.Register<ISerializer, Serializer>(LifeTime.Singleton);
            // SafeObject
            safeContainer.Register(() => SafeObjectFactory.StaticInstance, LifeTime.Singleton);
            // SafeBytes
            safeContainer.Register<ISafeBytes, SafeBytes>();
            safeContainer.Register<ISafeByte, SafeByte>();
            safeContainer.Register<ISafeByteFactory, MemoryCachedSafeByteFactory>(LifeTime.Singleton);
            safeContainer.Register<IByteArrayProtector, MemoryProtector>(LifeTime.Transient);
            safeContainer.Register<IMemoryProtectedBytes, MemoryProtectedBytes>(LifeTime.Transient);
            safeContainer.Register<IByteIdListSerializer<int>, ByteIdListSerializer>(LifeTime.Singleton);
            safeContainer.Register<IByteIdGenerator, HashedByteIdGenerator>(LifeTime.Singleton);
            safeContainer.Register<ISafeByteCollection, EncryptedSafeByteCollection>();
            safeContainer.Register<IFactory<ISafeBytes>, SafeContainerWrapper<ISafeBytes>>();
            safeContainer.Register<IFactory<ISafeByte>, SafeContainerWrapper<ISafeByte>>(LifeTime.Singleton);
            safeContainer.Register<IFactory<ISafeByteCollection>, SafeContainerWrapper<ISafeByteCollection>>();
            // SafeString
            safeContainer.Register<ISafeString, SafeString>();
            safeContainer.Register<IFactory<ISafeString>, SafeContainerWrapper<ISafeString>>();
            safeContainer.Register<ISafeStringToStringMarshaler, SafeStringToStringMarshaler>();
            // Random
            safeContainer.Register<IFastRandom, FastRandom>(LifeTime.Singleton);
            safeContainer.Register<ISafeRandom, SafeRandom>(LifeTime.Singleton);
            // Text
            safeContainer.Register<ITextService, TextService>();
        }
    }
}