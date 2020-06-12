using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using MqttClient.Models;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;
using ProtoBuf;
using Timer = System.Timers.Timer;

namespace MqttClient
{
    public partial class Form1 : Form
    {
        private IManagedMqttClient _mqttClient;
        private readonly Timer _timer;
        private readonly Dictionary<string, int> _usersMap = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _readersMap = new Dictionary<string, int>();   

        public Form1()
        {
            InitializeComponent();

            _timer = new Timer
            {
                AutoReset = true,
                Interval = 5000
            };

            _timer.Elapsed += OnTimedEvent;

            PathBox2.Text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConfigurationId.txt");
        }

        private async void OnSubscriberConnected(MqttClientConnectedEventArgs x)
        {
            if (x.AuthenticateResult.ResultCode != MqttClientConnectResultCode.Success)
            {
                return;
            }

            var item = $"Time: {DateTime.Now} | Connected";

            BeginInvoke((MethodInvoker)delegate { textBox1.Text = string.IsNullOrEmpty(textBox1.Text) 
                ? textBox1.Text + item 
                : textBox1.Text + Environment.NewLine + item; });

            await PublishDeviceIdentityAsync();
        }

        private void OnSubscriberDisconnected(MqttClientDisconnectedEventArgs x)
        {
            var item = $"Time: {DateTime.Now} | Disconnected";

            BeginInvoke((MethodInvoker)delegate { textBox1.Text = textBox1.Text + Environment.NewLine + item; });
        }

