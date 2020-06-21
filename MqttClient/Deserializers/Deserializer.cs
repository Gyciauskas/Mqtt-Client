using ProtoBuf;
using System.IO;

namespace MqttClient.Deserializers
{
    public class Deserializer
    {
        protected T DeserializeObject<T>(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                var deserialized = Serializer.Deserialize<T>(ms);

                return deserialized;
            }
        }
    }
}
