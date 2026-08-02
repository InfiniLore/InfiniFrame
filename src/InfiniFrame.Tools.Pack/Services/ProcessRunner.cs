// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Serilog;
using System.Diagnostics;
using System.Text;

namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
/// Provides functionality for running and managing external processes asynchronously.
/// </summary>
internal static class ProcessRunner {
    /// <summary>
    /// Represents the default timeout duration for processes executed using the <c>ProcessRunner</c> class.
    /// This timeout is used to cancel the process if it exceeds the specified duration.
    /// By default, the timeout is set to 10 minutes.
    /// </summary>
    public static readonly TimeSpan DefaultProcessTimeout = TimeSpan.FromMinutes(10);
    /// <summary>
    /// Represents a logger instance used for capturing and logging process-related information
    /// such as standard output, standard error, and other diagnostic messages.
    /// </summary>
    /// <remarks>
    /// This logger instance is specifically scoped for the <c>ProcessRunner</c> class
    /// and is intended to provide detailed logging of process execution activities,
    /// including informational messages and error handling.
    /// </remarks>
    private static readonly ILogger Logger = Log.ForContext(typeof(ProcessRunner));

    /// <summary>
    /// Asynchronously executes an external process using the specified parameters and returns the exit code upon completion.
    /// </summary>
    /// <param name="fileName">The name or full path of the executable file to run.</param>
    /// <param name="arguments">The command-line arguments to pass to the executable.</param>
    /// <param name="workingDirectory">The working directory for the process, or null to use the current directory.</param>
    /// <param name="timeout">The maximum amount of time to allow the process to run before it is terminated, or null for no timeout.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The exit code of the process upon its completion.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the process fails to start or encounters an unexpected error during execution.</exception>
    /// <exception cref="TaskCanceledException">Thrown if the process is aborted due to exceeding the specified timeout or cancellation token.</exception>
    public static async Task<int> RunAsync(
        string fileName,
        IReadOnlyList<string> arguments,
        string? workingDirectory = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default
    ) {
        ProcessRunResult result = await RunWithOutputAsync(fileName, arguments, workingDirectory, timeout, cancellationToken);
        return result.ExitCode;
    }

    /// <summary>
    /// Runs an external process asynchronously, captures stdout/stderr, streams output to the current console, and returns the exit code along with the captured output.
    /// </summary>
    /// <param name="fileName">The name or path of the executable to run.</param>
    /// <param name="arguments">The arguments to pass to the executable as discrete tokens.</param>
    /// <param name="workingDirectory">The optional working directory for the process. Defaults to null.</param>
    /// <param name="timeout">The optional timeout duration for the process execution. Defaults to null, resulting in a predefined timeout being used.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="ProcessRunResult"/> struct containing the process exit code, captured standard output, and captured standard error.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the process fails to start.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the specified timeout duration is zero or negative.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled or the timeout elapses before the process completes.</exception>
    public static async Task<ProcessRunResult> RunWithOutputAsync(
        string fileName,
        IReadOnlyList<string> arguments,
        string? workingDirectory = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default
    ) {
        TimeSpan effectiveTimeout = timeout ?? DefaultProcessTimeout;
        if (effectiveTimeout <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must be greater than zero.");

        var startInfo = new ProcessStartInfo(fileName) {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        if (!string.IsNullOrWhiteSpace(workingDirectory)) startInfo.WorkingDirectory = workingDirectory;

        foreach (string arg in arguments) {
            startInfo.ArgumentList.Add(arg);
        }

        var standardOutput = new StringBuilder();
        var standardError = new StringBuilder();
        var standardOutputLock = new Lock();
        var standardErrorLock = new Lock();

        using var process = new Process();
        process.StartInfo = startInfo;
        process.EnableRaisingEvents = true;

        process.OutputDataReceived += (_, e) => {
            if (string.IsNullOrWhiteSpace(e.Data)) return;

            lock (standardOutputLock) {
                standardOutput.AppendLine(e.Data);
            }

            Logger.Information("{ProcessOutput}", e.Data);
        };

        process.ErrorDataReceived += (_, e) => {
            if (string.IsNullOrWhiteSpace(e.Data)) return;

            lock (standardErrorLock) {
                standardError.AppendLine(e.Data);
            }

            Logger.Error("{ProcessError}", e.Data);
        };

        if (!process.Start()) throw new InvalidOperationException($"Failed to start process: {fileName}");

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        using var timeoutCts = new CancellationTokenSource(effectiveTimeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
        try {
            await process.WaitForExitAsync(linkedCts.Token);
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested) {
            try {
                if (!process.HasExited) process.Kill(entireProcessTree: true);
            }
            catch (InvalidOperationException) {
                // best effort
            }

            throw new TimeoutException($"Timed out after {effectiveTimeout} while running '{fileName}'.");
        }
        catch (OperationCanceledException) {
            try {
                if (!process.HasExited) process.Kill(entireProcessTree: true);
            }
            catch (InvalidOperationException) {
                // best effort
            }

            throw;
        }

        string capturedStandardOutput;
        string capturedStandardError;
        lock (standardOutputLock) {
            capturedStandardOutput = standardOutput.ToString();
        }

        lock (standardErrorLock) {
            capturedStandardError = standardError.ToString();
        }

        return new ProcessRunResult(process.ExitCode, capturedStandardOutput, capturedStandardError);
    }

    /// <summary>
    /// Represents the result of a process execution.
    /// </summary>
    /// <remarks>
    /// This type provides information about the outcome of a process that was executed using the ProcessRunner utility,
    /// including the exit code, captured standard output, and captured standard error.
    /// </remarks>
    internal readonly record struct ProcessRunResult(int ExitCode, string StandardOutput, string StandardError);
}