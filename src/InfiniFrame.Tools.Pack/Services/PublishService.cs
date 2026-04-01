// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class PublishService {
    private const string DotNet = "dotnet";

    public static async Task<int> PublishAsync(PublishOptions options) {
        string projectPath = Path.GetFullPath(options.ProjectPath);
        if (!File.Exists(projectPath)) throw new FileNotFoundException("Project file not found", projectPath);

        string projectDirectory = Path.GetDirectoryName(projectPath) ?? throw new InvalidOperationException("Unable to resolve project directory.");
        string framework = string.IsNullOrWhiteSpace(options.Framework) ? ProjectInfoResolver.ResolveFramework(projectPath) : options.Framework!;
        string rid = RuntimeResolver.ResolveRid(options.Rid);
        string output = ResolveOutputPath(options, projectDirectory, framework, rid);
        string assemblyName = ProjectInfoResolver.ResolveAssemblyName(projectPath);

        RepoPaths paths = RepoLayout.Resolve(projectDirectory, rid, options.Configuration);

        PrintPublishSummary(projectPath, framework, rid, options.SelfContained, output, paths.NativeArtifactsDir);

        if (Directory.Exists(output)) Directory.Delete(output, recursive: true);
        Directory.CreateDirectory(output);

        await NativeRuntimeBuilder.BuildAsync(paths.NativeProjectPath, paths.RepoRoot, options.Configuration, paths.NativePlatform, options.Verbose);
        NativeRuntimeBuilder.ValidateArtifacts(paths.NativeArtifactsDir, rid);

        using var tempTargets = TempTargetsFile.Create();

        List<string> publishArgs = [
            "publish",
            projectPath,
            "-c", options.Configuration,
            "-r", rid,
            "-f", framework,
            "--output", output,
            "-p:PublishSingleFile=true",
            $"-p:SelfContained={options.SelfContained.ToString().ToLowerInvariant()}",
            "-p:IncludeNativeLibrariesForSelfExtract=true",
            "-p:IncludeAllContentForSelfExtract=true",
            "-p:EnableCompressionInSingleFile=true",
            "-p:DebugType=none",
            "-p:DebugSymbols=false",
            $"-p:InfiniFramePackRootProject={projectPath}",
            $"-p:InfiniFramePackRuntimeIdentifier={rid}",
            $"-p:InfiniFramePackNativeArtifactsDir={paths.NativeArtifactsDir}",
            $"-p:CustomAfterMicrosoftCommonTargets={tempTargets.Path}",
            options.Verbose ? "-v:normal" : "-v:minimal"
        ];

        if (options.NoRestore) publishArgs.Add("--no-restore");

        int exitCode = await ProcessRunner.RunAsync(DotNet, publishArgs);
        if (exitCode != 0) return exitCode;

        PublishOutputCleaner.Cleanup(output);
        PrintOutputSummary(output, ResolveExpectedMainOutputPath(output, assemblyName));
        return 0;
    }

    private static string ResolveOutputPath(PublishOptions options, string projectDirectory, string framework, string rid) {
        return string.IsNullOrWhiteSpace(options.Output)
            ? Path.Combine(projectDirectory, "bin", options.Configuration, framework, rid, "publish")
            : Path.GetFullPath(options.Output!);
    }

    private static string ResolveExpectedMainOutputPath(string output, string assemblyName) {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? Path.Combine(output, $"{assemblyName}.exe")
            : Path.Combine(output, assemblyName);
    }

    private static void PrintPublishSummary(string projectPath, string framework, string rid, bool selfContained, string output, string nativeArtifacts) {
        Console.WriteLine("[InfiniFrame.Pack] Publishing single-file app");
        Console.WriteLine($"  Project: {projectPath}");
        Console.WriteLine($"  Framework: {framework}");
        Console.WriteLine($"  RID: {rid}");
        Console.WriteLine($"  SelfContained: {selfContained}");
        Console.WriteLine($"  Output: {output}");
        Console.WriteLine($"  NativeArtifacts: {nativeArtifacts}");
    }

    private static void PrintOutputSummary(string output, string expectedMainOutput) {
        if (!File.Exists(expectedMainOutput)) {
            Console.WriteLine("[InfiniFrame.Pack] Publish succeeded, but expected single-file output was not found.");
        }

        string[] files = Directory.GetFiles(output, "*", SearchOption.TopDirectoryOnly);
        Console.WriteLine("[InfiniFrame.Pack] Completed");
        Console.WriteLine($"  Files in output: {files.Length}");
        foreach (string file in files.Select(Path.GetFileName).Where(x => !string.IsNullOrWhiteSpace(x)).OrderBy(x => x)!) {
            Console.WriteLine($"  - {file}");
        }
    }
}
