using ConsoleBuildR;
using System;
using System.Threading;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = BuildConsoleApplication();

            Console.WriteLine("Running executables in order.");
            app.Run(args);

            Thread.Sleep(200);

            Console.WriteLine("Running executables in parallel.");
            app.RunAsync(args).GetAwaiter().GetResult();

            Thread.Sleep(200);
        }   

        static IConsole BuildConsoleApplication() =>
            ConsoleBuilder.CreateDefaultBuilder()
            .Execute<Executable1>()
            .Execute<Executable2>()
            .Build();
    }
}
