using System;
using System.Threading.Tasks;
using Utils;

namespace Broker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Broker Started");

            BrockerSocket socket = new BrockerSocket();
            socket.Start(Settings.BROKER_IP, Settings.BROKER_PORT);

            var worker = new MessageWorker();
            Task.Factory.StartNew(worker.SendMessage, TaskCreationOptions.LongRunning);

            Console.ReadLine();
        }
    }
}
