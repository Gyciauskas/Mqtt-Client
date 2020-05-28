using System;
using System.IO;
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

        public Form1()
        {
            InitializeComponent();

            _timer = new Timer
            {
                AutoReset = true,
                Interval = 5000
            };

            _timer.Elapsed += OnTimedEvent;

            PathBox2.Text = AppDomain.CurrentDomain.BaseDirectory;
        }

        private async void OnSubscriberConnected(MqttClientConnectedEventArgs x)
        {
            if (x.AuthenticateResult.ResultCode != MqttClientConnectResultCode.Success)
            {
                return;
            }

            var item = $"Time: {DateTime.Now} | Connected";

            BeginInvoke((MethodInvoker)delegate { textBox1.Text = item + Environment.NewLine + textBox1.Text; });

            await PublishDeviceIdentityAsync();
        }

        private void OnSubscriberDisconnected(MqttClientDisconnectedEventArgs x)
        {
            var item = $"Time: {DateTime.Now} | Disconnected";

            BeginInvoke((MethodInvoker)delegate { textBox1.Text = item + Environment.NewLine + textBox1.Text; });
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

            WriteLineToTextBox(deserialized);
        }

        private void WriteLineToTextBox(MessageEnvelope deserialized)
        {
            var item = $"Time: {DateTime.Now} | Size: {deserialized.Payload?.Length} | Code : {(MessageCodes) deserialized.Code}";

            BeginInvoke((MethodInvoker) delegate { textBox1.Text = item + Environment.NewLine + textBox1.Text; });
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

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(stream.ToArray())
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
            var eventData = new EventData
            {
                Id = 1,
                ReaderId = string.IsNullOrEmpty(ReaderIdBox.Text) ? (int?) null : int.Parse(ReaderIdBox.Text),
                UserId = string.IsNullOrEmpty(UserIdBox.Text) ? (int?) null : int.Parse(UserIdBox.Text),
                TypeId = (int) eventType,
                Time = DateTime.Now,
                Code = (int)MessageCodes.Event
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
                    Version = "1.0.0.1",
                    IpAddress = "127.0.0.1"
                }).Build();

            await PublishMessageAsync(msg, $"devices/{IdentifierBox.Text}/messages");

            var item = $"Time: {DateTime.Now} | Device identity message published";

            BeginInvoke((MethodInvoker)delegate { textBox1.Text = item + Environment.NewLine + textBox1.Text; });
        }
    }
}
