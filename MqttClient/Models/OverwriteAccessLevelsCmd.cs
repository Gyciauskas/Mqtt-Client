using ProtoBuf;

namespace MqttClient.Models
{
    [ProtoContract]
    public class OverwriteAccessLevelsCmd : IMessage
    {
        [ProtoMember(1)] public AccessLevelData[] AccessLevelsData { get; set; }
    }
}