using ProtoBuf;

namespace MqttClient.Models
{
    [ProtoContract]
    public class OverwriteUsersCmd : IMessage
    {
        [ProtoMember(1)] public UserData[] Users { get; set; }
    }
}