using ConsoleBuildR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

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

    public class Executable1 : IExecutable
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public Executable1(ILogger<Executable1> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public Task Execute(string[] args)
        {
            Thread.Sleep(100);

            _logger.LogInformation(_configuration.GetValue<string>("Message"));

            return Task.CompletedTask;
        }
    }

    public class Executable2 : IExecutable
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public Executable2(ILogger<Executable2> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public Task Execute(string[] args)
        {
            Thread.Sleep(50);

            _logger.LogInformation(_configuration.GetValue<string>("Message"));

            return Task.CompletedTask;
        }
    }
}
