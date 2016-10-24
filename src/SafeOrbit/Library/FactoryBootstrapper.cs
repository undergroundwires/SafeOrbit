using SafeOrbit.Encryption;
using SafeOrbit.Encryption.Kdf;
using SafeOrbit.Hash;
using SafeOrbit.Interfaces;
using SafeOrbit.Memory;
using SafeOrbit.Memory.Common;
using SafeOrbit.Memory.SafeBytesServices;
using SafeOrbit.Memory.SafeBytesServices.Collection;
using SafeOrbit.Memory.SafeBytesServices.DataProtection;
using SafeOrbit.Memory.SafeBytesServices.Factory;
using SafeOrbit.Memory.SafeBytesServices.Id;
using SafeOrbit.Memory.Serialization;
using SafeOrbit.Random;
using SafeOrbit.Text;

namespace SafeOrbit.Library
{
    public class FactoryBootstrapper
    {
        public static void Bootstrap(ISafeContainer safeContainer)
        {
            //Converters
            safeContainer.Register<ISafeContainer>(() => safeContainer, LifeTime.Singleton);;
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
            safeContainer.Register<ISafeObjectFactory>(() => SafeObjectFactory.StaticInstance, LifeTime.Singleton);
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