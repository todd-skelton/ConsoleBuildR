using System;
using System.Threading.Tasks;

namespace ConsoleBuildR
{
    /// <summary>
    /// Represents a configured console application.
    /// </summary>
    public interface IConsole : IDisposable
    {
        /// <summary>
        /// The <see cref="IServiceProvider"/> for the service.
        /// </summary>
        IServiceProvider Services { get; }

        /// <summary>
        /// Runs registered executables.
        /// </summary>
        Task Run(string[] args);
    }
}
