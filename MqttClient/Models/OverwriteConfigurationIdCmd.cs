using ProtoBuf;

namespace MqttClient.Models
{
    [ProtoContract]
    public class OverwriteConfigurationIdCmd : IMessage
    {
        [ProtoMember(2)] public int ConfigurationId { get; set; }
    }
}