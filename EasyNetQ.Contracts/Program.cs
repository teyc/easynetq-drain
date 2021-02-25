using System;
using System.Linq;
using Serilog;

namespace EasyNetQ.Contracts
{
    public class Program
    {
        public static void Main<T>(string[] args, Func<string, IBus, IHandle<T>> handlerFactory)
        {
            Serilog.Log.Logger = new LoggerConfiguration().WriteTo.Console().MinimumLevel.Information().CreateLogger();

            var connectionConfiguration = new ConnectionConfiguration
            {
                Hosts = {new HostConfiguration {Host = "localhost", Port = 5672}}
            };

            var instance = args.SingleOrDefault() ?? "DefaultInstance";

            using (var bus = RabbitHutch.CreateBus(connectionConfiguration, _ => { }))
            {
                var handler = handlerFactory(instance, bus);
                var subscription = bus.PubSub.Subscribe<T>("", handler.Handle,
                    config => { config.WithPrefetchCount(2); });

                Console.CancelKeyPress += (sender, eventArgs) => { };
                AppDomain.CurrentDomain.DomainUnload += (sender, eventArgs) =>
                {
                    subscription.ConsumerCancellation.Dispose();
                };

                Console.ReadLine();
            }
        }
    }
}