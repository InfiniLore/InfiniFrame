// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;
using InfiniFrame.NativeBridge;
using InfiniFrame.NativeBridge.Handles;
using InfiniFrame.NativeBridge.Parameters;
using Microsoft.Extensions.Logging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Runtime implementation managing the native InfiniFrame application lifecycle including platform registration,
///     message loop execution, and window collection tracking.
/// </summary>
public sealed class InfiniFrameApplication(
    ILogger<InfiniFrameApplication> logger
) : IInfiniFrameApplication, IDisposable, IAsyncDisposable {
    private NativeApplicationHandle? _handle;
    private ApplicationConfiguration? _configuration;
    private int _disposed;

    /// <inheritdoc />
    public Guid Id { get; } = Guid.NewGuid();

    /// <inheritdoc />
    public IntPtr ApplicationHandle => _handle?.DangerousGetHandle() ?? IntPtr.Zero;

    /// <inheritdoc />
    public bool IsShutdownRequested { get; private set; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------

    /// <inheritdoc />
    public void Initialize(ApplicationConfiguration config) {
        ObjectDisposedException.ThrowIf(_disposed != 0, this);
        ArgumentNullException.ThrowIfNull(config);

        _configuration = config;

        var parameters = new ApplicationInitParameters {
            StructSize = Marshal.SizeOf<ApplicationInitParameters>()
        };

        // Marshal string parameters
        IntPtr appUserModelIdPtr = IntPtr.Zero;
        IntPtr notificationRegIdPtr = IntPtr.Zero;
        IntPtr webView2RuntimePathPtr = IntPtr.Zero;

        try {
            if (config.WindowsAppUserModelId is not null) {
                appUserModelIdPtr = Marshal.StringToHGlobalAnsi(config.WindowsAppUserModelId);
                parameters.WindowsAppUserModelId = appUserModelIdPtr;
            }

            if (config.NotificationRegistrationId is not null) {
                notificationRegIdPtr = Marshal.StringToHGlobalAnsi(config.NotificationRegistrationId);
                parameters.NotificationRegistrationId = notificationRegIdPtr;
            }

            if (config.WebView2RuntimePath is not null) {
                webView2RuntimePathPtr = Marshal.StringToHGlobalAnsi(config.WebView2RuntimePath);
                parameters.WebView2RuntimePath = webView2RuntimePathPtr;
            }

            IntPtr unmanagedPtr = Marshal.AllocHGlobal(Marshal.SizeOf<ApplicationInitParameters>());
            try {
                Marshal.StructureToPtr(parameters, unmanagedPtr, false);

                InfiniFrameNativeInteropStatus status =
                    InfiniFrameNative.ApplicationConstructor(unmanagedPtr, out IntPtr handle);
                if (status != InfiniFrameNativeInteropStatus.Success) {
                    int lastError = Marshal.GetLastPInvokeError();
                    string nativeMessage = InfiniFrameNative.GetLastErrorMessage() ?? "No native error message provided.";
                    throw new InfiniFrameNativeInteropException(
                        $"Application constructor failed with status {status}. Error #{lastError}. {nativeMessage}");
                }

                ArgumentOutOfRangeException.ThrowIfZero(handle);

                _handle = new NativeApplicationHandle(new IntPtr(handle));

                logger.LogInformation("Native application initialized successfully.");
            }
            finally {
                Marshal.FreeHGlobal(unmanagedPtr);
            }

            // Platform-specific registration
            if (OperatingSystem.IsWindows()) {
                if (config.HInstance == IntPtr.Zero)
                    throw new InvalidOperationException("HInstance is required on Windows.");

                InfiniFrameNativeInteropStatus regStatus =
                    InfiniFrameNative.ApplicationRegisterWin32(_handle.DangerousGetHandle(), config.HInstance);
                if (regStatus != InfiniFrameNativeInteropStatus.Success) {
                    int lastError = Marshal.GetLastPInvokeError();
                    string nativeMessage = InfiniFrameNative.GetLastErrorMessage() ?? "No native error message provided.";
                    throw new InfiniFrameNativeInteropException(
                        $"Win32 registration failed with status {regStatus}. Error #{lastError}. {nativeMessage}");
                }

                logger.LogDebug("Win32 platform registration completed.");
            }
            else if (OperatingSystem.IsMacOS()) {
                InfiniFrameNativeInteropStatus regStatus =
                    InfiniFrameNative.ApplicationRegisterMac(_handle.DangerousGetHandle());
                if (regStatus != InfiniFrameNativeInteropStatus.Success) {
                    int lastError = Marshal.GetLastPInvokeError();
                    string nativeMessage = InfiniFrameNative.GetLastErrorMessage() ?? "No native error message provided.";
                    throw new InfiniFrameNativeInteropException(
                        $"macOS registration failed with status {regStatus}. Error #{lastError}. {nativeMessage}");
                }

                logger.LogDebug("macOS platform registration completed.");
            }
            else if (OperatingSystem.IsLinux()) {
                logger.LogDebug("Linux GTK initialization handled natively.");
            }
            else {
                throw new PlatformNotSupportedException();
            }
        }
        finally {
            if (appUserModelIdPtr != IntPtr.Zero) Marshal.FreeHGlobal(appUserModelIdPtr);
            if (notificationRegIdPtr != IntPtr.Zero) Marshal.FreeHGlobal(notificationRegIdPtr);
            if (webView2RuntimePathPtr != IntPtr.Zero) Marshal.FreeHGlobal(webView2RuntimePathPtr);
        }
    }

    /// <inheritdoc />
    public void Run() {
        ObjectDisposedException.ThrowIf(_disposed != 0, this);
        if (_handle is null)
            throw new InvalidOperationException("Application has not been initialized. Call Initialize() first.");

        logger.LogDebug("Starting application message loop.");

        InfiniFrameNativeInteropStatus status =
            InfiniFrameNative.ApplicationRun(_handle.DangerousGetHandle());
        if (status != InfiniFrameNativeInteropStatus.Success) {
            int lastError = Marshal.GetLastPInvokeError();
            string nativeMessage = InfiniFrameNative.GetLastErrorMessage() ?? "No native error message provided.";
            throw new InfiniFrameNativeInteropException(
                $"Application run failed with status {status}. Error #{lastError}. {nativeMessage}");
        }

        logger.LogDebug("Application message loop exited.");
    }

    /// <inheritdoc />
    public async Task RunAsync(CancellationToken ct = default) {
        await Task.Run(() => Run(), ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Shutdown() {
        if (_disposed != 0 || _handle is null) return;

        IsShutdownRequested = true;

        logger.LogDebug("Signaling application shutdown.");

        InfiniFrameNativeInteropStatus status =
            InfiniFrameNative.ApplicationShutdown(_handle.DangerousGetHandle());
        if (status != InfiniFrameNativeInteropStatus.Success) {
            int lastError = Marshal.GetLastPInvokeError();
            string nativeMessage = InfiniFrameNative.GetLastErrorMessage() ?? "No native error message provided.";
            logger.LogWarning(
                "Application shutdown signal returned status {Status}. Error #{LastError}. {Message}",
                status, lastError, nativeMessage);
        }
    }

    /// <inheritdoc />
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync() {
        Dispose();
        await ValueTask.CompletedTask;
    }

    private void Dispose(bool disposing) {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

        if (disposing) {
            _handle?.Dispose();
            _handle = null;
            logger.LogDebug("Application disposed.");
        }
    }
}
