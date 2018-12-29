# ConsoleBuildR
Upgrade your console application and use the same pattern as .NET Core MVC projects to configure services, logging, and use application settings.

## Sample
```csharp
    class Program
    {
        static void Main(string[] args)
        {
            var app = BuildConsoleApplication();

            Console.WriteLine("Running executables in order.");
            app.Run(args);

            Console.WriteLine("Running executables in parallel.");
            app.RunAsync(args).GetAwaiter().GetResult();
        }   

        static IConsole BuildConsoleApplication() =>
            ConsoleBuilder.CreateDefaultBuilder()
            .Execute<Executable1>()
            .Execute<Executable2>()
            .Build();
    }

    public class Executable1 : IExecutable
    {
        public Task Execute(string[] args)
        {
            Thread.Sleep(100);

            Console.WriteLine("Executable1: Hello World");

            return Task.CompletedTask;
        }
    }

    public class Executable2 : IExecutable
    {
        public Task Execute(string[] args)
        {
            Thread.Sleep(50);

            Console.WriteLine("Executable2: Hello World");

            return Task.CompletedTask;
        }
    }
```