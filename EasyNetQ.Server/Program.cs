using System;
using System.Linq;
using EasyNetQ.Contracts;
using Serilog;
using Log = Serilog.Log;

namespace EasyNetQ.Server
{
    internal class Program
    {
        
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().
                MinimumLevel.Information().
                CreateLogger();
            
            var connectionConfiguration = new ConnectionConfiguration
            {
                Hosts = {new HostConfiguration {Host = "localhost", Port = 5672}}
            };

            var instance = args.SingleOrDefault() ?? "DefaultInstance";
            
            using (var bus = RabbitHutch.CreateBus(connectionConfiguration, _ => { }))
            {
                var handler = new BeginDelegationCommandHandler(instance, bus);
                var subscription = bus.PubSub.Subscribe<BeginDelegationCommand>("", handler.Handle,
                    config => { config.WithPrefetchCount(2); });
                
                Console.ReadLine();
            }
        }

    }
}