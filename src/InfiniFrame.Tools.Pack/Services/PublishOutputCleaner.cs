// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class PublishOutputCleaner {
    public static void Cleanup(string output) {
        string wwwroot = Path.Combine(output, "wwwroot");
        if (Directory.Exists(wwwroot)) Directory.Delete(wwwroot, recursive: true);

        foreach (string file in NativeRuntimeBuilder.NativeRuntimeFiles) {
            string fullPath = Path.Combine(output, file);
            if (File.Exists(fullPath)) File.Delete(fullPath);
        }
    }
}
