using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using ConsoleBuildR.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ConsoleBuildR
{
    /// <summary>
    /// A builder for <see cref="IConsole"/>
    /// </summary>
    public class ConsoleBuilder : IConsoleBuilder
    {
        private readonly List<Action<ConsoleBuilderContext, IServiceCollection>> _configureServicesDelegates;

        private IConfiguration _config;
        private ConsoleBuilderContext _context;
        private bool _webHostBuilt;
        private List<Action<ConsoleBuilderContext, IConfigurationBuilder>> _configureAppConfigurationBuilderDelegates;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleBuilder"/> class.
        /// </summary>
        public ConsoleBuilder()
        {
            _configureServicesDelegates = new List<Action<ConsoleBuilderContext, IServiceCollection>>();
            _configureAppConfigurationBuilderDelegates = new List<Action<ConsoleBuilderContext, IConfigurationBuilder>>();

            _config = new ConfigurationBuilder()
                .Build();

            _context = new ConsoleBuilderContext
            {
                Configuration = _config
            };
        }

        /// <summary>
        /// Get the setting value from the configuration.
        /// </summary>
        /// <param name="key">The key of the setting to look up.</param>
        /// <returns>The value the setting currently contains.</returns>
        public string GetSetting(string key)
        {
            return _config[key];
        }

        /// <summary>
        /// Add or replace a setting in the configuration.
        /// </summary>
        /// <param name="key">The key of the setting to add or replace.</param>
        /// <param name="value">The value of the setting to add or replace.</param>
        /// <returns>The <see cref="IConsoleBuilder"/>.</returns>
        public IConsoleBuilder UseSetting(string key, string value)
        {
            _config[key] = value;
            return this;
        }

        /// <summary>
        /// Adds a delegate for configuring additional services for the console application. This may be called
        /// multiple times.
        /// </summary>
        /// <param name="configureServices">A delegate for configuring the <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IConsoleBuilder"/>.</returns>
        public IConsoleBuilder ConfigureServices(Action<ConsoleBuilderContext, IServiceCollection> configureServices)
        {
            if (configureServices == null)
            {
                throw new ArgumentNullException(nameof(configureServices));
            }

            _configureServicesDelegates.Add(configureServices);
            return this;
        }

        /// <summary>
        /// Adds a delegate for configuring additional services for the console application. This may be called
        /// multiple times.
        /// </summary>
        /// <param name="configureServices">A delegate for configuring the <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IConsoleBuilder"/>.</returns>
        public IConsoleBuilder ConfigureServices(Action<IServiceCollection> configureServices)
        {
            if (configureServices == null)
            {
                throw new ArgumentNullException(nameof(configureServices));
            }

            return ConfigureServices((_, services) => configureServices(services));
        }

        /// <summary>
        /// Adds a delegate for configuring the <see cref="IConfigurationBuilder"/> that will construct an <see cref="IConfiguration"/>.
        /// </summary>
        /// <param name="configureDelegate">The delegate for configuring the <see cref="IConfigurationBuilder" /> that will be used to construct an <see cref="IConfiguration" />.</param>
        /// <returns>The <see cref="IConsoleBuilder"/>.</returns>
        /// <remarks>
        /// The <see cref="IConfiguration"/> and <see cref="ILoggerFactory"/> on the <see cref="ConsoleBuilderContext"/> are uninitialized at this stage.
        /// The <see cref="IConfigurationBuilder"/> is pre-populated with the settings of the <see cref="IConsoleBuilder"/>.
        /// </remarks>
        public IConsoleBuilder ConfigureAppConfiguration(Action<ConsoleBuilderContext, IConfigurationBuilder> configureDelegate)
        {
            if (configureDelegate == null)
            {
                throw new ArgumentNullException(nameof(configureDelegate));
            }

            _configureAppConfigurationBuilderDelegates.Add(configureDelegate);
            return this;
        }

        /// <summary>
        /// Builds the required services and an <see cref="IConsole"/> which hosts a web application.
        /// </summary>
        public IConsole Build()
        {
            if (_webHostBuilt)
            {
                throw new InvalidOperationException("This console application has already been built.");
            }
            _webHostBuilt = true;

            var services = BuildCommonServices();
            var applicationServices = services.Clone();
            var hostingServiceProvider = GetProviderFromFactory(services);

            var service = new ConsoleApplication(applicationServices, hostingServiceProvider, _config);

            try
            {
                service.Initialize();

                return service;
            }
            catch
            {
                // Dispose the host if there's a failure to initialize, this should clean up
                // will dispose services that were constructed until the exception was thrown
                service.Dispose();
                throw;
            }

            IServiceProvider GetProviderFromFactory(IServiceCollection collection)
            {
                var provider = collection.BuildServiceProvider();
                var factory = provider.GetService<IServiceProviderFactory<IServiceCollection>>();

                if (factory != null)
                {
                    using (provider)
                    {
                        return factory.CreateServiceProvider(factory.CreateBuilder(collection));
                    }
                }

                return provider;
            }
        }

        private IServiceCollection BuildCommonServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton(_context);

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddConfiguration(_config);

            foreach (var configureAppConfiguration in _configureAppConfigurationBuilderDelegates)
            {
                configureAppConfiguration(_context, builder);
            }

            var configuration = builder.Build();
            services.AddSingleton<IConfiguration>(configuration);
            _context.Configuration = configuration;

            services.AddOptions();
            services.AddLogging();

            services.AddTransient<IServiceProviderFactory<IServiceCollection>, DefaultServiceProviderFactory>();

            foreach (var configureServices in _configureServicesDelegates)
            {
                configureServices(_context, services);
            }

            return services;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ConsoleBuilder"/> class with pre-configured defaults.
        /// </summary>
        /// <returns>The initialized <see cref="IConsoleBuilder"/>.</returns>
        public static IConsoleBuilder CreateDefaultBuilder()
        {
            var builder = new ConsoleBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);
                })
                .ConfigureLogging((windowsServiceContext, logging) =>
                {
                    logging.AddConfiguration(windowsServiceContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                })
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = false;
                });

            return builder;
        }
    }
}
