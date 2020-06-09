using ProtoBuf;

namespace MqttClient.Models
{
    [ProtoContract]
    public class UserData
    {
        [ProtoMember(1)] public int Id { get; set; }

        [ProtoMember(2)] public string FirstName { get; set; }

        [ProtoMember(3)] public string LastName { get; set; }

        [ProtoMember(4)] public string CompanyName { get; set; }

        [ProtoMember(5)] public string DepartmentName { get; set; }

        [ProtoMember(6)] public string UserTitleName { get; set; }

        [ProtoMember(7)] public string EmployeeNumber { get; set; }

        [ProtoMember(8)] public IdentificationData[] Identifications { get; set; }

        [ProtoMember(9)] public int[] AccessLevelIds { get; set; }
    }
}