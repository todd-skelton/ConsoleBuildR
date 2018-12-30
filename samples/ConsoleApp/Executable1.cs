using ConsoleBuildR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
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
}
