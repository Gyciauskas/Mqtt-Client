using ProtoBuf;

namespace MqttClient.Models
{
    [ProtoContract]
    public class MessageEnvelope : IMessage
    {
        [ProtoMember(1)] public int Code { get; set; }
        [ProtoMember(2)] public byte[] Payload { get; set; }
    }
}