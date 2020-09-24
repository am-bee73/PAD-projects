using Grpc.Core;
using GrpcAgent;
using System;
using System.Threading.Tasks;

namespace Broker.Services
{
   public class PublisherService : Publisher.PublisherBase
    {
        public override Task<PublishReply> PublishMessage(PublishRequest request, ServerCallContext con)
        {
            Console.WriteLine($"Received: {request.Topic} {request.Message}");
            return Task.FromResult(new PublishReply()
            {
                IsSuccess = true
            });
        }
    }
}
