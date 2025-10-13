// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniLore.Photino.NET;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
//There can only be 1 message loop for all windows.
internal static class MessageLoopState {
    private static readonly Lock Lock = new Lock();
    private static bool IsStarted { get; set; }
    
    public static bool TryAcquireFirstState() {
        lock (Lock) {
            if (IsStarted) return false;
            IsStarted = true;
            return true;
        }
    }
}
