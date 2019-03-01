[![](https://img.shields.io/nuget/vpre/ConsoleBuildR.svg)](https://www.nuget.org/packages/ConsoleBuildR)
[![](https://img.shields.io/nuget/v/ConsoleBuildR.svg)](https://www.nuget.org/packages/ConsoleBuildR)

# ConsoleBuildR
Upgrade your console application and use the same pattern as .NET Core MVC projects to configure services, logging, and use application settings.

## Installation

### NuGet Package Manager
`Install-Package ConsoleBuildR`

### .NET CLI
`dotnet add package ConsoleBuildR`

## Usage

Add using statement to your application.

`using ConsoleBuildR;`

Start by implementing the `IExcutable` interface with the code you want to run.

```csharp
public class MyProgram : IExecutable
{
    public Task Execute(string[] args)
    {
	    Console.WriteLine("Hello World!");
		
	    return Task.Delay(1000);
    }
}
```

Then, change your `Program.cs` file to look like this:

```csharp
class Program
{
    static Task Main(string[] args) =>
        ConsoleBuilder.CreateDefaultBuilder()
        .Execute<MyProgram>()
        .Build()
        .Run(args);
}
```

The `Execute<MyProgram>()` method is how you tell the application builder what `IExcutable` implementations you want to run. You can chain multiple implementations to do different tasks.

## Dependency Injection

Configuring implementations for dependency injection is simple using the `ConfigureServices()` method just like you can do in a .NET Core Web Application.

Here's how you would register an Entity Framework Core `DbContext` for your console application.

```csharp
static Task Main(string[] args) =>
    ConsoleBuilder.CreateDefaultBuilder()
    .ConfigureServices((config, services) =>
    {
        services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("ApplicationDb"));
    })
    .Execute<MyProgram>()
    .Build()
    .Run(args);
```

Now you can inject it into your `IExcutable`

```csharp
public class MyProgram : IExecutable
{
	private readonly ApplicationDbContext _applicationDbContext;

	public MyProgram(ApplicationDbContext applicationDbContext)
	{
	    _applicationDbContext = applicationDbContext;
	}

	public Task Execute(string[] args)
	{
		// do something with your db
		return _applicationDbContext.SaveChangesAsync();
	}
}
```

## Application Settings

Make sure the copy to output directory property on your `appsettings.json` is set to copy if newer or always.

## Console Builder

`ConsoleBuilder.CreateDefaultBuilder()` will create a default implementation that has Microsoft's console logging, and use appsettings.json for configuration.

```csharp
public static IConsoleBuilder CreateDefaultBuilder()
{
    var builder = new ConsoleBuilder()
        .ConfigureAppConfiguration((context, config) =>
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                  .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);
        })
        .ConfigureLogging((context, logging) =>
        {
            logging.AddConfiguration(context.Configuration.GetSection("Logging"));
            logging.AddConsole();
        })
        .UseDefaultServiceProvider((context, options) =>
        {
            options.ValidateScopes = false;
        });

    return builder;
}
```

You can create your own configuration using the `new ConsoleBuilder()` if you require customizations.

## Sample
```csharp
class Program
{
    static Task Main(string[] args) =>
        ConsoleBuilder.CreateDefaultBuilder()
        .Execute<App>()
        .Build()
        .Run(args);
}

public class App : IExecutable
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;

    public App(ILogger<App> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public Task Execute(string[] args)
    {
        _logger.LogInformation(_configuration.GetValue<string>("Message"));

        return Task.Delay(100);
    }
}
```
`appsettings.json`

```json
{
  "Message":  "Hello World!"
}
```
