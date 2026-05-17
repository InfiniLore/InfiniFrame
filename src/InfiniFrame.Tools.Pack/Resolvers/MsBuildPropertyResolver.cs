// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics;

namespace InfiniFrame.Tools.Pack.Resolvers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class MsBuildPropertyResolver {
    public static async Task<string?> TryGetPropertyAsync(
        string projectPath,
        string propertyName,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default
    ) {
        TimeSpan effectiveTimeout = timeout ?? TimeSpan.FromMinutes(2);
        if (effectiveTimeout <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must be greater than zero.");

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

        Task<string> stdOutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        Task<string> stdErrTask = process.StandardError.ReadToEndAsync(cancellationToken);
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

            throw new TimeoutException(
                $"Timed out after {effectiveTimeout} while evaluating MSBuild property '{propertyName}' for '{projectPath}'.");
        }

        string stdOut = (await stdOutTask).Trim();
        _ = await stdErrTask;

        if (process.ExitCode != 0 || string.IsNullOrWhiteSpace(stdOut)) return null;

        return stdOut;
    }
}
