// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class InfiniFrameWebMessageContext {
    private static readonly AsyncLocal<string?> CurrentOriginHolder = new();

    public static string? CurrentOrigin => CurrentOriginHolder.Value;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static IDisposable Push(string? origin) {
        string? previous = CurrentOriginHolder.Value;
        CurrentOriginHolder.Value = origin;
        return new Scope(previous);
    }

    private sealed class Scope(string? previous) : IDisposable {
        private bool _disposed;

        public void Dispose() {
            if (_disposed) return;

            _disposed = true;
            CurrentOriginHolder.Value = previous;
        }
    }
}
