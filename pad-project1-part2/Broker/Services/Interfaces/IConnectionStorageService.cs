using Broker.Models;
using System.Collections.Generic;

namespace Broker.Services.Interfaces
{
    public interface IConnectionStorageService
    {
        // MARK: Methods
        void AddConnection(SubscriberConnection conn);

        void RemoveConnection(string addr);

        IList<SubscriberConnection> GetConnections(string topic);
    }
}
