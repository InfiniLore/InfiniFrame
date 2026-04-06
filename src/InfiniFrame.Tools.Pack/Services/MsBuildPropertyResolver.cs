// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics;

namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class MsBuildPropertyResolver {
    public static string? TryGetProperty(string projectPath, string propertyName) {
        var startInfo = new ProcessStartInfo("dotnet") {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        startInfo.ArgumentList.Add("msbuild");
        startInfo.ArgumentList.Add(projectPath);
        startInfo.ArgumentList.Add("-nologo");
        startInfo.ArgumentList.Add("-v:q");
        startInfo.ArgumentList.Add($"-getProperty:{propertyName}");

        using var process = new Process();
        process.StartInfo = startInfo;

        if (!process.Start()) return null;

        Task<string> stdOutTask = process.StandardOutput.ReadToEndAsync();
        Task<string> stdErrTask = process.StandardError.ReadToEndAsync();
        process.WaitForExit();
        string stdOut = stdOutTask.GetAwaiter().GetResult().Trim();
        _ = stdErrTask.GetAwaiter().GetResult();

        if (process.ExitCode != 0 || string.IsNullOrWhiteSpace(stdOut)) return null;

        return stdOut;
    }
}
