// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class NativeRuntimeBuilder {
    /// <summary>
    ///     The native runtime file names that are stripped from the final publish output after embedding.
    /// </summary>
    public static readonly string[] NativeRuntimeFiles = InfiniFrameNativeArtifactManifest.AllFileNames;

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

    private static string[] RequiredFilesForRid(string rid) => InfiniFrameNativeArtifactManifest.RequiredFileNamesForRid(rid);
}
