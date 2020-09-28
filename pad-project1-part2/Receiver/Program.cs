using System;
using System.Net.Http;
using Common;
using Grpc.Net.Client;
using GrpcAgent;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Receiver
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Receiver");

            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            var host = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls(EndpointConstants.SubscriberAddress)
                .Build();

            host.Start();

            Subscribe();

            Console.WriteLine("Press Enter to Exit");
            Console.ReadLine();
        }

        private static void Subscribe()
        {
            var channel = GrpcChannel.ForAddress(EndpointConstants.BrokerAddress);
            var client = new Subscriber.SubscriberClient(channel);

            Console.Write("Enter the nickname: ");
            var nickname = Console.ReadLine();

            Console.Write("Enter the topic: ");
            var topic = Console.ReadLine().ToLower();

            // Get Address

            var request = new SubscribeRequest() { Nickname = nickname, Topic = topic, Address = "" };

            // Subscribe
        }
    }
}
