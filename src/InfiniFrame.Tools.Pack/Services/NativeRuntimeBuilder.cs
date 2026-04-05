// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class NativeRuntimeBuilder {
    /// <summary>
    ///     The native runtime file names that are stripped from final publish output after embedding.
    /// </summary>
    public static readonly string[] NativeRuntimeFiles = [
        "InfiniFrame.Native.dll",
        "WebView2Loader.dll",
        "InfiniFrame.Native.so",
        "InfiniFrame.Native.dylib"
    ];

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
