using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Broker.Models
{
    public class SubscriberConnection
    {
        // MARK: Properties
        public string Address { get; }

        public string Nickname { get; }

        public string Topic { get; }

        // MARK: Initializers
        public SubscriberConnection(string address, string nickname, string topic)
        {
            this.Address = address;
            this.Nickname = nickname;
            this.Topic = topic;
        }
    }
}
