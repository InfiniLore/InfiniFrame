// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class RepoLayout {
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