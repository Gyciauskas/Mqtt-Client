using ProtoBuf;

namespace MqttClient.Models
{
    [ProtoContract]
    public class OverwriteSchedulesCmd : IMessage
    {
        [ProtoMember(1)] public ScheduleData[] SchedulesData { get; set; }
    }
}