using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.Contracts;
using Serilog;

namespace EasyNetQ.Server
{
    public class BeginDelegationCommandHandler : IHandle<BeginDelegationCommand>
    {
        private readonly IBus _bus;
        private readonly string _instance;

        public BeginDelegationCommandHandler(string instance, IBus bus)
        {
            _bus = bus;
            _instance = instance;
        }

        public async Task Handle(BeginDelegationCommand command, CancellationToken cancellationToken)
        {
            Log.Information($"Received BeginDelegationCommand DelegationId={command.DelegationId}");
            await Task.Delay(1000);
            await _bus.PubSub.PublishAsync(new ResumeDelegationCommand
                {DelegationId = command.DelegationId, Instance = _instance});
            Log.Information($"Sent ResumeDelegationCommand DelegationId={command.DelegationId}");
        }
    }
}