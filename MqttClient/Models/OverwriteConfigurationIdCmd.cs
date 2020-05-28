using ProtoBuf;

namespace MqttClient.Models
{
    [ProtoContract]
    public class OverwriteConfigurationIdCmd : IMessage
    {
        [ProtoMember(1)] public int ConfigurationId { get; set; }
    }
}