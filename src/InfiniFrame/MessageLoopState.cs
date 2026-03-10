// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
/// There can only be 1 message loop for all windows.
/// </summary>
internal static class MessageLoopState {
    #if NET9_0_OR_GREATER
    private static readonly Lock Lock = new();
    #else
    private static readonly object Lock = new();
    #endif

    private static bool IsStarted { get; set; }

    public static bool TryAcquireFirstState() {
        lock (Lock) {
            if (IsStarted) return false;

            IsStarted = true;
            return true;
        }
    }

    public static void ReleaseState() {
        lock (Lock) {
            IsStarted = false;
        }
    }
}
