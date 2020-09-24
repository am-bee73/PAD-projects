using Common;
using Grpc.Net.Client;
using GrpcAgent;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sender
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Publisher");
            
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            var channel = GrpcChannel.ForAddress(EndpointConstants.BrokerAddress,
                new GrpcChannelOptions { HttpHandler = httpHandler});
            var client = new Publisher.PublisherClient(channel);

            while (true)
            {
                Console.Write("Enter the nickname: ");
                var nickname = Console.ReadLine();

                Console.Write("Enter the topic: ");
                var topic = Console.ReadLine().ToLower();

                Console.Write("Enter the message: ");
                var message = Console.ReadLine();

                var request = new PublishRequest()
                {
                    Nickname = nickname,
                    Topic = topic,
                    MessageBody = message
                };

                try
                {
                    var reply = await client.PublishMessageAsync(request);
                    Console.WriteLine($"Publish Reply: {reply.IsSuccess}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error sending the message: {e.Message}");
                }
            }
        }
    }
}
