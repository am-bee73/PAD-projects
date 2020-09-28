using Broker.Models;
using Broker.Services.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Broker.Services
{
    public class MessageStorageService : IMessageStorageService
    {

        // MARK: Properties
        private readonly ConcurrentQueue<Message> _messages;

        // MARK: Initializers
        public MessageStorageService()
        {
            this._messages = new ConcurrentQueue<Message>();
        }

        // MARK: Methods
        public void AddMessage(Message message)
        {
            this._messages.Enqueue(message);
        }

        public Message GetNextMessage()
        {
            Message message;
            this._messages.TryDequeue(out message);

            return message;
        }

        public bool isEmpty()
        {
            return this._messages.IsEmpty;
        }
    }
}
