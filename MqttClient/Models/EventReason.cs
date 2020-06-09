namespace MqttClient.Models
{
    public enum EventReason
    {
        NoReason = 0,
        NoAccess,
        NotInSchedule,
        InvalidCredential,
        IdentificationNotFound,
        UnknownUser,
        NoAccessLevel,
        ExpiredAccessLevel,
        // Input alarm reason
        GroundFault,
        Short,
        OpenCircuit,
        ForeignVoltage,
        Masked,
        RejectedBeforeActivation,
        RejectedAfterExpiration,
        NeverAllowed,
        OperatorAction
    }
}