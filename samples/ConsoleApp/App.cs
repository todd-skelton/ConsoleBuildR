using ConsoleBuildR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class App : IExecutable
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public App(ILogger<App> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task Execute(string[] args)
        {
            _logger.LogInformation(_configuration.GetValue<string>("Message"));

            await Task.Delay(100);
        }
    }
}
