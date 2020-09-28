using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Receiver
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls(EndpointConstants.SubscriberAddress)
                .Build();

            host.Start();

            Subscribe();

            Console.WriteLine("Press Enter to Exit");
            Console.ReadLine();
        }

        private static void Subscribe()
        {

        }
    }
}
