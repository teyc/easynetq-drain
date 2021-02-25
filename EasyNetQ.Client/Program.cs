namespace EasyNetQ.Client
{
    public class Program
    {
        internal static void Main(string[] args)
        {
            Contracts.Program.Main(args,
                (instance, bus) =>
                {
                    var dispatcher = new BeginDelegationCommandDispatcher(bus, 20);
                    dispatcher.Start();

                    return new ResumeDelegationCommandHandler(instance, bus);
                });
        }
    }
}