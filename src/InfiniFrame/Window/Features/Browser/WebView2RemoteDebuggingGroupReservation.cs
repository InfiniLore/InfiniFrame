// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class WebView2RemoteDebuggingGroupReservation(WebView2EnvironmentKey key, int port) : IDisposable {
    private int _disposed;

    public WebView2EnvironmentKey Key { get; } = key;

    public void Dispose() {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

        WebView2WindowManager.SharedRemoteDebuggingReservations.TryRemove(
            new KeyValuePair<int, WebView2RemoteDebuggingGroupReservation>(port, this)
        );
    }
}
