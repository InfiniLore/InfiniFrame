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
internal static class ProcessRunner {
    private static readonly ILogger Logger = Log.ForContext(typeof(ProcessRunner));

    /// <summary>
    ///     Runs an external process, streams stdout/stderr to the current console, and returns the process exit code.
    /// </summary>
    /// <param name="fileName">Executable name or path.</param>
    /// <param name="arguments">Arguments passed as discrete tokens.</param>
    /// <param name="workingDirectory">Optional process working directory.</param>
    /// <returns>The exit code reported by the process.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the process fails to start.</exception>
    public static async Task<int> RunAsync(string fileName, IReadOnlyList<string> arguments, string? workingDirectory = null) {
        ProcessRunResult result = await RunWithOutputAsync(fileName, arguments, workingDirectory);
        return result.ExitCode;
    }

    /// <summary>
    ///     Runs an external process, streams stdout/stderr to the current console, and returns exit code and captured output.
    /// </summary>
    /// <param name="fileName">Executable name or path.</param>
    /// <param name="arguments">Arguments passed as discrete tokens.</param>
    /// <param name="workingDirectory">Optional process working directory.</param>
    /// <returns>Exit code and captured stdout/stderr.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the process fails to start.</exception>
    public static async Task<ProcessRunResult> RunWithOutputAsync(string fileName, IReadOnlyList<string> arguments, string? workingDirectory = null) {
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
        await process.WaitForExitAsync();

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

    internal readonly record struct ProcessRunResult(int ExitCode, string StandardOutput, string StandardError);
}
