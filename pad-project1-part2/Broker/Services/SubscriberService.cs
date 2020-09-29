using Broker.Models;
using Broker.Services.Interfaces;
using Grpc.Core;
using GrpcAgent;
using System;
using System.Threading.Tasks;

namespace Broker.Services
{
    public class SubscriberService: Subscriber.SubscriberBase

    {   public readonly IConnectionStorageService _connectionStorage;

        public SubscriberService(IConnectionStorageService connectionStorage)
            {
            _connectionStorage = connectionStorage;
        }
        public override Task<SubscribeReply> Subscribe(SubscribeRequest request, ServerCallContext context)
        {
            Console.WriteLine($"{request.Nickname} is connecting to topic {request.Topic}");

            try
            {
                var conn = new SubscriberConnection(request.Address, request.Nickname, request.Topic);

                // Store Connection
                _connectionStorage.Add(connection);
               
                Console.WriteLine($"New client subscribed: {request.Address} {request.Topic}");

            } 
            catch (Exception e)
            {
                Console.WriteLine($"{request.Nickname} failed to connect to topic {request.Topic}. /n Reason {e.Message}");
            }

            return Task.FromResult(new SubscribeReply()
            {
                IsSuccess = true
            });
        }
    }
}
