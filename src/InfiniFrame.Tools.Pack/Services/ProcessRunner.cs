// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics;

namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class ProcessRunner {
    public static async Task<int> RunAsync(string fileName, IReadOnlyList<string> arguments) {
        var startInfo = new ProcessStartInfo(fileName) {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

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
