using Common;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;

namespace Broker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Broker");

            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls(EndpointConstants.BrokerAddress)
                .Build()
                .Run();
        }
    }
}
