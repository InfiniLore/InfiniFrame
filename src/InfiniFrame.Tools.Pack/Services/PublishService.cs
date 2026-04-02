// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack.Exceptions;
using System.Runtime.InteropServices;

namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class PublishService {
    private const string DotNet = "dotnet";

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------

    /// <summary>
    ///     Executes the full InfiniFrame publish pipeline for a project.
    /// </summary>
    /// <param name="options">Publish options parsed from the command line.</param>
    /// <returns>The process exit code of the publish operation.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the target project file does not exist.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when repository layout cannot be resolved, native build fails, or required artifacts are missing.
    /// </exception>
    public static async Task<int> PublishAsync(PublishOptions options) {
        string projectPath = Path.GetFullPath(options.ProjectPath);
        if (!File.Exists(projectPath)) throw new FileNotFoundException("Project file not found", projectPath);

        string projectDirectory = Path.GetDirectoryName(projectPath) ?? throw new InvalidOperationException("Unable to resolve project directory.");
        string framework = string.IsNullOrWhiteSpace(options.Framework) ? ProjectInfoResolver.ResolveFramework(projectPath) : options.Framework!;
        string rid = RuntimeResolver.ResolveRid(options.Rid);
        string output = ResolveOutputPath(options, projectDirectory, framework, rid);
        string assemblyName = ProjectInfoResolver.ResolveAssemblyName(projectPath);

        ResolvedNativeArtifacts nativeArtifacts = await ResolveNativeArtifactsAsync(options, projectPath, framework, rid);

        PrintPublishSummary(projectPath, framework, rid, options.SelfContained, output, nativeArtifacts.Directory);

        if (Directory.Exists(output)) Directory.Delete(output, true);
        Directory.CreateDirectory(output);

        try {
            using var tempTargets = TempTargetsFile.Create();

            List<string> publishArgs = [
                "publish",
                projectPath,
                "-c", options.Configuration,
                "-r", rid,
                "-f", framework,
                "--output", output,
                "-p:InfiniFramePackInvoked=true",
                "-p:PublishSingleFile=true",
                $"-p:SelfContained={options.SelfContained.ToString().ToLowerInvariant()}",
                "-p:IncludeNativeLibrariesForSelfExtract=true",
                "-p:IncludeAllContentForSelfExtract=true",
                "-p:EnableCompressionInSingleFile=true",
                "-p:DebugType=none",
                "-p:DebugSymbols=false",
                $"-p:InfiniFramePackRootProject={projectPath}",
                $"-p:InfiniFramePackRuntimeIdentifier={rid}",
                $"-p:InfiniFramePackNativeArtifactsDir={nativeArtifacts.Directory}",
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
        finally {
            if (nativeArtifacts.DeleteWhenDone && Directory.Exists(nativeArtifacts.Directory)) {
                Directory.Delete(nativeArtifacts.Directory, true);
            }
        }
    }

    private static string ResolveOutputPath(PublishOptions options, string projectDirectory, string framework, string rid) =>
        string.IsNullOrWhiteSpace(options.Output)
            ? Path.Combine(projectDirectory, "bin", options.Configuration, framework, rid, "publish")
            : Path.GetFullPath(options.Output!);

    private static string ResolveExpectedMainOutputPath(string output, string assemblyName) =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? Path.Combine(output, $"{assemblyName}.exe")
            : Path.Combine(output, assemblyName);

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

    private static async Task<ResolvedNativeArtifacts> ResolveNativeArtifactsAsync(
        PublishOptions options,
        string projectPath,
        string framework,
        string rid
    ) {
        string preflightDirectory = Path.Combine(Path.GetTempPath(), $"infiniframe-pack-native-{Guid.NewGuid():N}");
        Directory.CreateDirectory(preflightDirectory);
        int preflightExitCode = await RunPreflightPublishAsync(options, projectPath, framework, rid, preflightDirectory);
        if (preflightExitCode != 0) {
            throw new InvalidOperationException($"Preflight publish failed with exit code {preflightExitCode}.");
        }

        try {
            NativeRuntimeBuilder.ValidateArtifacts(preflightDirectory, rid);
            return new ResolvedNativeArtifacts(preflightDirectory, true);
        }
        catch (InvalidOperationException preflightValidationError) {
            throw new NativeDependencyNotFoundException(
                "Could not resolve required InfiniFrame native artifacts from project publish output. " +
                "Ensure InfiniFrame is included as a dependency for this project/RID and that native runtime files are produced, " +
                "and that publish preserves native runtime files. " +
                $"Details: {preflightValidationError.Message}"
            );
        }
    }

    private static async Task<int> RunPreflightPublishAsync(PublishOptions options, string projectPath, string framework, string rid, string outputDirectory) {
        List<string> publishArgs = [
            "publish",
            projectPath,
            "-c", options.Configuration,
            "-r", rid,
            "-f", framework,
            "--output", outputDirectory,
            "-p:InfiniFramePackInvoked=true",
            "-p:PublishSingleFile=false",
            $"-p:SelfContained={options.SelfContained.ToString().ToLowerInvariant()}",
            "-p:IncludeNativeLibrariesForSelfExtract=true",
            options.Verbose ? "-v:normal" : "-v:minimal"
        ];

        if (options.NoRestore) publishArgs.Add("--no-restore");
        return await ProcessRunner.RunAsync(DotNet, publishArgs);
    }

    private sealed record ResolvedNativeArtifacts(string Directory, bool DeleteWhenDone);
}
