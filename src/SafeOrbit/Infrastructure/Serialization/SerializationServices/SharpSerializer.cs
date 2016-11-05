
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

//This is a modified version of the beautiful SharpSerializer by Pawel Idzikowski (see: http://www.sharpserializer.com)

using System;
using System.IO;
using System.Runtime.CompilerServices;
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Advanced;
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Advanced.Binary;
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Advanced.Deserializing;
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Advanced.Serializing;
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Core;
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Deserializing;
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Serializing;
using SafeOrbit.Memory.SafeObject.SharpSerializer;

namespace SafeOrbit.Infrastructure.Serialization.SerializationServices
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
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));
            if (deserializer == null) throw new ArgumentNullException(nameof(deserializer));
            _serializer = serializer;
            _deserializer = deserializer;
        }

        /// <summary>
        ///     Default it is an instance of PropertyProvider. It provides all properties to serialize.
        ///     You can use an Inheritor and overwrite its GetAllProperties and IgnoreProperty methods to implement your custom
        ///     rules.
        /// </summary>
        public PropertyProvider PropertyProvider
        {
            get
            {
                if (_propertyProvider == null) _propertyProvider = new PropertyProvider();
                return _propertyProvider;
            }
            set { _propertyProvider = value; }
        }

        /// <summary>
        ///     What name should have the root property. Default is "Root".
        /// </summary>
        public string RootName
        {
            get
            {
                if (_rootName == null) _rootName = "Root";
                return _rootName;
            }
            set { _rootName = value; }
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
            IBinaryReader reader = null;
            IBinaryWriter writer = null;
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

#if !NETCORE
        /// <summary>
        ///     Serializing to a file. File will be always new created and closed after the serialization.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filename"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Serialize(object data, string filename)
        {
            CreateDirectoryIfNeccessary(filename);
            using (Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                Serialize(data, stream);
            }
        }

        private void CreateDirectoryIfNeccessary(string filename)
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
#if !NETCORE
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

#if !NETCORE
        /// <summary>
        ///     Deserializing from the file. After deserialization the file will be closed.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public object Deserialize(string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                return Deserialize(stream);
            }
        }
#endif

        /// <summary>
        ///     Deserialization from the stream. After deserialization the stream will NOT be closed.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
#if !NETCORE
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