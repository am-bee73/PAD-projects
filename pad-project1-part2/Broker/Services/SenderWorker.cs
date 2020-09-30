using Broker.Services.Interfaces;
using Grpc.Core;
using GrpcAgent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Broker.Services
{
    public class SenderWorker : IHostedService
    {
        // MARK: Propeties
        private Timer _timer;
        private const int TimeToWait = 2000;
        private readonly IMessageStorageService _messageStorageService;
        private readonly IConnectionStorageService _connectionStorageService;

        public SenderWorker(IServiceScopeFactory serviceScopeFactory)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                this._messageStorageService = scope.ServiceProvider.GetRequiredService<IMessageStorageService>();
                this._connectionStorageService = scope.ServiceProvider.GetRequiredService<IConnectionStorageService>();
            }
        }

        // MARK: Methods
        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._timer = new Timer(DoSendWork, null, 0, TimeToWait);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this._timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private void DoSendWork(object state)
        {
            while(!this._messageStorageService.isEmpty())
            {
                var message = this._messageStorageService.GetNextMessage();

                if (message != null)
                {
                    var connections = this._connectionStorageService.GetConnections(message.Topic);

                    foreach (var connection in connections)
                    {
                        var client = new Notifier.NotifierClient(connection.Channel);
                        var request = new NotifyRequest() { Nickname = message.Nickname, Content = message.MessageBody };

                        try
                        {
                            var reply = client.Notify(request);
                            Console.WriteLine($"Message delivered to {connection.Nickname}");
                        }
                        catch (RpcException rpcException)
                        {
                            if (rpcException.StatusCode == StatusCode.Internal)
                            {
                                this._connectionStorageService.RemoveConnection(connection.Address);
                            }

                            Console.WriteLine($"Finished with rpcException: {rpcException.Message}");
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine($"Finished with exception: {exception.Message}");
                        }
                    }
                }
            }
        }
    }
}
