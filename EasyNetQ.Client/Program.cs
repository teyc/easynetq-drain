using System;
using System.Linq;
using EasyNetQ.Contracts;

namespace EasyNetQ.Client
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            
            var connectionConfiguration = new ConnectionConfiguration
            {
                Hosts = {new HostConfiguration {Host = "localhost", Port = 5672}}
            };

            var instance = args.SingleOrDefault() ?? "DefaultInstance";
            
            using (var bus = RabbitHutch.CreateBus(connectionConfiguration, _ => { }))
            {
                var handler = new ResumeDelegationCommandHandler(instance, bus);
                var subscription = bus.PubSub.SubscribeAsync<ResumeDelegationCommand>(instance, handler.Handle,
                    config => { config.WithPrefetchCount(2); });

                var dispatcher = new BeginDelegationCommandDispatcher(bus, 20);
                dispatcher.Start();
                
                Console.ReadLine();
            }
        }
    }
}