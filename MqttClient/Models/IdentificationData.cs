using ProtoBuf;

namespace MqttClient.Models
{
    [ProtoContract]
    public class IdentificationData
    {
        [ProtoMember(1)] public int Id { get; set; }

        [ProtoMember(2)] public string RawCardHex { get; set; }

        [ProtoMember(3)] public string PinCode { get; set; }

        [ProtoMember(4)] public int UserId { get; set; }

        [ProtoMember(5)] public string CardNumber { get; set; }

        [ProtoMember(6)] public int FacilityCode { get; set; }

        [ProtoMember(7)] public string QrCode { get; set; }
    }
}