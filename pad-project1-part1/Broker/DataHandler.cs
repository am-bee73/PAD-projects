using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using Utils;

namespace Broker
{
    class DataHandler
    {
        public static void Handle(byte[] payloadBytes, ConnectionInfo connectionInfo)
        {
            var payloadString = Encoding.UTF8.GetString(payloadBytes);

           if (payloadString.StartsWith("subscribe#"))
            {
                // Handle subscribe message from subscriber
                connectionInfo.Subject = payloadString.Split("subscribe#").LastOrDefault();

                // Add new connection to storage
                ConnectionsStorage.AddNewConnection(connectionInfo);
            }
           else
            {
                // Handle new message from sender
                Console.WriteLine($"{connectionInfo.Address} -> {payloadString}");

                Data payload = JsonConvert.DeserializeObject<Data>(payloadString);

                // Add message to storage
                DataStorage.AddNewElement(payload);
            }
        }
    }
}
