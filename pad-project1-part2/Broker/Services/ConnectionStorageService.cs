using Broker.Models;
using Broker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Broker.Services
{
    public class ConnectionStorageService : IConnectionStorageService
    {
        // MARK: Properties
        private readonly List<SubscriberConnection> _connections;
        private readonly object _locker;

        // MARK: Initializers
        public ConnectionStorageService()
        {
            this._connections = new List<SubscriberConnection>();
            this._locker = new object();
        }

        // MARK: Methods
        public void AddConnection(SubscriberConnection conn)
        {
            lock(this._locker)
            {
                this._connections.Add(conn);
            }
        }

        public IList<SubscriberConnection> GetConnections(string topic)
        {
            lock (this._locker)
            {
                var fillteredConnections = this._connections.Where(conn => conn.Topic == topic).ToList();
                return fillteredConnections;
            }
        }

        public void RemoveConnection(string addr)
        {
            lock(this._locker)
            {
                this._connections.RemoveAll(conn => conn.Address == addr);
            }
        }
    }
}
