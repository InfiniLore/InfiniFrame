// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class FileUtility {
    /// <summary>
    /// Safely deletes a directory and all contents with retry support.
    /// </summary>
    /// <param name="path">Directory path to delete.</param>
    /// <param name="maxRetries">Maximum retry attempts.</param>
    /// <param name="delayMs">Delay between retries in milliseconds.</param>
    /// <returns>True if deleted or already missing; otherwise false.</returns>
    public static bool SafeDeleteDirectory(
        string path,
        int maxRetries = 5,
        int delayMs = 250
    ) {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Path cannot be null or empty.", nameof(path));
        if (!Directory.Exists(path)) return true;

        for (int attempt = 1; attempt <= maxRetries; attempt++) {
            try {
                // Remove readonly attributes recursively
                ClearReadOnlyAttributes(path);

                Directory.Delete(path, recursive: true);

                // Verify deletion
                if (!Directory.Exists(path)) return true;
            }
            catch (IOException) {
                // File may still be in use
            }
            catch (UnauthorizedAccessException) {
                // Permissions or locked files
            }

            // Wait before retrying
            if (attempt < maxRetries)
                Thread.Sleep(delayMs);
        }

        return !Directory.Exists(path);
    }

    private static void ClearReadOnlyAttributes(string path) {
        foreach (string file in Directory.GetFiles(path, "*", SearchOption.AllDirectories)) {
            try {
                File.SetAttributes(file, FileAttributes.Normal);
            }
            catch {
                // Ignore attribute failures
            }
        }
    }
}
