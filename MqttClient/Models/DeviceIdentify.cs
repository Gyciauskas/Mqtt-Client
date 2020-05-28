using ProtoBuf;

namespace MqttClient.Models
{
    [ProtoContract]
    public class DeviceIdentify : IMessage
    {
        [ProtoMember(1)] public string IpAddress { get; set; }
        [ProtoMember(2)] public string Version { get; set; }
        [ProtoMember(3)] public int Code { get; set; }
    }
}