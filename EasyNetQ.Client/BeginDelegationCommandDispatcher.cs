using System.Threading;
using EasyNetQ.Contracts;

namespace EasyNetQ.Client
{
    public class BeginDelegationCommandDispatcher
    {
        private readonly IBus _bus;
        private readonly int _n;

        public BeginDelegationCommandDispatcher(IBus bus, int n)
        {
            _bus = bus;
            _n = n;
        }

        public void Start()
        {
            new Thread(Dispatch).Start(_n);
        }

        private void Dispatch(object n)
        {
            for (var i = 0; i < (int) n; i++) _bus.PubSub.PublishAsync(new BeginDelegationCommand {DelegationId = i});
        }
    }
}