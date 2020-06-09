using ProtoBuf;

namespace MqttClient.Models
{
    [ProtoContract]
    public class ScheduleIntervalData
    {
        [ProtoMember(1)] public int WeekDay { get; set; }

        [ProtoMember(2)] public int StartTime { get; set; }

        [ProtoMember(3)] public int EndTime { get; set; }
    }
}