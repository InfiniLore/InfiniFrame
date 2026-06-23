// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class WebView2IsolatedInitializationLease : IDisposable {
    private readonly bool _processGateAcquired;
    private int _disposed;

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------    
    private WebView2IsolatedInitializationLease(bool processGateAcquired) {
        _processGateAcquired = processGateAcquired;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static WebView2IsolatedInitializationLease Acquire() {
        WebView2WindowManager.IsolatedInitializationGate.Wait();
        bool processGateAcquired = false;
        try {
            WebView2WindowManager.IsolatedInitializationProcessGate.WaitOne();
            processGateAcquired = true;
            return new WebView2IsolatedInitializationLease(processGateAcquired);
        }
        catch (AbandonedMutexException) {
            processGateAcquired = true;
            return new WebView2IsolatedInitializationLease(processGateAcquired);
        }
        catch {
            if (processGateAcquired) WebView2WindowManager.IsolatedInitializationProcessGate.ReleaseMutex();
            WebView2WindowManager.IsolatedInitializationGate.Release();
            throw;
        }
    }

    public void Dispose() {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
        if (_processGateAcquired) WebView2WindowManager.IsolatedInitializationProcessGate.ReleaseMutex();
        WebView2WindowManager.IsolatedInitializationGate.Release();
    }
}
