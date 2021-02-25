using System.Threading;
using EasyNetQ.Contracts;

namespace EasyNetQ.Server
{
    public class BeginDelegationCommandHandler
    {
        private IBus _bus;
        private string _instance;

        public BeginDelegationCommandHandler(string instance, IBus bus)
        {
            _bus = bus;
            _instance = instance;
        }

        public void Handle(BeginDelegationCommand command)
        {
            Thread.Sleep(4000);
            _bus.PubSub.Publish(new ResumeDelegationCommand() { DelegationId = command.DelegationId, Instance = _instance});
        }
    }
}