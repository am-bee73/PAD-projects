using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Broker
{
    class MessageWorker
    {
        // MARK: - Properties
        private const int TIME_TO_SLEEP = 500;
        // MARK: - Initializers

        // MARK: - Methods
        public void SendMessage()
        {
            while(true)
            {
                // Get message
                var data = DataStorage.GetNextElement();

                if (data != null)
                {
                    // If we have a message, get all connections by subject
                    var connections = ConnectionsStorage.GetConnections(data.Subject);

                    // Send message to every connection
                    foreach(var connection in connections)
                    {
                        var dataString = JsonConvert.SerializeObject(data);
                        byte[] dataBytes = Encoding.UTF8.GetBytes(dataString);

                        connection.Socket.Send(dataBytes);
                    }
                }

                Thread.Sleep(TIME_TO_SLEEP);
            }
        }
    }
}
