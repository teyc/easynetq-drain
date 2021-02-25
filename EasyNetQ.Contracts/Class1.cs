namespace EasyNetQ.Contracts
{
    public class BeginDelegationCommand
    {
        public int DelegationId { get; set; }
    }

    public class ResumeDelegationCommand
    {
        public int DelegationId { get; set; }
        public string Instance { get; set; }
    }
}