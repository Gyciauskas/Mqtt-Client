using System.IO;
using MqttClient.Models;
using ProtoBuf;

namespace MqttClient
{
    public class MessageEnvelopeBuilder
    {
        private int _code;
        private IMessage _payload;

        public MessageEnvelopeBuilder SetCode(int code)
        {
            _code = code;
            return this;
        }

        public MessageEnvelopeBuilder SetCode(MessageCodes code)
        {
            _code = (int)code;
            return this;
        }

        public MessageEnvelopeBuilder SetPayload(IMessage payload)
        {
            _payload = payload;

            return this;
        }

        public MessageEnvelope Build()
        {
            byte[] bytes = null;

            if (_payload != null)
            {
                using (var ms = new MemoryStream())
                {
                    Serializer.Serialize(ms, _payload);
                    bytes = ms.ToArray();
                }
            }

            var msg = new MessageEnvelope
            {
                Code = _code,
                Payload = bytes
            };

            return msg;
        }
    }
}