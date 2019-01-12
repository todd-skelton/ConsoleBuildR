using ConsoleBuildR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace EfCoreApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await BuildConsoleApplication().Run(args);
        }

        static IConsole BuildConsoleApplication() =>
            ConsoleBuilder.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("ApplicationDb"));
            })
            .Execute<AddUsersFromConfiguration>()
            .Execute<LogUsersInDatabase>()
            .Build();
    }
}
