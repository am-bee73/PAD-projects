using Broker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Broker.Services.Interfaces
{
    public interface IMessageStorageService
    {
        void AddMessage(Message message);
        Message GetNextMessage();
        bool isEmpty();
    }
}
