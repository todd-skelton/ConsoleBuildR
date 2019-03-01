using ConsoleBuildR;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static Task Main(string[] args) =>
            // create console application with default settings
            ConsoleBuilder.CreateDefaultBuilder()
            // execute the app class on run
            .Execute<App>()
            // build the console app
            .Build()
            // run the app
            .Run(args);
    }
}
