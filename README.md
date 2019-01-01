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
		
		return Task.CompletedTask;
    }
}
```

Then, change your `Program.cs` file to look like this:

```csharp
class Program
{
    static void Main(string[] args)
    {
        BuildConsoleApplication().Run(args);
    }   

    static IConsole BuildConsoleApplication() =>
        ConsoleBuilder.CreateDefaultBuilder()
        .Execute<MyProgram>()
        .Build();
}
```

The `Execute<MyProgram>()` method is how you tell the application builder what `IExcutable` implementations you want to run. You can chain multiple implementations to do different tasks. Call `Run(args)` to run in series or `RunAsync(args)` to run in parallel.

## Dependency Injection

Configuring implementations for dependency injection is simple using the `ConfigureServices()` method just like you can do in a .NET Core Web Application.

Here's how you would register an Entity Framework Core `DbContext` for your console application.

```csharp
static IConsole BuildConsoleApplication() =>
    ConsoleBuilder.CreateDefaultBuilder()
    .ConfigureServices((config, services) =>
    {
        services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("ApplicationDb"));
    })
    .Execute<MyProgram>()
    .Build();
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

	public async Task Execute(string[] args)
	{
		// do something with your db
		await _applicationDbContext.SaveChangesAsync();
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
    static void Main(string[] args)
    {
        var app = BuildConsoleApplication();

        Console.WriteLine("Running executables in order.");
        app.Run(args);

        Thread.Sleep(200);

        Console.WriteLine("Running executables in parallel.");
        app.RunAsync(args).GetAwaiter().GetResult();

        Thread.Sleep(200);
    }   

    static IConsole BuildConsoleApplication() =>
        ConsoleBuilder.CreateDefaultBuilder()
        .Execute<Executable1>()
        .Execute<Executable2>()
        .Build();
}

public class Executable1 : IExecutable
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;

    public Executable1(ILogger<Executable1> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public Task Execute(string[] args)
    {
        Thread.Sleep(100);

        _logger.LogInformation(_configuration.GetValue<string>("Message"));

        return Task.CompletedTask;
    }
}

public class Executable2 : IExecutable
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;

    public Executable2(ILogger<Executable2> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public Task Execute(string[] args)
    {
        Thread.Sleep(50);

        _logger.LogInformation(_configuration.GetValue<string>("Message"));

        return Task.CompletedTask;
    }
}
```