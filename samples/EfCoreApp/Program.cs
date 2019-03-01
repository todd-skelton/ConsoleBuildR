using ConsoleBuildR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace EfCoreApp
{
    class Program
    {
        static Task Main(string[] args) =>
            // create console application with default settings
            ConsoleBuilder.CreateDefaultBuilder()
            // configure dependency injection
            .ConfigureServices(services =>
            {
                // register ef core with dependency injection
                services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("ApplicationDb"));
            })
            // execute the add users class on run
            .Execute<AddUsersFromConfiguration>()
            // execute the log users class on run
            .Execute<LogUsersInDatabase>()
            // build the console app
            .Build()
            // run the console app
            .Run(args);
    }
}
