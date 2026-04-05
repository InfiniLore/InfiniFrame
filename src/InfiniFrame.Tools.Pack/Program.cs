// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack.Exceptions;
using InfiniFrame.Tools.Pack.Services;

namespace InfiniFrame.Tools.Pack;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class Program {
    private const int GenericFailureExitCode = 1;
    private const int NativeDependencyMissingExitCode = 2;

    /// <summary>
    ///     Parses command-line arguments and executes the requested pack operation.
    /// </summary>
    /// <param name="args">The command-line arguments passed to the tool process.</param>
    /// <returns>
    ///     <c>0</c> when usage is shown successfully or publish completes successfully; otherwise, a non-zero exit code.
    /// </returns>
    public static async Task<int> Main(string[] args) {
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
            await Console.Error.WriteLineAsync($"[InfiniFrame.Pack] ERROR: {ex.Message}");
            return NativeDependencyMissingExitCode;
        }
        catch (Exception ex) when (IsNonFatalException(ex)) {
            await Console.Error.WriteLineAsync($"[InfiniFrame.Pack] ERROR: {ex.Message}");
            return GenericFailureExitCode;
        }
    }

    private static bool IsNonFatalException(Exception exception)
        => exception is not (OutOfMemoryException or AccessViolationException);
}
