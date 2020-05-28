namespace MqttClient.Models
{
    public enum MessageCodes
    {
        DeviceIdentify,
        DeviceIdentifyAccept,
        DeviceIdentifyReject,
        AddUser,
        OverwriteUsers,
        DeleteUser,
        OverwriteAccessLevels,
        OverwriteDoors,
        Event,
        OverwriteSchedules,
        OverwriteUserImage,
        AddUserImage,
        DeleteUserImage,
        GetConfigurationId,
        OverwriteConfigurationId,
        ConfigurationId
    }
}