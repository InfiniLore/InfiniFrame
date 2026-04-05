// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class OutputPathSafety {
    public static void EnsureOutputCanBeDeleted(string outputPath, string projectDirectory, bool forceCleanOutput) {
        string fullPath = Path.GetFullPath(outputPath);
        if (string.IsNullOrWhiteSpace(fullPath)) throw new InvalidOperationException("Cannot delete an empty path.");

        string? root = Path.GetPathRoot(fullPath);
        if (string.Equals(fullPath, root, StringComparison.OrdinalIgnoreCase)) {
            throw new InvalidOperationException($"Refusing to delete root directory '{fullPath}'.");
        }

        string projectBinDirectory = Path.GetFullPath(Path.Join(projectDirectory, "bin"));
        if (IsUnderDirectory(fullPath, projectBinDirectory)) return;

        if (!forceCleanOutput) {
            throw new InvalidOperationException(
                $"Refusing to delete non-default output directory '{fullPath}'. " +
                "Pass --force-clean-output to allow this."
            );
        }
    }

    private static bool IsUnderDirectory(string candidatePath, string parentPath) {
        string normalizedCandidate = EnsureTrailingSeparator(Path.GetFullPath(candidatePath));
        string normalizedParent = EnsureTrailingSeparator(Path.GetFullPath(parentPath));
        return normalizedCandidate.StartsWith(normalizedParent, StringComparison.OrdinalIgnoreCase);
    }

    private static string EnsureTrailingSeparator(string path) =>
        path.EndsWith(Path.DirectorySeparatorChar) || path.EndsWith(Path.AltDirectorySeparatorChar)
            ? path
            : path + Path.DirectorySeparatorChar;
}
