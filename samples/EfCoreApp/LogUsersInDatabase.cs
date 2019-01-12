using ConsoleBuildR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace EfCoreApp
{
    public class LogUsersInDatabase : IExecutable
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _applicationDbContext;

        public LogUsersInDatabase(ILogger<LogUsersInDatabase> logger, ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
        }

        public async Task Execute(string[] args)
        {
            foreach(var user in _applicationDbContext.Users)
            {
                _logger.LogInformation($"Username: {user.Username}\n      Full Name:{user.FullName}");
            }

            await Task.Delay(100);
        }
    }
}
