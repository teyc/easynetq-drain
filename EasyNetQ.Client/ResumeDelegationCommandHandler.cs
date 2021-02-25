using System.Threading;
using EasyNetQ.Contracts;

namespace EasyNetQ.Client
{
    public class ResumeDelegationCommandHandler
    {
        private IBus _bus;
        private string _instance;

        public ResumeDelegationCommandHandler(string instance, IBus bus)
        {
            _bus = bus;
            _instance = instance;
        }

        public void Handle(ResumeDelegationCommand command)
        {
            Log.Information($"Received {command.Instance} {command.DelegationId}");
        }
    }
}