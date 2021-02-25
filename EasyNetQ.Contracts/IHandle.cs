using System.Threading;
using System.Threading.Tasks;

namespace EasyNetQ.Contracts
{
    public interface IHandle<in T>
    {
        Task Handle(T command, CancellationToken cancellationToken);
    }
}