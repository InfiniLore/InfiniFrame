// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class OutputPathSafety {
    private static readonly StringComparison PathComparison =
        OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

    public static bool EnsureOutputCanBeDeleted(string outputPath, string projectDirectory, bool forceCleanOutput) {
        string fullPath = Path.GetFullPath(outputPath);
        if (string.IsNullOrWhiteSpace(fullPath)) throw new InvalidOperationException("Cannot delete an empty path.");

        string? root = Path.GetPathRoot(fullPath);
        if (string.Equals(fullPath, root, PathComparison)) {
            throw new InvalidOperationException($"Refusing to delete root directory '{fullPath}'.");
        }

        string projectBinDirectory = Path.GetFullPath(Path.Join(projectDirectory, "bin"));
        if (IsUnderDirectory(fullPath, projectBinDirectory)) return true;

        if (!forceCleanOutput) {
            throw new InvalidOperationException(
                $"Refusing to delete non-default output directory '{fullPath}'. " +
                "Pass --force-clean-output to allow this."
            );
        }
        
        return true;
    }

    private static bool IsUnderDirectory(string candidatePath, string parentPath) {
        string normalizedCandidate = EnsureTrailingSeparator(Path.GetFullPath(candidatePath));
        string normalizedParent = EnsureTrailingSeparator(Path.GetFullPath(parentPath));
        return normalizedCandidate.StartsWith(normalizedParent, PathComparison);
    }

    private static string EnsureTrailingSeparator(string path) =>
        path.EndsWith(Path.DirectorySeparatorChar) || path.EndsWith(Path.AltDirectorySeparatorChar)
            ? path
            : path + Path.DirectorySeparatorChar;
}
