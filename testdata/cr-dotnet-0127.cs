using System;
using System.Threading;

namespace GracefulShutdownViolation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Application started.");

            // Simulate some work
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"Processing {i + 1}/10...");
                Thread.Sleep(1000);
            }

            Console.WriteLine("Application finished.");
        }
    }
}
