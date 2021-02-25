using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.Contracts;
using Serilog;

namespace EasyNetQ.Client
{
    public class ResumeDelegationCommandHandler : IHandle<ResumeDelegationCommand>
    {
        private static readonly ConcurrentBag<int> DelegationIds = new ConcurrentBag<int>();
        private static readonly ConcurrentDictionary<string, int> Handlers = new ConcurrentDictionary<string, int>();
        private IBus _bus;
        private string _instance;

        public ResumeDelegationCommandHandler(string instance, IBus bus)
        {
            _bus = bus;
            _instance = instance;
            Handlers["alpha"] = 0;
            Handlers["beta"] = 0;
        }

        public async Task Handle(ResumeDelegationCommand command, CancellationToken cancellationToken)
        {
            await Task.Delay(50);
            DelegationIds.Add(command.DelegationId);
            Handlers[command.Instance] += 1;
            Log.Information(
                $"Received {command.Instance} {command.DelegationId} totalReceived={DelegationIds.Count} alpha={Handlers["alpha"]} beta={Handlers["beta"]}");
        }
    }
}