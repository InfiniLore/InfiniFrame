// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack.Exceptions;
using Serilog;

namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class PublishService {
    private const string DotNet = "dotnet";
    private static readonly StringComparison PathComparison =
        OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
    private static readonly ILogger Logger = Log.ForContext(typeof(PublishService));

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

        // Safe recursive deletion
        if (Directory.Exists(output)) SafeDeleteDirectory(output, projectDirectory, options.ForceCleanOutput);
        Directory.CreateDirectory(output);

        try {
            using var tempTargets = TempTargetsFile.Create();

            List<string> publishArgs = BuildPublishArguments(
                options,
                projectPath,
                framework,
                rid,
                output,
                nativeArtifacts.Directory,
                tempTargets.Path
            );

            int exitCode = await ProcessRunner.RunAsync(DotNet, publishArgs);
            if (exitCode != 0) return exitCode;

            string[] cleanupWarnings = PublishOutputCleaner.Cleanup(output);
            foreach (string warning in cleanupWarnings) {
                Logger.Warning("[InfiniFrame.Pack] {CleanupWarning}", warning);
            }
            string expectedMainOutput = ResolveExpectedMainOutputPath(output, assemblyName, rid);
            OutputShapeValidation validation = ValidateOutputShape(output, expectedMainOutput);
            PrintOutputSummary(output, expectedMainOutput, validation.UnexpectedEntries);

            if (!validation.FoundMainOutput) return ExitCodes.MissingMainOutput;
            return validation.UnexpectedEntries.Length == 0 ? ExitCodes.Success : ExitCodes.UnexpectedOutputShape;
        }
        finally {
            if (nativeArtifacts.DeleteWhenDone && Directory.Exists(nativeArtifacts.Directory)) {
                Directory.Delete(nativeArtifacts.Directory, true);
            }
        }
    }

    private static string ResolveOutputPath(PublishOptions options, string projectDirectory, string framework, string rid) =>
        string.IsNullOrWhiteSpace(options.Output)
            ? Path.Join(projectDirectory, "bin", options.Configuration, framework, rid, "publish")
            : Path.GetFullPath(options.Output!);

    private static string ResolveExpectedMainOutputPath(string output, string assemblyName, string rid) {
        string extension = rid.StartsWith("win-", StringComparison.OrdinalIgnoreCase) ? ".exe" : "";
        return Path.Join(output, $"{assemblyName}{extension}");
    }

    private static void PrintPublishSummary(string projectPath, string framework, string rid, bool selfContained, string output, string nativeArtifacts) {
        Logger.Information("[InfiniFrame.Pack] Publishing single-file app");
        Logger.Information("  Project: {ProjectPath}", projectPath);
        Logger.Information("  Framework: {Framework}", framework);
        Logger.Information("  RID: {Rid}", rid);
        Logger.Information("  SelfContained: {SelfContained}", selfContained);
        Logger.Information("  Output: {Output}", output);
        Logger.Information("  NativeArtifacts: {NativeArtifacts}", nativeArtifacts);
    }

    internal static OutputShapeValidation ValidateOutputShape(string output, string expectedMainOutput) {
        string normalizedExpectedMainOutput = Path.GetFullPath(expectedMainOutput);
        bool foundMainOutput = File.Exists(normalizedExpectedMainOutput);
        string[] unexpectedFiles = Directory.GetFiles(output, "*", SearchOption.TopDirectoryOnly)
            .Where(file => !string.Equals(Path.GetFullPath(file), normalizedExpectedMainOutput, PathComparison))
            .Select(file => Path.GetFileName(file))
            .Where(fileName => !string.IsNullOrWhiteSpace(fileName))
            .ToArray();
        string[] unexpectedDirectories = Directory.GetDirectories(output, "*", SearchOption.TopDirectoryOnly)
            .Select(directory => Path.GetFileName(directory))
            .Where(directoryName => !string.IsNullOrWhiteSpace(directoryName))
            .ToArray();
        string[] unexpectedEntries = unexpectedFiles
            .Concat(unexpectedDirectories)
            .OrderBy(entry => entry, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new OutputShapeValidation(foundMainOutput, unexpectedEntries);
    }

    private static void PrintOutputSummary(string output, string expectedMainOutput, string[] unexpectedEntries) {
        if (!File.Exists(expectedMainOutput)) {
            Logger.Warning("[InfiniFrame.Pack] Publish succeeded, but expected single-file output was not found.");
        }
        else if (unexpectedEntries.Length != 0) {
            Logger.Warning("[InfiniFrame.Pack] Publish output contains unexpected entries.");
        }

        string[] files = Directory.GetFiles(output, "*", SearchOption.TopDirectoryOnly);
        Logger.Information("[InfiniFrame.Pack] Completed");
        Logger.Information("  Files in output: {FileCount}", files.Length);
        foreach (string file in files.Select(Path.GetFileName).Where(x => !string.IsNullOrWhiteSpace(x)).OrderBy(x => x)!) {
            Logger.Information("  - {File}", file);
        }

        if (unexpectedEntries.Length == 0) return;

        Logger.Warning("  Unexpected entries:");
        foreach (string unexpectedEntry in unexpectedEntries) {
            Logger.Warning("  - {UnexpectedEntry}", unexpectedEntry);
        }
    }

    private static async Task<ResolvedNativeArtifacts> ResolveNativeArtifactsAsync(
        PublishOptions options,
        string projectPath,
        string framework,
        string rid
    ) {
        string preflightDirectory = Path.Join(Path.GetTempPath(), $"infiniframe-pack-native-{Guid.NewGuid():N}");
        Directory.CreateDirectory(preflightDirectory);

        bool preflightValidated = false;
        try {
            int preflightExitCode = await ProcessRunner.RunAsync(DotNet,
                BuildPublishArguments(options, projectPath, framework, rid, preflightDirectory, isPreflight: true));

            if (preflightExitCode != 0) {
                throw new InvalidOperationException($"Preflight publish failed with exit code {preflightExitCode}.");
            }

            try {
                NativeRuntimeBuilder.ValidateArtifacts(preflightDirectory, rid);
                preflightValidated = true;
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
        finally {
            if (!preflightValidated && Directory.Exists(preflightDirectory)) Directory.Delete(preflightDirectory, true);
        }
    }

    private static void SafeDeleteDirectory(string path, string projectDirectory, bool forceCleanOutput) {
        string fullPath = Path.GetFullPath(path);
        OutputPathSafety.EnsureOutputCanBeDeleted(fullPath, projectDirectory, forceCleanOutput);

        // Log deletion for transparency
        Logger.Information("[InfiniFrame.Pack] Cleaning previous output folder: {OutputDirectory}", fullPath);

        try {
            Directory.Delete(fullPath, true);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) {
            throw new InvalidOperationException($"Failed to delete output folder '{fullPath}': {ex.Message}", ex);
        }
    }

    private static List<string> BuildPublishArguments(
        PublishOptions options,
        string projectPath,
        string framework,
        string rid,
        string output,
        string? nativeArtifactsDir = null,
        string? customTargetsPath = null,
        bool isPreflight = false
    ) {
        List<string> args = [
            "publish",
            projectPath,
            "-c", options.Configuration,
            "-r", rid,
            "-f", framework,
            "--output", output,
            "-p:InfiniFramePackInvoked=true",
            $"-p:SelfContained={options.SelfContained.ToString().ToLowerInvariant()}",
            "-p:IncludeNativeLibrariesForSelfExtract=true",
            options.Verbose ? "-v:normal" : "-v:minimal"
        ];

        if (isPreflight) {
            args.Add("-p:PublishSingleFile=false");
        }
        else {
            args.AddRange([
                "-p:PublishSingleFile=true",
                "-p:IncludeAllContentForSelfExtract=true",
                "-p:EnableCompressionInSingleFile=true",
                "-p:DebugType=none",
                "-p:DebugSymbols=false",
                $"-p:InfiniFramePackRootProject={projectPath}",
                $"-p:InfiniFramePackRuntimeIdentifier={rid}",
                $"-p:InfiniFramePackNativeArtifactsDir={nativeArtifactsDir}",
                $"-p:CustomAfterMicrosoftCommonTargets={customTargetsPath}"
            ]);
        }

        if (options.NoRestore) args.Add("--no-restore");

        return args;
    }

    internal readonly record struct OutputShapeValidation(bool FoundMainOutput, string[] UnexpectedEntries);
}