        private async void OnSubscriberMessageReceived(MqttApplicationMessageReceivedEventArgs x)
        {
            var deserialized = ToMessageWrapper(x.ApplicationMessage.Payload);

            switch ((MessageCodes)deserialized.Code)
            {
                case MessageCodes.OverwriteAccessLevels:
                {
                    var result = DeserializeAccessLevelsCmd(deserialized.Payload);
                }
                    break;
                case MessageCodes.OverwriteSchedules:
                {
                    var result = DeserializeSchedulesCmd(deserialized.Payload);

                    var item = $"Time: {DateTime.Now} | Size: {deserialized.Payload?.Length} | Code : {(MessageCodes)deserialized.Code}";

                    WriteLineToTextBox(item);

                    foreach (var scheduleData in result.SchedulesData)
                    {
                        var message = $"    Schedule name: {scheduleData.Name} | Intervals count: {scheduleData.Intervals?.Length}";

                        WriteLineToTextBox(message);
                    }

                    return;
                }

                case MessageCodes.OverwriteDoors:
                {
                    var result = DeserializeDoorsCmd(deserialized.Payload);

                    var item = $"Time: {DateTime.Now} | Size: {deserialized.Payload?.Length} | Code : {(MessageCodes)deserialized.Code}";

                    WriteLineToTextBox(item);

                    if (result.DoorsData == null) return;

                    var readers = new object[result.DoorsData.First().ReadersData.Length];

                    var counter = 0;

                    foreach (var doorData in result.DoorsData)
                    {
                        foreach (var readerData in doorData.ReadersData)
                        {
                            if (readerData.Side == 0)
                            {
                                _readersMap.Add("Entry reader", readerData.Id);

                                readers[counter] = "Entry reader";
                            }
                            else
                            {
                                _readersMap.Add("Exit reader", readerData.Id);

                                readers[counter] = "Exit reader";
                            }

                            counter++;
                        }
                    }

                    BeginInvoke((MethodInvoker)delegate { comboBox2.Items.AddRange(readers); });

                    return;
                }

                case MessageCodes.AddUser:
                {
                    var result = DeserializeUsersCmd(deserialized.Payload);

                    var item = $"Time: {DateTime.Now} | Size: {deserialized.Payload?.Length} | Code : {(MessageCodes)deserialized.Code}";

                    WriteLineToTextBox(item);

                    return;
                }
                case MessageCodes.OverwriteUsers:
                {
                    var result = DeserializeUsersCmd(deserialized.Payload);

                    var item = $"Time: {DateTime.Now} | Size: {deserialized.Payload?.Length} | Code : {(MessageCodes)deserialized.Code}";

                    WriteLineToTextBox(item);

                    var users = new object[result.Users.Length];
                    var counter = 0;

                    foreach (var userData in result.Users)      
                    {
                        //    var message = $"    User name: {userData.FirstName} | User last name: {userData.LastName} | Identifications count: {userData.Identifications?.Length}";

                        //    WriteLineToTextBox(message);

                        //    message = $"        Company: {userData.CompanyName} | Department: {userData.DepartmentName} | Title: {userData.UserTitleName} | Employee number: {userData.EmployeeNumber}";

                        //    WriteLineToTextBox(message);

                        //    if (userData.Identifications != null && userData.Identifications.Any())
                        //    {
                        //        var identification = userData.Identifications.First();

                        //        message = $"        Card data: {identification.RawCardHex} | Card number: {identification.CardNumber} | Facility code: {identification.FacilityCode} | Pin code: {identification.PinCode}";

                        //        WriteLineToTextBox(message);    
                        //    }

                        users[counter] = $"{userData.FirstName} {userData.LastName}";

                        if (!_usersMap.ContainsKey($"{userData.FirstName} {userData.LastName}"))
                        {
                            _usersMap.Add($"{userData.FirstName} {userData.LastName}", userData.Id);
                        }

                        counter++;
                    }

                    BeginInvoke((MethodInvoker)delegate { comboBox1.Items.AddRange(users); });

                    return;
                }
                case MessageCodes.GetConfigurationId:
                {
                    var configurationId = 0;

                    if (File.Exists(PathBox2.Text))
                    {
                        var result = File.ReadAllText(PathBox2.Text);

                        int.TryParse(result, out configurationId);
                    }

                    var msg = new MessageEnvelopeBuilder()
                        .SetCode(MessageCodes.ConfigurationId)
                        .SetPayload(new ConfigurationId {Id = configurationId})
                        .Build();

                    await PublishMessageAsync(msg, $"devices/{IdentifierBox.Text}/messages");
                }
                    break;
                case MessageCodes.OverwriteConfigurationId:
                {
                    var result = DeserializeConfigurationId(deserialized.Payload);

                    File.WriteAllText(PathBox2.Text, result.ToString());

                    var message = $"    ConfigurationId: {result}";

                    WriteLineToTextBox(message);
                }
                    break;
                case MessageCodes.DeviceIdentifyReject:
                {
                    _timer.Start();
                }
                    break;
                case MessageCodes.DeviceIdentifyAccept:
                {
                    _timer.Stop();
                }
                    break;
            }

            WriteLineToTextBoxMessage(deserialized);
        }

        private void WriteLineToTextBoxMessage(MessageEnvelope deserialized)
        {
            var item = $"Time: {DateTime.Now} | Size: {deserialized.Payload?.Length} | Code : {(MessageCodes) deserialized.Code}";

            WriteLineToTextBox(item);
        }

        private void WriteLineToTextBox(string item)
        {
            BeginInvoke((MethodInvoker) delegate { textBox1.Text = textBox1.Text + Environment.NewLine + item; });
        }

        public MessageEnvelope ToMessageWrapper(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                var wrapper = Serializer.Deserialize<MessageEnvelope>(ms);

                return wrapper;
            }
        }

        private OverwriteAccessLevelsCmd DeserializeAccessLevelsCmd(byte[] bytes)     
        {
            using (var ms = new MemoryStream(bytes))
            {
                var deserialized = Serializer.Deserialize<OverwriteAccessLevelsCmd>(ms);       

                return deserialized;
            }
        }

