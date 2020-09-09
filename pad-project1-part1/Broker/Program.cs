using System;
using Utils;

namespace Broker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Broker");

            BrockerSocket socket = new BrockerSocket();
            socket.Start(Settings.BROKER_IP, Settings.BROKER_PORT);

            Console.ReadLine();
        }
    }
}
