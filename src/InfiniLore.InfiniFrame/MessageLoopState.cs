// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniLore.InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
/// There can only be 1 message loop for all windows.
/// </summary>
internal static class MessageLoopState {
    private static readonly Lock Lock = new();
    private static bool IsStarted { get; set; }

    public static bool TryAcquireFirstState() {
        lock (Lock) {
            if (IsStarted) return false;

            IsStarted = true;
            return true;
        }
    }
}
