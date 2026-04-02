// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class PublishOutputCleaner {
    /// <summary>
    ///     Removes unpacked runtime artifacts that should not remain beside the single-file executable.
    /// </summary>
    /// <param name="output">Publish output directory.</param>
    public static void Cleanup(string output) {
        string wwwroot = Path.Combine(output, "wwwroot");
        if (Directory.Exists(wwwroot)) Directory.Delete(wwwroot, true);

        foreach (string file in NativeRuntimeBuilder.NativeRuntimeFiles) {
            string fullPath = Path.Combine(output, file);
            if (File.Exists(fullPath)) File.Delete(fullPath);
        }
    }
}
