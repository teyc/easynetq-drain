using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.Contracts;
using EasyNetQ.Internals;

namespace EasyNetQ.Client
{
    public class ResumeDelegationCommandHandler
    {
        private IBus _bus;
        private string _instance;
        private static ConcurrentBag<int> _delegationIds = new ConcurrentBag<int>();

        public ResumeDelegationCommandHandler(string instance, IBus bus)
        {
            _bus = bus;
            _instance = instance;
        }

        public async Task Handle(ResumeDelegationCommand command, CancellationToken cancellationToken)
        {
            await Task.Delay(50);
            _delegationIds.Add(command.DelegationId);
            Log.Information($"Received {command.Instance} {command.DelegationId} totalReceived={_delegationIds.Count()}");
        }
    }
}