        private OverwriteSchedulesCmd DeserializeSchedulesCmd(byte[] bytes)      
        {
            using (var ms = new MemoryStream(bytes))
            {
                var deserialized = Serializer.Deserialize<OverwriteSchedulesCmd>(ms);

                return deserialized;
            }
        }

        private OverwriteUsersCmd DeserializeUsersCmd(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                var deserialized = Serializer.Deserialize<OverwriteUsersCmd>(ms);

                return deserialized;
            }
        }

        private OverwriteDoorsCmd DeserializeDoorsCmd(byte[] bytes)     
        {
            using (var ms = new MemoryStream(bytes))
            {
                var deserialized = Serializer.Deserialize<OverwriteDoorsCmd>(ms);

                return deserialized;
            }
        }

        public int DeserializeConfigurationId(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                var deserialized = Serializer.Deserialize<OverwriteConfigurationIdCmd>(ms);

                return deserialized.ConfigurationId;
            }
        }

        public async Task PublishMessageAsync(IMessage msg, string topic)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, msg);

                var payload = stream.ToArray();

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .Build();

                await _mqttClient.PublishAsync(message);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var factory = new MqttFactory();

            _mqttClient = factory.CreateManagedMqttClient();

            _mqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(OnSubscriberConnected);

            _mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnSubscriberDisconnected);

            _mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(OnSubscriberMessageReceived);

            await _mqttClient
                .SubscribeAsync(new TopicFilterBuilder()
                    .WithTopic($"devices/{IdentifierBox.Text}/config")
                    .WithAtMostOnceQoS()
                    .Build());

            var options = new ManagedMqttClientOptionsBuilder()
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId(IdentifierBox.Text)
                    .WithCredentials(UserName.Text, Password.Text)
                    .WithTcpServer(IpAddressBox.Text, int.Parse(ServerPort.Text))
                    .WithCleanSession())
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .Build();

            await _mqttClient.StartAsync(options);
        }

        private async void Disconnect_button_Click(object sender, EventArgs e)
        {
            await _mqttClient.StopAsync();

            _timer.Stop();

            _mqttClient = null;
        }

        private async void AccessGranted_Click(object sender, EventArgs e)
        {
            await AccessEvent(EventType.AccessGranted);
        }

        private async Task AccessEvent(EventType eventType)
        {
            if (!_usersMap.TryGetValue((string)comboBox1.SelectedItem, out var userId))
            {
                return;
            }

            if (!_readersMap.TryGetValue((string)comboBox2.SelectedItem, out var readerId))     
            {
                return;
            }

            var eventData = new EventData
            {
                Id = 1,
                ReaderId = readerId,
                UserId = userId,
                TypeId = (int) eventType,
                Time = DateTime.Now,
                Code = (int)MessageCodes.Event,
                Reason = eventType == EventType.AccessDenied ? (int)EventReason.ExpiredAccessLevel : (int)EventReason.NoReason,
                Description = UserName.Text
            };

            var msg = new MessageEnvelopeBuilder()
                .SetCode(MessageCodes.Event)
                .SetPayload(eventData)
                .Build();

            await PublishMessageAsync(msg, $"devices/{IdentifierBox.Text}/events");
        }

        private async void AccessDenied_Click(object sender, EventArgs e)
        {
            await AccessEvent(EventType.AccessDenied);
        }

        private async void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            await PublishDeviceIdentityAsync();
        }

        private async Task PublishDeviceIdentityAsync()
        {
            var msg = new MessageEnvelopeBuilder()
                .SetCode(MessageCodes.DeviceIdentify)
                .SetPayload(new DeviceIdentify
                {
                    Version = "1.0.0.10",
                    IpAddress = "127.0.0.1"
                }).Build();

            await PublishMessageAsync(msg, $"devices/{IdentifierBox.Text}/messages");

            var item = $"Time: {DateTime.Now} | Device identity message published";

            BeginInvoke((MethodInvoker)delegate { textBox1.Text = textBox1.Text + Environment.NewLine + item; });
        }
    }
}
