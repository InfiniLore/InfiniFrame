// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Concurrent;

namespace InfiniFrame.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class BrowserProfileUtility {
    private static readonly ConcurrentDictionary<Guid, string> AutoProfilePaths = new();

    public static void RegisterAutoProfilePath(Guid windowId, string? path) {
        if (string.IsNullOrWhiteSpace(path)) return;

        AutoProfilePaths[windowId] = path;
    }

    public static void CleanupAutoProfilePath(Guid windowId) {
        if (!AutoProfilePaths.TryRemove(windowId, out string? path)) return;

        try {
            if (Directory.Exists(path)) Directory.Delete(path, recursive: true);
        }
        catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
            // Best-effort cleanup: browser runtimes may release profile files after managed teardown returns.
        }
    }
}
