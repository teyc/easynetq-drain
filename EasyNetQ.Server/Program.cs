using System;
using System.Linq;
using EasyNetQ.Contracts;
using EasyNetQ.Events;

namespace EasyNetQ.Server
{
    internal class Program
    {
        
        public static void Main(string[] args)
        {
            
            var connectionConfiguration = new ConnectionConfiguration
            {
                Hosts = {new HostConfiguration {Host = "localhost", Port = 10089}}
            };

            var instance = args.SingleOrDefault() ?? "DefaultInstance";
            
            using (var bus = RabbitHutch.CreateBus(connectionConfiguration, _ => { }))
            {
                var handler = new BeginDelegationCommandHandler(instance, bus);
                var subscription = bus.PubSub.Subscribe<BeginDelegationCommand>(instance, handler.Handle,
                    config => { config.WithPrefetchCount(2); });
            }
        }

    }
}