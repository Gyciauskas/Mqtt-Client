using System;
using System.Collections.Concurrent;
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
    public class User
    {
        public int UserId { get; set; }
        public IList<string> AccessLevels { get; set; }
    }

    public class Interval
    {
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public int Id { get; set; }     
    }

    public partial class Form1 : Form
    {
        private readonly Interval[] _accessIntervals =      
        {
            // Explanation -> TimeSpan(int hours, int minutes, int seconds)
            new Interval {Start = new TimeSpan(16, 55, 0), End = new TimeSpan(17, 00, 0), Id = 4}, // Breakfast
            new Interval {Start = new TimeSpan(17, 00, 0), End = new TimeSpan(17, 05, 0), Id = 3}, // Lunch
            new Interval {Start = new TimeSpan(17, 05, 0), End = new TimeSpan(17, 10, 0), Id = 5} // Dinner
        };            

        private IManagedMqttClient _mqttClient;
        private readonly Timer _timer;
        private readonly Timer _accessGrantedTimer;
        private readonly ConcurrentDictionary<string, int> _usersMap = new ConcurrentDictionary<string, int>();
        private readonly Dictionary<string, int> _readersMap = new Dictionary<string, int>();

        private readonly ConcurrentDictionary<int, IEnumerable<int>> _userAccessLevels = new ConcurrentDictionary<int, IEnumerable<int>>();

        public readonly TimeSpan AccessLevelsAssignTime = new TimeSpan(12, 19, 0);

        public Form1()
        {
            InitializeComponent();

            _timer = new Timer
            {
                AutoReset = true,
                Interval = 5000
            };

            _timer.Elapsed += OnTimedEvent;

            _accessGrantedTimer = new Timer(2000)
            {
                AutoReset = true
            };

            _accessGrantedTimer.Elapsed += OnAccessGrantedEvent;

            PathBox2.Text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConfigurationId.txt");
        }

        private async void OnSubscriberConnected(MqttClientConnectedEventArgs x)
        {
            if (x.AuthenticateResult.ResultCode != MqttClientConnectResultCode.Success)
            {
                return;
            }

            var item = $"Time: {DateTime.Now} | Connected";

            BeginInvoke((MethodInvoker)delegate {textBox1.Text = string.IsNullOrEmpty(textBox1.Text)
                            ? textBox1.Text + item
                            : textBox1.Text + Environment.NewLine + item;});

            await PublishDeviceIdentityAsync();
        }

        private void OnSubscriberDisconnected(MqttClientDisconnectedEventArgs x)
        {
            var item = $"Time: {DateTime.Now} | Disconnected";

            BeginInvoke((MethodInvoker)delegate {
                textBox1.Text = string.IsNullOrEmpty(textBox1.Text)
                    ? textBox1.Text + item
                    : textBox1.Text + Environment.NewLine + item;});
        }

        private async void OnSubscriberMessageReceived(MqttApplicationMessageReceivedEventArgs x)
        {
            var deserialized = ToMessageWrapper(x.ApplicationMessage.Payload);

            switch ((MessageCodes)deserialized.Code)
            {
                case MessageCodes.OverwriteAccessLevels:
                    {
                        var result = DeserializeObject<OverwriteAccessLevelsCmd>(deserialized.Payload);
                    }
                    break;
                case MessageCodes.OverwriteSchedules:
                    {
                        var result = DeserializeObject<OverwriteSchedulesCmd>(deserialized.Payload);

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
                        var result = DeserializeObject<OverwriteDoorsCmd>(deserialized.Payload);

                        var item = $"Time: {DateTime.Now} | Size: {deserialized.Payload?.Length} | Code : {(MessageCodes)deserialized.Code}";

                        WriteLineToTextBox(item);

                        if (result.DoorsData == null) return;

                        var readers = new object[result.DoorsData.First().ReadersData.Length];

                        var counter = 0;
                        _readersMap.Clear();

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
                        var result = DeserializeObject<OverwriteUsersCmd>(deserialized.Payload);

                        foreach (var user in result.Users)
                        {
                            if (_userAccessLevels.TryRemove(user.Id, out var removedAls))
                            {
                                var added = _userAccessLevels.TryAdd(user.Id, user.AccessLevelIds);

                                var als = user.AccessLevelIds != null
                                    ? string.Join(";", user.AccessLevelIds)
                                    : string.Empty; 

                                var item = $"Time: {DateTime.Now} | Updated: {added} | User : {user.FirstName} {user.LastName} | Removed als : {string.Join(";", removedAls)} | Als : {als}";

                                WriteLineToTextBox(item);
                            }
                        }

                        return;
                    }
                case MessageCodes.OverwriteUsers:
                    {
                        var result = DeserializeObject<OverwriteUsersCmd>(deserialized.Payload);

                        // If no users return
                        if (result.Users == null) return;

                        var item = $"Time: {DateTime.Now} | Size: {deserialized.Payload?.Length} | Code : {(MessageCodes)deserialized.Code}";

                        WriteLineToTextBox(item);

                        var users = new List<object>();

                        _usersMap.Clear();

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

                            if (userData.AccessLevelIds is null || userData.AccessLevelIds.Any() is false) continue;

                            users.Add($"{userData.FirstName} {userData.LastName}");

                            if (_usersMap.ContainsKey($"{userData.FirstName} {userData.LastName}") is false)
                            {
                                _usersMap.TryAdd($"{userData.FirstName} {userData.LastName}", userData.Id);

                                //if (_userAccessLevels.TryGetValue(userData.Id, out var als))
                                //{

                                //}
                                //else
                                //{
                                //    _userAccessLevels.TryAdd(userData.Id, userData.AccessLevelIds);
                                //}

                                _userAccessLevels.TryAdd(userData.Id, userData.AccessLevelIds);
                            }
                        }

                        BeginInvoke((MethodInvoker)delegate { comboBox1.Items.AddRange(users.ToArray()); });

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
                            .SetPayload(new ConfigurationId { Id = configurationId })
                            .Build();

                        await PublishMessageAsync(msg, $"devices/{IdentifierBox.Text}/messages");
                    }
                    break;
                case MessageCodes.OverwriteConfigurationId:
                    {
                        var result = DeserializeObject<OverwriteConfigurationIdCmd>(deserialized.Payload);

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
            var item = $"Time: {DateTime.Now} | Size: {deserialized.Payload?.Length} | Code : {(MessageCodes)deserialized.Code}";

            WriteLineToTextBox(item);
        }

        private void WriteLineToTextBox(string item)
        {
            BeginInvoke((MethodInvoker)delegate { textBox1.Text = textBox1.Text + Environment.NewLine + item; });
        }

        public MessageEnvelope ToMessageWrapper(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                var wrapper = Serializer.Deserialize<MessageEnvelope>(ms);

                return wrapper;
            }
        }

        private static T DeserializeObject<T>(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                var deserialized = Serializer.Deserialize<T>(ms);

                return deserialized;
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
                    .WithKeepAlivePeriod(TimeSpan.FromSeconds(5))
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

            _accessGrantedTimer.Stop();

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
                TypeId = (int)eventType,
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

        private async void OnAccessGrantedEvent(Object source, ElapsedEventArgs e)
        {
            if (_readersMap.Any() is false) return;

            if (_usersMap.Any() is false) return;

            for (int i = 0; i < 5; i++)     
            {
                var random = new Random();

                var userId = random.Next(_usersMap.Values.Min(), _usersMap.Values.Max());

                if (_userAccessLevels.TryGetValue(userId, out var als))
                {
                    var interval = _accessIntervals.FirstOrDefault(a => a.End >= DateTime.Now.TimeOfDay && a.Start <= DateTime.Now.TimeOfDay);

                    if (interval != null && als != null)
                    {
                        if (als.Contains(interval.Id) is false)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                // If not have als, skip
                else
                {
                    continue;
                }

                var eventData = new EventData
                {
                    Id = 1,
                    ReaderId = _readersMap.Values.First(),
                    UserId = userId,
                    TypeId = (int)EventType.AccessGranted,
                    Time = DateTime.Now,
                    Code = (int)MessageCodes.Event,
                    Reason = (int)EventReason.NoReason,
                    Description = UserName.Text
                };

                var msg = new MessageEnvelopeBuilder()
                    .SetCode(MessageCodes.Event)
                    .SetPayload(eventData)
                    .Build();

                await PublishMessageAsync(msg, $"devices/{IdentifierBox.Text}/events");

                await Task.Delay(50);
            }
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

        // start random AG
        private void button1_Click_1(object sender, EventArgs e)
        {
            _accessGrantedTimer.Start();
        }

        // stop random AG
        private void button2_Click(object sender, EventArgs e)
        {
            _accessGrantedTimer.Stop();
        }
    }
}
