// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class PublishValidator {

    /// <summary>
    ///     Runs all preflight validation checks before publish.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when any validation step fails.
    /// </exception>
    public static void PreflightValidate(
        string projectDirectory,
        string outputPath,
        string rid,
        string nativeArtifactsDir,
        bool forceCleanOutput
    ) {
        PublishValidationHelpers.ValidateRidConsistency(rid);
        PublishValidationHelpers.ValidateOutputPath(projectDirectory, outputPath, forceCleanOutput);
        ValidateNativeArtifacts(nativeArtifactsDir, rid);
    }

    public static void ValidateNativeArtifacts(
        string nativeArtifactsDir,
        string rid
    ) {
        if (!Directory.Exists(nativeArtifactsDir)) throw new InvalidOperationException($"Native artifacts directory was not found: {nativeArtifactsDir}");

        string[] requiredPaths = InfiniFramePackNativeArtifactManifest.RequiredFileNamesForRid(rid)
            .Select(file => Path.IsPathRooted(file) ? file : Path.Join(nativeArtifactsDir, file))
            .ToArray();

        string? missingPath = requiredPaths.FirstOrDefault(path => !File.Exists(path));
        if (missingPath is not null) {
            throw new InvalidOperationException($"Required native artifact was not found: {missingPath}");
        }

        if (!rid.StartsWith("win-", StringComparison.OrdinalIgnoreCase)) return;

        ushort expectedMachine = PublishValidationHelpers.ExpectedPeMachineForRid(rid);
        foreach (string path in requiredPaths) {
            using FileStream stream = File.OpenRead(path);
            ushort actualMachine = PublishValidationHelpers.ReadPeMachineFromStream(stream);
            if (actualMachine == expectedMachine) continue;

            throw new InvalidOperationException(
                $"Native artifact architecture mismatch for '{path}'. " +
                $"Expected {PublishValidationHelpers.DescribePeMachine(expectedMachine)} for RID '{rid}', found {PublishValidationHelpers.DescribePeMachine(actualMachine)}."
            );
        }
    }
}
