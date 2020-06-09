using ProtoBuf;

namespace MqttClient.Models
{
    [ProtoContract]
    public class ScheduleData
    {
        [ProtoMember(1)] public int Id { get; set; }
        [ProtoMember(2)] public string Name { get; set; }
        [ProtoMember(3)] public ScheduleIntervalData[] Intervals { get; set; }
    }
}