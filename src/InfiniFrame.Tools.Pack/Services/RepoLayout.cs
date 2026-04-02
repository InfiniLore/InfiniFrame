// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class RepoLayout {
    /// <summary>
    /// Resolves repository-relative paths required by the pack pipeline.
    /// </summary>
    /// <param name="projectDirectory">Directory containing the project to publish.</param>
    /// <param name="rid">Resolved runtime identifier.</param>
    /// <param name="configuration">Build configuration.</param>
    /// <returns>Resolved repository and native artifact paths.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no repository root containing the native project can be located.
    /// </exception>
    public static RepoPaths Resolve(string projectDirectory, string rid, string configuration) {
        string repoRoot = ResolveRepositoryRoot(projectDirectory);
        string nativeProjectPath = Path.Combine(repoRoot, "src", "InfiniFrame.Native", "InfiniFrame.Native.proj");
        string nativeOsDir = RuntimeResolver.ResolveNativeOsDir(rid);
        string nativePlatform = RuntimeResolver.ResolveNativePlatform(rid);
        string nativeArtifactsDir = Path.Combine(repoRoot, "artifacts", "native", nativeOsDir, nativePlatform, configuration);

        return new RepoPaths {
            RepoRoot = repoRoot,
            NativeProjectPath = nativeProjectPath,
            NativeArtifactsDir = nativeArtifactsDir,
            NativePlatform = nativePlatform
        };
    }

    private static string ResolveRepositoryRoot(string projectDirectory) {
        DirectoryInfo? current = new(projectDirectory);
        while (current is not null) {
            string nativeProject = Path.Combine(current.FullName, "src", "InfiniFrame.Native", "InfiniFrame.Native.proj");
            if (File.Exists(nativeProject)) return current.FullName;

            current = current.Parent;
        }

        throw new InvalidOperationException("Unable to locate repository root containing src/InfiniFrame.Native/InfiniFrame.Native.proj.");
    }
}
