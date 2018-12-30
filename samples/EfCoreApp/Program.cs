using ConsoleBuildR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;

namespace EfCoreApp
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildConsoleApplication().Run(args);
        }

        static IConsole BuildConsoleApplication() =>
            ConsoleBuilder.CreateDefaultBuilder()
            .ConfigureServices((config, services) =>
            {
                services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("ApplicationDb"));
            })
            .Execute<AddUsersFromConfiguration>()
            .Execute<LogUsersInDatabase>()
            .Build();
    }
}
