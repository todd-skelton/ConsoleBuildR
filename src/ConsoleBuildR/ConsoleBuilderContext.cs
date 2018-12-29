using Microsoft.Extensions.Configuration;

namespace ConsoleBuildR
{
    public class ConsoleBuilderContext
    {
        /// <summary>
        /// The <see cref="IConfiguration" /> containing the merged configuration of the application and the <see cref="IConsole" />.
        /// </summary>
        public IConfiguration Configuration { get; set; }
    }
}
