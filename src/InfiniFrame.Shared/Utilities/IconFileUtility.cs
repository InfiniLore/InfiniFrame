// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

namespace InfiniFrame.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides utility methods for resolving icon file paths.
/// </summary>
internal static class IconFileUtility {
    /// <summary>
    ///     Attempts to resolve the full path of an icon file, checking the base directory and current directory.
    /// </summary>
    /// <param name="filePath">The relative or absolute file path to resolve.</param>
    /// <param name="resolvedFilePath">When this method returns, contains the resolved full path if the file exists.</param>
    /// <param name="baseDirectory">The base directory to use for relative path resolution. If null, <see cref="AppContext.BaseDirectory" /> is used.</param>
    /// <returns><c>true</c> if the icon file was found; otherwise, <c>false</c>.</returns>
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
