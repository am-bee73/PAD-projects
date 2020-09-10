using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Broker
{
    static class ConnectionsStorage
    {
        // MARK: Properties
        private static List<ConnectionInfo> _connections;
        private static object _locker;

        // MARK: Initializers
        static ConnectionsStorage()
        {
            _connections = new List<ConnectionInfo>();
            _locker = new object();
        }

        // MARK: Methods
        public static void AddNewConnection(ConnectionInfo connectionInfo)
        {
            lock(_locker)
            {
                _connections.Add(connectionInfo);
            }
        }

        public static void RemoveConnection(string connectionAddress)
        {
            lock(_locker)
            {
                _connections.RemoveAll(conn => conn.Address == connectionAddress);
            }
        }

        public static List<ConnectionInfo> GetConnections(string subject)
        {
            List<ConnectionInfo> connections;

            lock(_locker)
            {
                connections = _connections.Where(conn => conn.Subject == subject).ToList();
            }

            return connections;
        }
    }
}
