// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class WebView2ProcessInitializationLease : IDisposable {
    private readonly bool _processGateAcquired;
    private int _disposed;

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    private WebView2ProcessInitializationLease(bool processGateAcquired) {
        _processGateAcquired = processGateAcquired;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static WebView2ProcessInitializationLease Acquire() {
        bool processGateAcquired = false;
        try {
            WebView2WindowManager.InitializationProcessGate.WaitOne();
            processGateAcquired = true;
            return new WebView2ProcessInitializationLease(processGateAcquired);
        }
        catch (AbandonedMutexException) {
            processGateAcquired = true;
            return new WebView2ProcessInitializationLease(processGateAcquired);
        }
        catch {
            if (processGateAcquired) WebView2WindowManager.InitializationProcessGate.ReleaseMutex();
            throw;
        }
    }

    public void Dispose() {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
        if (_processGateAcquired) WebView2WindowManager.InitializationProcessGate.ReleaseMutex();
    }
}
