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
            Log.Logger = new LoggerConfiguration().WriteTo.Console().MinimumLevel.Debug().CreateLogger();
            
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
                Log.CloseAndFlush();
            }
        }

        private static void AddShutdownHandler(ISubscriptionResult subscription, CountdownEvent countdownEvent)
        {
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                Log.Warning("Ctrl+C pressed. Ignored.");
                eventArgs.Cancel = true;
            };
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnDomainUnload(subscription, countdownEvent, "ProcessExit");
            AppDomain.CurrentDomain.DomainUnload += CurrentDomainOnDomainUnload(subscription, countdownEvent, "DomainUnload");
        }

        private static EventHandler CurrentDomainOnDomainUnload(ISubscriptionResult subscription, CountdownEvent countdownEvent, string eventName)
        {
            return (sender, eventArgs) =>
            {
                Log.Warning(eventName);
                subscription.ConsumerCancellation.Dispose();
                countdownEvent.Signal();
                while (!countdownEvent.IsSet) countdownEvent.Wait();
                Thread.Sleep(2000);
            };
        }
    }
}