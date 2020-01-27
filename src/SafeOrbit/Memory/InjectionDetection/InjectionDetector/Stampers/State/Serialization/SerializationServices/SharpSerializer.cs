//This is a modified version of the beautiful SharpSerializer by Pawel Idzikowski (see: http://www.sharpserializer.com)

using System;
using System.IO;
using System.Runtime.CompilerServices;
using SafeOrbit.Memory.InjectionServices.Stampers.Serialization.SerializationServices.Advanced;
using SafeOrbit.Memory.InjectionServices.Stampers.Serialization.SerializationServices.Advanced.Binary;
using SafeOrbit.Memory.InjectionServices.Stampers.Serialization.SerializationServices.Advanced.Deserializing;
using SafeOrbit.Memory.InjectionServices.Stampers.Serialization.SerializationServices.Advanced.Serializing;
using SafeOrbit.Memory.InjectionServices.Stampers.Serialization.SerializationServices.Core;
using SafeOrbit.Memory.InjectionServices.Stampers.Serialization.SerializationServices.Deserializing;
using SafeOrbit.Memory.InjectionServices.Stampers.Serialization.SerializationServices.Serializing;

namespace SafeOrbit.Memory.InjectionServices.Stampers.Serialization.SerializationServices
{
    /// <summary>
    ///     This is the main class of SharpSerializer. It serializes and deserializes objects.
    ///     SafeOrbit SharpSerializer is modified and is not the same as the original.
    /// </summary>
    internal sealed class SharpSerializer
    {
        private IPropertyDeserializer _deserializer;
        private PropertyProvider _propertyProvider;
        private string _rootName;
        private IPropertySerializer _serializer;

        /// <summary>
        ///     Standard Constructor for binary serialization.
        /// </summary>
        public SharpSerializer()
        {
            Initialize(new BinarySettings());
        }

        /// <summary>
        ///     Binary serialization with custom settings
        /// </summary>
        /// <param name="settings"></param>
        public SharpSerializer(BinarySettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            Initialize(settings);
        }

        /// <summary>
        ///     Custom serializer and deserializer
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="deserializer"></param>
        public SharpSerializer(IPropertySerializer serializer, IPropertyDeserializer deserializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        }

        /// <summary>
        ///     Default it is an instance of PropertyProvider. It provides all properties to serialize.
        ///     You can use an Inheritor and overwrite its GetAllProperties and IgnoreProperty methods to implement your custom
        ///     rules.
        /// </summary>
        public PropertyProvider PropertyProvider
        {
            get { return _propertyProvider ??= new PropertyProvider(); }
            set => _propertyProvider = value;
        }

        /// <summary>
        ///     What name should have the root property. Default is "Root".
        /// </summary>
        public string RootName
        {
            get { return _rootName ??= "Root"; }
            set => _rootName = value;
        }

        private void Initialize(BinarySettings settings)
        {
            //RootName
            RootName = settings.AdvancedSettings.RootName;

            // TypeNameConverter)
            var typeNameConverter = settings.AdvancedSettings.TypeNameConverter ??
                                    DefaultInitializer.GetTypeNameConverter(
                                        settings.IncludeAssemblyVersionInTypeName,
                                        settings.IncludeCultureInTypeName,
                                        settings.IncludePublicKeyTokenInTypeName);


            // Create Serializer and Deserializer
            IBinaryReader reader;
            IBinaryWriter writer;
            if (settings.Mode == BinarySerializationMode.Burst)
            {
                // Burst mode
                writer = new BurstBinaryWriter(typeNameConverter, settings.Encoding);
                reader = new BurstBinaryReader(typeNameConverter, settings.Encoding);
            }
            else
            {
                // Size optimized mode
                writer = new SizeOptimizedBinaryWriter(typeNameConverter, settings.Encoding);
                reader = new SizeOptimizedBinaryReader(typeNameConverter, settings.Encoding);
            }

            _deserializer = new BinaryPropertyDeserializer(reader);
            _serializer = new BinaryPropertySerializer(writer);
        }

        #region Serializing/Deserializing methods

#if !NETSTANDARD1_6
        /// <summary>
        ///     Serializing to a file. File will be always new created and closed after the serialization.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filename"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Serialize(object data, string filename)
        {
            CreateDirectoryIfNecessary(filename);
            using Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write);
            Serialize(data, stream);
        }

        private static void CreateDirectoryIfNecessary(string filename)
        {
            var directory = Path.GetDirectoryName(filename);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }
#endif

        /// <summary>
        ///     Serializing to the stream. After serialization the stream will NOT be closed.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="stream"></param>
#if !NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.Synchronized)]
#endif
        public void Serialize(object data, Stream stream)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var factory = new PropertyFactory(PropertyProvider);

            var property = factory.CreateProperty(RootName, data);

            try
            {
                _serializer.Open(stream);
                _serializer.Serialize(property);
            }
            finally
            {
                _serializer.Close();
            }
        }

#if !NETSTANDARD1_6
        /// <summary>
        ///     Deserializing from the file. After deserialization the file will be closed.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public object Deserialize(string filename)
        {
            using var stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            return Deserialize(stream);
        }
#endif

        /// <summary>
        ///     Deserialization from the stream. After deserialization the stream will NOT be closed.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
#if !NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.Synchronized)]
#endif
        public object Deserialize(Stream stream)
        {
            try
            {
                // Deserialize Property
                _deserializer.Open(stream);
                var property = _deserializer.Deserialize();
                _deserializer.Close();

                // create object from Property
                var factory = new ObjectFactory();
                return factory.CreateObject(property);
            }
            catch (Exception exception)
            {
                // corrupted Stream
                throw new DeserializingException(
                    "An error occurred during the deserialization. Details are in the inner exception.", exception);
            }
        }
        #endregion
    }
}