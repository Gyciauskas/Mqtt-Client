using ProtoBuf;

namespace MqttClient.Models
{
    [ProtoContract]
    public class ConfigurationId : IMessage
    {
        [ProtoMember(1)] public int Id { get; set; }
    }
}