// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class WebView2EnvironmentGroup(WebView2EnvironmentKey key) : IDisposable {
    #if NET9_0_OR_GREATER
    private readonly Lock _lock = new();
    #else
    private readonly object _lock = new();
    #endif
    
    private int _referenceCount;
    private Exception? _initializationFailure;
    private bool _initialized;
    private WebView2RemoteDebuggingGroupReservation? _remoteDebuggingReservation;

    public WebView2EnvironmentKey Key { get; } = key;
    public int ReferenceCount => Volatile.Read(ref _referenceCount);

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void AddReference(Guid windowId) {
        int references = Interlocked.Increment(ref _referenceCount);
        if (references > 0) return;

        Interlocked.Decrement(ref _referenceCount);
        throw new InvalidOperationException($"Invalid WebView2 manager reference count for window {windowId}.");
    }

    public void InitializeOrThrow(Guid windowId) {
        lock (_lock) {
            if (_initializationFailure is not null) {
                throw new InvalidOperationException(
                    $"ManagedShared WebView2 environment initialization previously failed for key '{Key.Diagnostics}'.",
                    _initializationFailure);
            }

            if (_initialized) return;

            try {
                Directory.CreateDirectory(Key.ProfilePath);
                _initialized = true;
            }
            catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
                _initializationFailure = ex;
                throw new InvalidOperationException(
                    $"Failed to initialize ManagedShared WebView2 environment for window {windowId}. " +
                    $"Key: '{Key.Diagnostics}'.",
                    ex);
            }
        }
    }

    public void ReserveRemoteDebugging(int port, Guid windowId, ILogger logger) {
        if (port == 0) return;

        lock (_lock) {
            if (_remoteDebuggingReservation is not null) return;

            _remoteDebuggingReservation = WebView2WindowManager.SharedRemoteDebuggingReservations.AddOrUpdate(
                port,
                _ => {
                    RemoteDebuggingUtility.EnsureSupportedPlatform(port);
                    RemoteDebuggingUtility.ValidatePortAvailabilityOrThrow(port, logger);
                    return new WebView2RemoteDebuggingGroupReservation(Key, port);
                },
                (_, existing) => {
                    if (existing.Key.Equals(Key)) {
                        return existing;
                    }

                    throw new InvalidOperationException(
                        $"ManagedShared WebView2 remote debugging port {port} is already reserved by an incompatible environment. " +
                        $"Existing key: '{existing.Key.Diagnostics}'. Requested key: '{Key.Diagnostics}'.");
                });
        }
    }

    public void Release(Guid windowId) {
        int references = Interlocked.Decrement(ref _referenceCount);
        if (references >= 0) return;

        Interlocked.Exchange(ref _referenceCount, 0);
        throw new InvalidOperationException($"Invalid WebView2 manager release for window {windowId}.");
    }

    public void Dispose() {
        _remoteDebuggingReservation?.Dispose();
        _remoteDebuggingReservation = null;

        try {
            if (Directory.Exists(Key.ProfilePath)) Directory.Delete(Key.ProfilePath, recursive: true);
        }
        catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
            // Browser processes may release files shortly after the final controller is closed.
        }
    }
}
