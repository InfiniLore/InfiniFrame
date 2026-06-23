// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class WebView2EnvironmentGroupStartupLease(
    WebView2EnvironmentGroup group,
    Guid windowId
) : IDisposable {
    private int _disposed;

    public void Dispose() {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

        group.ReleaseStartupLease(windowId);
    }
}
