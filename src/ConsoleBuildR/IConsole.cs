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
        /// Runs all executables in the order they were registered.
        /// </summary>
        void Run(string[] args);

        /// <summary>
        /// Runs all executables in parallel.
        /// </summary>
        Task RunAsync(string[] args);
    }
}
