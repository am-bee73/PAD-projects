using Common;
using Grpc.Net.Client;
using System.Net.Http;

namespace Broker.Models
{
    public class SubscriberConnection
    {
        // MARK: Properties
        public string Address { get; }

        public string Nickname { get; }

        public string Topic { get; }

        public GrpcChannel Channel { get; set; }

        // MARK: Initializers
        public SubscriberConnection(string address, string nickname, string topic)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            var httpClient = new HttpClient(httpHandler);

            this.Address = address;
            this.Nickname = nickname;
            this.Topic = topic;
            this.Channel = GrpcChannel.ForAddress(EndpointConstants.BrokerAddress, new GrpcChannelOptions { HttpClient = httpClient });
        }
    }
}
