// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class NativeRuntimeBuilder {
    public static readonly string[] NativeRuntimeFiles = [
        "InfiniFrame.Native.dll",
        "WebView2Loader.dll",
        "InfiniFrame.Native.so",
        "InfiniFrame.Native.dylib"
    ];

    public static async Task BuildAsync(string nativeProjectPath, string repoRoot, string configuration, string platform, bool verbose) {
        if (!File.Exists(nativeProjectPath)) throw new FileNotFoundException("InfiniFrame native project was not found.", nativeProjectPath);

        string solutionDir = repoRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;

        Console.WriteLine("[InfiniFrame.Pack] Building native runtime");
        Console.WriteLine($"  NativeProject: {nativeProjectPath}");
        Console.WriteLine($"  Configuration: {configuration}");
        Console.WriteLine($"  Platform: {platform}");

        List<string> buildArgs = [
            "msbuild",
            nativeProjectPath,
            "-t:Build",
            $"-p:Configuration={configuration}",
            $"-p:Platform={platform}",
            $"-p:SolutionDir={solutionDir}",
            verbose ? "-v:normal" : "-v:minimal"
        ];

        int exitCode = await ProcessRunner.RunAsync("dotnet", buildArgs);
        if (exitCode != 0) throw new InvalidOperationException($"Native build failed with exit code {exitCode}.");
    }

    public static void ValidateArtifacts(string nativeArtifactsDir, string rid) {
        if (!Directory.Exists(nativeArtifactsDir)) throw new InvalidOperationException($"Native artifacts directory was not found: {nativeArtifactsDir}");

        foreach (string file in RequiredFilesForRid(rid)) {
            string path = Path.Combine(nativeArtifactsDir, file);
            if (!File.Exists(path)) throw new InvalidOperationException($"Required native artifact was not found: {path}");
        }
    }

    private static string[] RequiredFilesForRid(string rid) {
        if (rid.StartsWith("win-", StringComparison.OrdinalIgnoreCase)) return ["InfiniFrame.Native.dll", "WebView2Loader.dll"];
        if (rid.StartsWith("linux-", StringComparison.OrdinalIgnoreCase)) return ["InfiniFrame.Native.so"];
        if (rid.StartsWith("osx-", StringComparison.OrdinalIgnoreCase)) return ["InfiniFrame.Native.dylib"];

        throw new InvalidOperationException($"Unsupported RID for native artifact validation: {rid}");
    }
}
