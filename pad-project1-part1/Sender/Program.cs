using Newtonsoft.Json;
using System;
using System.Text;
using Utils;

namespace Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Sender");

            var senderSocket = new SenderSocket();

            senderSocket.Connect(Settings.BROKER_IP, Settings.BROKER_PORT);

            if(senderSocket.IsConnected)
            {
                while(true)
                {
                    var data = new Data();
                    Console.Write("Enter subject: ");
                    data.Subject = Console.ReadLine().ToLower();

                    Console.Write("Enter body of message: ");
                    data.Body = Console.ReadLine();

                    var serializedData = JsonConvert.SerializeObject(data);

                    byte[] buff = Encoding.UTF8.GetBytes(serializedData);

                    senderSocket.Send(buff);
                }
            }

            Console.ReadLine();
        }
    }
}
