using ConsoleBuildR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
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
