using System;
using System.Text;
using EasyNetQ.Consumer;
using Serilog;

namespace EasyNetQ.Contracts
{
    public class ConsumerErrorStrategy: IConsumerErrorStrategy
    {
        public void Dispose()
        {
            
        }

        public AckStrategy HandleConsumerError(ConsumerExecutionContext context, Exception exception)
        {
            Log.Error(exception, "{Handler} {Body}", context.Handler, Encoding.UTF8.GetString(context.Body));
            return AckStrategies.Ack;
        }

        public AckStrategy HandleConsumerCancelled(ConsumerExecutionContext context)
        {
            Log.Warning("{Handler} {Body}", context.Handler, Encoding.UTF8.GetString(context.Body));
            return AckStrategies.Ack;
        }
    }
}