using Broker.Models;
using Broker.Services.Interfaces;
using Grpc.Core;
using GrpcAgent;
using System;
using System.Threading.Tasks;

namespace Broker.Services
{
   public class PublisherService : Publisher.PublisherBase
    {
        // MARK: Properties
        private readonly IMessageStorageService _messageStorageService;

        // MARK: Initializers
        public PublisherService(IMessageStorageService messageStorageService)
        {
            this._messageStorageService = messageStorageService;
        }

        // MARK: Methods
        public override Task<PublishReply> PublishMessage(PublishRequest request, ServerCallContext con)
        {
            Console.WriteLine($"Received: {request.Topic} {request.MessageBody}");

            var message = new Message(request.Nickname, request.Topic, request.MessageBody);

            // Store new message
            this._messageStorageService.AddMessage(message);

            return Task.FromResult(new PublishReply()
            {
                IsSuccess = true
            });
        }
    }
}
