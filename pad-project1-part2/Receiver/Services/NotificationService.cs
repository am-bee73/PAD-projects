using Grpc.Core;
using GrpcAgent;
using System;
using System.Threading.Tasks;

namespace Receiver.Services
{
    public class NotificationService: Notifier.NotifierBase
    {
        public override Task<NotifyReply> Notify(NotifyRequest request, ServerCallContext context)
        {
            Console.WriteLine($"");

            return Task.FromResult(new NotifyReply() { IsSuccess = true });
        }
    }
}
