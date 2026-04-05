// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack.Exceptions;
using InfiniFrame.Tools.Pack.Services;
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
        bool verbose = args.Any(arg => string.Equals(arg, "--verbose", StringComparison.OrdinalIgnoreCase));
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(verbose ? LogEventLevel.Debug : LogEventLevel.Information)
            .Enrich.WithProperty("Tool", "InfiniFrame.Pack")
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        try {
            ParseResult parse = CommandLine.Parse(args);

            // ReSharper disable once InvertIf
            if (parse.ShowUsage) {
                CommandLine.PrintUsage();
                return parse.ExitCode;
            }

            return await PublishService.PublishAsync(parse.Options);

        }
        catch (NativeDependencyNotFoundException ex) {
            Log.Error(ex, "[InfiniFrame.Pack] ERROR: {Message}", ex.Message);
            return ExitCodes.NativeDependencyMissing;
        }
        catch (Exception ex) when (IsNonFatalException(ex)) {
            Log.Error(ex, "[InfiniFrame.Pack] ERROR: {Message}", ex.Message);
            return ExitCodes.GenericFailure;
        }
        finally {
            await Log.CloseAndFlushAsync();
        }
    }

    private static bool IsNonFatalException(Exception exception)
        => exception is not (OutOfMemoryException or AccessViolationException);
}
