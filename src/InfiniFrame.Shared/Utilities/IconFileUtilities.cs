// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

namespace InfiniFrame.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IconFileUtilities {
    public static bool TryResolveIconFilePath(
        string? filePath,
        [NotNullWhen(true)] out string? resolvedFilePath,
        string? baseDirectory = null
    ) {
        resolvedFilePath = null;
        if (string.IsNullOrWhiteSpace(filePath)) return false;

        string resolvedBaseDirectory = string.IsNullOrWhiteSpace(baseDirectory)
            ? AppContext.BaseDirectory
            : baseDirectory;

        // Resolve relative paths against AppContext.BaseDirectory first.
        string fromBaseDirectory = Path.GetFullPath(filePath, resolvedBaseDirectory);
        if (File.Exists(fromBaseDirectory)) {
            resolvedFilePath = fromBaseDirectory;
            return true;
        }

        // Preserve support for absolute or current-directory-relative paths.
        string fromCurrentDirectory = Path.GetFullPath(filePath);
        if (!File.Exists(fromCurrentDirectory)) return false;

        resolvedFilePath = fromCurrentDirectory;
        return true;

    }
}
