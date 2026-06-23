// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text;

namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class PublishOutputCleaner {
    private const int MaxDeleteAttempts = 3;

    /// <summary>
    ///     The native runtime file names that are stripped from the final publication output after embedding.
    /// </summary>
    public static readonly string[] NativeRuntimeFiles = InfiniFramePackNativeArtifactManifest.AllFileNames;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Removes unpacked runtime artifacts that should not remain beside the single-file executable.
    /// </summary>
    /// <param name="output">Publish output directory.</param>
    /// <returns>Non-fatal cleanup warnings.</returns>
    public static string[] Cleanup(string output) {
        List<string> warnings = [];

        string wwwroot = Path.Combine(output, "wwwroot");
        if (Directory.Exists(wwwroot)) {
            string? warning = TryDeleteDirectoryWithRetries(wwwroot);
            if (!string.IsNullOrWhiteSpace(warning)) warnings.Add(warning);
        }

        IEnumerable<string?> enumerable = NativeRuntimeFiles
            .Select(file => Path.IsPathRooted(file) ? file : Path.Combine(output, file))
            .Where(File.Exists)
            .Select(TryDeleteFileWithRetries)
            .Where(warning => !string.IsNullOrWhiteSpace(warning));

        warnings.AddRange(enumerable!);

        return warnings.ToArray();
    }

    private static string? TryDeleteDirectoryWithRetries(string directoryPath) {
        for (int attempt = 1; attempt <= MaxDeleteAttempts; attempt++) {
            try {
                if (Directory.Exists(directoryPath)) Directory.Delete(directoryPath, true);
                return null;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) {
                if (attempt == MaxDeleteAttempts) {
                    return BuildFailureMessage("directory", directoryPath, attempt, ex);
                }

                Thread.Sleep(50 * attempt);
            }
        }

        return null;
    }

    private static string? TryDeleteFileWithRetries(string filePath) {
        for (int attempt = 1; attempt <= MaxDeleteAttempts; attempt++) {
            try {
                if (File.Exists(filePath)) File.Delete(filePath);
                return null;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) {
                if (attempt == MaxDeleteAttempts) {
                    return BuildFailureMessage("file", filePath, attempt, ex);
                }

                Thread.Sleep(50 * attempt);
            }
        }

        return null;
    }

    private static string BuildFailureMessage(string targetType, string path, int attempts, Exception ex) {
        var builder = new StringBuilder();
        builder.Append("Cleanup skipped ");
        builder.Append(targetType);
        builder.Append(" '");
        builder.Append(path);
        builder.Append("' after ");
        builder.Append(attempts);
        builder.Append(" attempts: ");
        builder.Append(ex.Message);
        return builder.ToString();
    }
}
