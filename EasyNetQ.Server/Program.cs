namespace EasyNetQ.Server
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Contracts.Program.Main(args,
                (instance, bus) => { return new BeginDelegationCommandHandler(instance, bus); });
        }
    }
}