// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Logging.Abstractions;

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
        var runner = new Services.ProcessRunner(NullLogger<Services.ProcessRunner>.Instance);

        Services.ProcessRunner.ProcessRunResult result = await runner.RunWithOutputAsync(
            "dotnet",
            ["msbuild", projectPath, "-nologo", "-v:q", $"-getProperty:{propertyName}"],
            timeout: timeout,
            cancellationToken: cancellationToken
        );

        if (result.ExitCode != 0 || string.IsNullOrWhiteSpace(result.StandardOutput)) return null;

        return result.StandardOutput.Trim();
    }
}
