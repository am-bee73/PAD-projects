using Newtonsoft.Json;
using System;
using System.Text;
using Utils;

namespace Receiver
{
    class PayloadHandler
    {
        public static void Handle(byte[] payloadBytes)
        {
            var payloadString = Encoding.UTF8.GetString(payloadBytes);
            var payload = JsonConvert.DeserializeObject<Data>(payloadString);

            Console.WriteLine(payload.Body);
        }
    }
}
