using ConsoleBuildR;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await BuildConsoleApplication().Run(args);
        }   

        static IConsole BuildConsoleApplication() =>
            ConsoleBuilder.CreateDefaultBuilder()
            .Execute<App>()
            .Build();
    }
}
