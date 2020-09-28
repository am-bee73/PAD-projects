using Grpc.Net.Client;

namespace Broker.Models
{
    public class SubscriberConnection
    {
        // MARK: Properties
        public string Address { get; }

        public string Nickname { get; }

        public string Topic { get; }

        public GrpcChannel Channel { get; }

        // MARK: Initializers
        public SubscriberConnection(string address, string nickname, string topic)
        {
            this.Address = address;
            this.Nickname = nickname;
            this.Topic = topic;
            this.Channel = GrpcChannel.ForAddress(address);
        }
    }
}
