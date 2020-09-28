using Grpc.Core;
using GrpcAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Receiver.Services
{
    public class NotificationService: Notifier.NotifierBase
    {
        // MARK: Methods
        public override Task<NotifyReply> Notify(NotifyRequest request, ServerCallContext context)
        {
            Console.WriteLine($"{request.Nickname} said: {request.Content}");

            return Task.FromResult(
                new NotifyReply()
                {
                    IsSuccess = true
                });
        }
    }
}
