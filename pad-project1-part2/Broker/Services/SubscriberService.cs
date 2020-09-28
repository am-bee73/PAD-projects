using Broker.Models;
using Grpc.Core;
using GrpcAgent;
using System.Threading.Tasks;

namespace Broker.Services
{
    public class SubscriberService: Subscriber.SubscriberBase
    {
        public override Task<SubscribeReply> Subscribe(SubscribeRequest request, ServerCallContext context)
        {
            var conn = new SubscriberConnection(request.Address, request.Nickname, request.Topic);

            // Store Connection

            return Task.FromResult(new SubscribeReply()
            {
                IsSuccess = true
            });
        }
    }
}
