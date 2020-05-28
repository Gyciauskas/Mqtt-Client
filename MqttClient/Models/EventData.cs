using System;
using ProtoBuf;

namespace MqttClient.Models
{
    [ProtoContract]
    public class EventData : IMessage
    {
        [ProtoIgnore] public int Id { get; set; }

        [ProtoMember(1)] public int TypeId { get; set; }

        [ProtoMember(2)] public int? UserId { get; set; }
        [ProtoMember(3)] public int? ReaderId { get; set; }

        [ProtoMember(4)] public int Code { get; set; }

        [ProtoMember(5)] public int Status { get; set; }

        [ProtoMember(6)] public DateTime Time { get; set; }
    }
}
