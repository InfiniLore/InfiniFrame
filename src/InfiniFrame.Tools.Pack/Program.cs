// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack.Exceptions;
using InfiniFrame.Tools.Pack.Services;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace InfiniFrame.Tools.Pack;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class Program {
    /// <summary>
    ///     Parses command-line arguments and executes the requested pack operation.
    /// </summary>
    /// <param name="args">The command-line arguments passed to the tool process.</param>
    /// <returns>
    ///     <c>0</c> when usage is shown successfully or publish completes successfully; otherwise, a non-zero exit code.
    /// </returns>
    public static async Task<int> Main(string[] args) {
        using var cts = new CancellationTokenSource();
        ConsoleCancelEventHandler cancelHandler = (_, e) => {
            e.Cancel = true;
            // ReSharper disable once AccessToDisposedClosure
            cts.Cancel();
        };
        Console.CancelKeyPress += cancelHandler;

        bool verbose = args.Any(arg => string.Equals(arg, "--verbose", StringComparison.OrdinalIgnoreCase));
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(verbose ? LogEventLevel.Debug : LogEventLevel.Information)
            .Enrich.WithProperty("Tool", "InfiniFrame.Pack")
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        try {
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddSerilog(dispose: true));
            services.AddSingleton<ProcessRunner>();
            services.AddSingleton<PublishService>();
            services.AddSingleton<CommandLine>();
            using ServiceProvider provider = services.BuildServiceProvider();

            var commandLine = provider.GetRequiredService<CommandLine>();
            ParseResult parse = commandLine.Parse(args);

            // ReSharper disable once InvertIf
            if (parse.ShowUsage) {
                commandLine.PrintUsage();
                return parse.ExitCode;
            }

            var publishService = provider.GetRequiredService<PublishService>();
            return await publishService.PublishAsync(parse.Options, cts.Token);

        }
        catch (OperationCanceledException) {
            Log.Warning("Operation canceled.");
            return ExitCodes.GenericFailure;
        }
        catch (NativeDependencyNotFoundException ex) {
            Log.Error(ex, "ERROR: {Message}", ex.Message);
            return ExitCodes.NativeDependencyMissing;
        }
        catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
            Log.Error(ex, "ERROR: {Message}", ex.Message);
            return ExitCodes.GenericFailure;
        }
        finally {
            Console.CancelKeyPress -= cancelHandler;
            await Log.CloseAndFlushAsync();
        }
    }
}
