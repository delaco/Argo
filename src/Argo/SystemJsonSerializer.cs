using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Argo
{
    public class SystemJsonSerializer : ISerializer
    {
        private JsonSerializerOptions _serializerOptions;

        public SystemJsonSerializer(JsonSerializerOptions serializerOptions = null)
        {
            _serializerOptions = serializerOptions ?? GetDefaultSettings();
        }

        public T Deserialize<T>(byte[] serializedObject)
        {
            return JsonSerializer.Deserialize<T>(serializedObject, _serializerOptions);
        }

        public Task<T> DeserializeAsync<T>(byte[] serializedObject)
        {
            using (var stream = new MemoryStream(serializedObject))
            {
                return JsonSerializer.DeserializeAsync<T>(stream, _serializerOptions).AsTask();
            }
        }

        public byte[] Serialize<T>(T item)
        {
            return JsonSerializer.SerializeToUtf8Bytes(item, _serializerOptions);
        }

        public Task<byte[]> SerializeAsync<T>(T item)
        {
            return Task.FromResult(JsonSerializer.SerializeToUtf8Bytes(item, _serializerOptions));
        }

        private JsonSerializerOptions GetDefaultSettings()
        {
            _serializerOptions = new JsonSerializerOptions()
            {
                IgnoreNullValues = true
            };

            return _serializerOptions;
        }
    }
}