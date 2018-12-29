using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;

namespace ConsoleBuildR
{
    public static class ConsoleBuilderExtensions
    {
        /// <summary>
        /// Run this <see cref="IExecutable"/> when the console application starts.
        /// </summary>
        /// <typeparam name="TExecutable">The type of executable.</typeparam>
        /// <param name="applicationBuilder">The <see cref="IConsoleBuilder"/>.</param>
        /// <returns>The <see cref="IConsole"/>.</returns>
        /// <remarks>This can be called multiple times.</remarks>
        public static IConsoleBuilder Execute<TExecutable>(this IConsoleBuilder applicationBuilder)
            where TExecutable : class, IExecutable
        {
            return applicationBuilder.ConfigureServices(services => services.TryAddEnumerable(ServiceDescriptor.Singleton<IExecutable, TExecutable>()));
        }

        /// <summary>
        /// Configures the default service provider
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IConsoleBuilder"/> to configure.</param>
        /// <param name="configure">A callback used to configure the <see cref="ServiceProviderOptions"/> for the default <see cref="IServiceProvider"/>.</param>
        /// <returns>The <see cref="IConsoleBuilder"/>.</returns>
        public static IConsoleBuilder UseDefaultServiceProvider(this IConsoleBuilder applicationBuilder, Action<ServiceProviderOptions> configure)
        {
            return applicationBuilder.UseDefaultServiceProvider((context, options) => configure(options));
        }

        /// <summary>
        /// Configures the default service provider
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IConsoleBuilder"/> to configure.</param>
        /// <param name="configure">A callback used to configure the <see cref="ServiceProviderOptions"/> for the default <see cref="IServiceProvider"/>.</param>
        /// <returns>The <see cref="IConsoleBuilder"/>.</returns>
        public static IConsoleBuilder UseDefaultServiceProvider(this IConsoleBuilder applicationBuilder, Action<ConsoleBuilderContext, ServiceProviderOptions> configure)
        {
            return applicationBuilder.ConfigureServices((context, services) =>
            {
                var options = new ServiceProviderOptions();
                configure(context, options);
                services.Replace(ServiceDescriptor.Singleton<IServiceProviderFactory<IServiceCollection>>(new DefaultServiceProviderFactory(options)));
            });
        }

        /// <summary>
        /// Adds a delegate for configuring the <see cref="IConfigurationBuilder"/> that will construct an <see cref="IConfiguration"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IConsoleBuilder"/> to configure.</param>
        /// <param name="configureDelegate">The delegate for configuring the <see cref="IConfigurationBuilder" /> that will be used to construct an <see cref="IConfiguration" />.</param>
        /// <returns>The <see cref="IConsoleBuilder"/>.</returns>
        /// <remarks>
        /// The <see cref="IConfiguration"/> and <see cref="ILoggerFactory"/> on the <see cref="ConsoleBuilderContext"/> are uninitialized at this stage.
        /// The <see cref="IConfigurationBuilder"/> is pre-populated with the settings of the <see cref="IConsoleBuilder"/>.
        /// </remarks>
        public static IConsoleBuilder ConfigureAppConfiguration(this IConsoleBuilder applicationBuilder, Action<IConfigurationBuilder> configureDelegate)
        {
            return applicationBuilder.ConfigureAppConfiguration((context, builder) => configureDelegate(builder));
        }

        /// <summary>
        /// Adds a delegate for configuring the provided <see cref="LoggerFactory"/>. This may be called multiple times.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IConsoleBuilder" /> to configure.</param>
        /// <param name="configureLogging">The delegate that configures the <see cref="LoggerFactory"/>.</param>
        /// <returns>The <see cref="IConsoleBuilder"/>.</returns>
        public static IConsoleBuilder ConfigureLogging(this IConsoleBuilder applicationBuilder, Action<ConsoleBuilderContext, ILoggingBuilder> configureLogging)
        {
            return applicationBuilder.ConfigureServices((context, collection) => collection.AddLogging(builder => configureLogging(context, builder)));
        }

        /// <summary>
        /// Adds a delegate for configuring the provided <see cref="ILoggingBuilder"/>. This may be called multiple times.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IConsoleBuilder" /> to configure.</param>
        /// <param name="configureLogging">The delegate that configures the <see cref="ILoggingBuilder"/>.</param>
        /// <returns>The <see cref="IConsoleBuilder"/>.</returns>
        public static IConsoleBuilder ConfigureLogging(this IConsoleBuilder applicationBuilder, Action<ILoggingBuilder> configureLogging)
        {
            return applicationBuilder.ConfigureServices(collection => collection.AddLogging(configureLogging));
        }
    }
}
