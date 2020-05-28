using ProtoBuf;

namespace MqttClient.Models
{
    [ProtoContract]
    public class ReaderScheduleData
    {
        [ProtoMember(1)] public int ReaderId { get; set; }
        [ProtoMember(2)] public int ScheduleId { get; set; }
    }
}