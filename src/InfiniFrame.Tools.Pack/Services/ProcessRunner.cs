// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics;

namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class ProcessRunner {
    /// <summary>
    ///     Runs an external process, streams stdout/stderr to the current console, and returns the process exit code.
    /// </summary>
    /// <param name="fileName">Executable name or path.</param>
    /// <param name="arguments">Arguments passed as discrete tokens.</param>
    /// <param name="workingDirectory">Optional process working directory.</param>
    /// <returns>The exit code reported by the process.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the process fails to start.</exception>
    public static async Task<int> RunAsync(string fileName, IReadOnlyList<string> arguments, string? workingDirectory = null) {
        var startInfo = new ProcessStartInfo(fileName) {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        if (!string.IsNullOrWhiteSpace(workingDirectory)) startInfo.WorkingDirectory = workingDirectory;

        foreach (string arg in arguments) {
            startInfo.ArgumentList.Add(arg);
        }

        using var process = new Process();
        process.StartInfo = startInfo;
        process.EnableRaisingEvents = true;

        process.OutputDataReceived += (_, e) => {
            if (!string.IsNullOrWhiteSpace(e.Data)) Console.WriteLine(e.Data);
        };

        process.ErrorDataReceived += (_, e) => {
            if (!string.IsNullOrWhiteSpace(e.Data)) Console.Error.WriteLine(e.Data);
        };

        if (!process.Start()) throw new InvalidOperationException($"Failed to start process: {fileName}");

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        await process.WaitForExitAsync();
        return process.ExitCode;
    }
}
