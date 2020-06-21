using MqttClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttClient.Deserializers
{
    public class DoorsDeserializer : Deserializer
    {
        public DoorsDeserializer()
        {

        }

        public string Deserialize(byte[] payload)
        {
            DeserializeObject<OverwriteDoorsCmd>(payload);

            return "";
        }
    }
}
