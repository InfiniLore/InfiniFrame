// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using Serilog;

namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class NativeRuntimeBuilder {
    private static readonly ILogger Logger = Log.ForContext(typeof(NativeRuntimeBuilder));

    /// <summary>
    ///     The native runtime file names that are stripped from final publish output after embedding.
    /// </summary>
    public static readonly string[] NativeRuntimeFiles = [
        "InfiniFrame.Native.dll",
        "WebView2Loader.dll",
        "InfiniFrame.Native.so",
        "InfiniFrame.Native.dylib"
    ];

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------

    /// <summary>
    ///     Builds the native InfiniFrame runtime project for the resolved platform.
    /// </summary>
    /// <param name="nativeProjectPath">Path to <c>InfiniFrame.Native.proj</c>.</param>
    /// <param name="nativeProjectDirectory">Directory containing <paramref name="nativeProjectPath" />.</param>
    /// <param name="configuration">Build configuration, typically <c>Debug</c> or <c>Release</c>.</param>
    /// <param name="platform">Native platform value passed to MSBuild (for example, <c>x64</c>).</param>
    /// <param name="nativeArtifactsDir">Directory where native build artifacts are copied.</param>
    /// <param name="verbose"><see langword="true" /> to use normal verbosity; otherwise minimal verbosity.</param>
    /// <exception cref="FileNotFoundException">Thrown when <paramref name="nativeProjectPath" /> does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the native build process exits with a non-zero code.</exception>
    public static async Task BuildAsync(string nativeProjectPath, string nativeProjectDirectory, string configuration, string platform, string nativeArtifactsDir, bool verbose) {
        if (!File.Exists(nativeProjectPath)) throw new FileNotFoundException("InfiniFrame native project was not found.", nativeProjectPath);

        Logger.Information("[InfiniFrame.Pack] Building native runtime");
        Logger.Information("  NativeProject: {NativeProjectPath}", nativeProjectPath);
        Logger.Information("  Configuration: {Configuration}", configuration);
        Logger.Information("  Platform: {Platform}", platform);
        Logger.Information("  NativeArtifacts: {NativeArtifactsDir}", nativeArtifactsDir);

        List<string> buildArgs = [
            "msbuild",
            nativeProjectPath,
            "-t:Build",
            $"-p:Configuration={configuration}",
            $"-p:Platform={platform}",
            $"-p:NativeOutputDir={nativeArtifactsDir}",
            verbose ? "-v:normal" : "-v:minimal"
        ];

        int exitCode = await ProcessRunner.RunAsync("dotnet", buildArgs, nativeProjectDirectory);
        if (exitCode != 0) throw new InvalidOperationException($"Native build failed with exit code {exitCode}.");
    }

    /// <summary>
    ///     Validates that all required native artifacts for a RID are present in the artifact directory.
    /// </summary>
    /// <param name="nativeArtifactsDir">Directory containing native build outputs.</param>
    /// <param name="rid">Runtime identifier used to determine required native files.</param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the artifact directory is missing, a required file is missing, or the RID is unsupported.
    /// </exception>
    public static void ValidateArtifacts(string nativeArtifactsDir, string rid) {
        if (!Directory.Exists(nativeArtifactsDir)) throw new InvalidOperationException($"Native artifacts directory was not found: {nativeArtifactsDir}");

        IEnumerable<string> enumerable = RequiredFilesForRid(rid)
            .Select(file => Path.IsPathRooted(file) ? file : Path.Join(nativeArtifactsDir, file));
        
        foreach (string path in enumerable) {
            if (!File.Exists(path)) throw new InvalidOperationException($"Required native artifact was not found: {path}");
        }
    }

    [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
    private static string[] RequiredFilesForRid(string rid) {
        if (rid.StartsWith("win-", StringComparison.OrdinalIgnoreCase)) return ["InfiniFrame.Native.dll", "WebView2Loader.dll"];
        if (rid.StartsWith("linux-", StringComparison.OrdinalIgnoreCase)) return ["InfiniFrame.Native.so"];
        if (rid.StartsWith("osx-", StringComparison.OrdinalIgnoreCase)) return ["InfiniFrame.Native.dylib"];

        throw new InvalidOperationException($"Unsupported RID for native artifact validation: {rid}");
    }
}
