using System;
using System.Linq;
using System.Threading;
using EasyNetQ.Consumer;
using Serilog;

namespace EasyNetQ.Contracts
{
    public class Program
    {
        public static void Main<T>(string[] args, Func<string, IBus, IHandle<T>> handlerFactory)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().MinimumLevel.Information().CreateLogger();
            
            var countdownEvent = new CountdownEvent(1);
            
            var connectionConfiguration = new ConnectionConfiguration
            {
                Hosts = {new HostConfiguration {Host = "localhost", Port = 5672}}
            };

            var instance = args.SingleOrDefault() ?? "DefaultInstance";

            // IServiceRegister container = new DefaultServiceContainer();
            // container.Register<IConsumerErrorStrategy, ConsumerErrorStrategy>();
            // RabbitHutch.RegisterBus(container);
            using (var bus = RabbitHutch.CreateBus(connectionConfiguration, _ =>
            {
                _.Register<IConsumerErrorStrategy, ConsumerErrorStrategy>();
            }))
            {
                var handler = handlerFactory(instance, bus);
                var subscription = bus.PubSub.Subscribe<T>("", 
                    async (msg, cancellationToken) =>
                    {
                        try
                        {
                            countdownEvent.AddCount(); 
                            Log.Information("CountDown {value}", countdownEvent.CurrentCount);
                            await handler.Handle(msg, cancellationToken);
                        }
                        finally
                        {
                            countdownEvent.Signal();
                            Log.Information("CountDown {value}", countdownEvent.CurrentCount);
                        }
                    },
                    config => { config.WithPrefetchCount(2); });

                AddShutdownHandler(subscription, countdownEvent);

                Console.ReadLine();
                Console.WriteLine("Program terminating...");
            }
        }

        private static void AddShutdownHandler(ISubscriptionResult subscription, CountdownEvent countdownEvent)
        {
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                Log.Warning("Ctrl+C pressed. Ignored.");
                eventArgs.Cancel = true;
            };
            AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) =>
            {
                Log.Warning("ProcessExit");
            };
            AppDomain.CurrentDomain.DomainUnload += (sender, eventArgs) =>
            {
                Log.Warning("DomainUnload");
                subscription.ConsumerCancellation.Dispose();
                countdownEvent.Signal();
                while (!countdownEvent.IsSet) countdownEvent.Wait();
            };
        }
    }
}