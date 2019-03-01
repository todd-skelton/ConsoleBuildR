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

        public Task Execute(string[] args)
        {
            // get message from appsettings
            _logger.LogInformation(_configuration.GetValue<string>("Message"));

            // return delay to allow enough time for the console to log the value
            return Task.Delay(100);
        }
    }
}
