// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

namespace InfiniFrame.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IconFileUtilities {
    public static string? ResolveIconFilePath([NotNullWhen(true)] string? filePath, string? baseDirectory = null) {
        if (string.IsNullOrWhiteSpace(filePath)) return null;

        string resolvedBaseDirectory = string.IsNullOrWhiteSpace(baseDirectory)
            ? AppContext.BaseDirectory
            : baseDirectory;

        // Resolve relative paths against AppContext.BaseDirectory first.
        string fromBaseDirectory = Path.GetFullPath(filePath, resolvedBaseDirectory);
        if (File.Exists(fromBaseDirectory)) return fromBaseDirectory;

        // Preserve support for absolute or current-directory-relative paths.
        string fromCurrentDirectory = Path.GetFullPath(filePath);
        return File.Exists(fromCurrentDirectory) 
            ? fromCurrentDirectory 
            : null;
    }
}
