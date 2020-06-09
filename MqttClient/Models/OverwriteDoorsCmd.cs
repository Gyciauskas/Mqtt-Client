using ProtoBuf;

namespace MqttClient.Models
{
    [ProtoContract]
    public class OverwriteDoorsCmd : IMessage
    {
        [ProtoMember(1)] public DoorData[] DoorsData { get; set; }
    }
}