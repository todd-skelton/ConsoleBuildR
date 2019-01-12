using ConsoleBuildR;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await BuildConsoleApplication().RunAsync(args);
        }   

        static IConsole BuildConsoleApplication() =>
            ConsoleBuilder.CreateDefaultBuilder()
            .Execute<App>()
            .Build();
    }
}
