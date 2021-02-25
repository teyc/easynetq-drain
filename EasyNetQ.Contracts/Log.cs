using System;

namespace EasyNetQ.Contracts
{
    public class Log
    {
        public static void Information(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }
    }
}