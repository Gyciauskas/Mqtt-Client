using ProtoBuf;

namespace MqttClient.Models
{
    [ProtoContract]
    public class DoorData
    {
        [ProtoMember(1)] public int Id { get; set; }
        [ProtoMember(2)] public string Name { get; set; }
        [ProtoMember(3)] public ReaderData[] ReadersData { get; set; }
    }

    [ProtoContract]
    public class ReaderData
    {
        [ProtoMember(1)] public int Id { get; set; }

        /// <summary>
        /// 0 - Entry
        /// 1 - Exit
        /// </summary>
        [ProtoMember(2)] public byte Side { get; set; }
    }
}