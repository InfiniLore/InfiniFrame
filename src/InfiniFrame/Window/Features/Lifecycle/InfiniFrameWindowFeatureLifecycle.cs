// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using FluentValidation;
using InfiniFrame.NativeBridge;
using InfiniFrame.NativeBridge.Parameters;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowFeatureLifecycle(
    IInfiniFrameWindow window,
    ILogger<InfiniFrameWindowFeatureLifecycle> logger,
    IValidator<InfiniFrameNativeParameters> validator
) : IInfiniFrameWindowFeatureLifecycle {
    private enum LifecycleStatus {
        Undefined = 0,
        Closing = 1,
        Closed = 2
    }
    
    private int _lifecycleState = (int)LifecycleStatus.Undefined;

    private LifecycleStatus LifecycleState {
        get => (LifecycleStatus)Volatile.Read(ref _lifecycleState);
        set => Volatile.Write(ref _lifecycleState, (int)value);
    }

    // Holds the native handle after MarkAsClosed zeros InstanceHandle but before Dispose frees it.
    private IntPtr _cleanupHandle = IntPtr.Zero;

    /// <inheritdoc cref="IInfiniFrameWindowFeatureLifecycle.CleanupNativeHandle"/>
    void IInfiniFrameWindowFeatureLifecycle.CleanupNativeHandle() {
        IntPtr handle = Interlocked.Exchange(ref _cleanupHandle, IntPtr.Zero);

        try {
            if (handle != IntPtr.Zero) InfiniFrameNative.Destructor(handle);
        }
        finally {
            window.Events.ReleaseNativeCallbackRoot();
        }
    }
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="InfiniFrameWindowFeatureLifecycle.Initialize"/>
    internal void Initialize() => window.Features.Lifecycle.Initialize();

    /// <inheritdoc cref="IInfiniFrameWindowFeatureLifecycle.Initialize"/>
    void IInfiniFrameWindowFeatureLifecycle.Initialize() {
        InfiniFrameNativeParameters startupParameters = window.Configuration.StartupParameters;
        bool webInspectorEnabled = startupParameters.WebInspectorEnabled;

        try {
            if (startupParameters.RemoteDebuggingPort != 0) {
                logger.LogInformation(
                    "Remote debugging requested on loopback port {RemoteDebuggingPort}.",
                    startupParameters.RemoteDebuggingPort);

                if (OperatingSystem.IsLinux() && !startupParameters.DevToolsEnabled) {
                    logger.LogInformation(
                        "Linux remote debugging keeps WebKit developer extras enabled while active."
                    );
                }
            }
            else {
                logger.LogDebug("Remote debugging is disabled.");
            }

            RemoteDebuggingUtility.EnsureSupportedPlatform(startupParameters.RemoteDebuggingPort);
            RemoteDebuggingUtility.ValidatePortAvailabilityOrThrow(startupParameters.RemoteDebuggingPort, logger);
            if (webInspectorEnabled) {
                MacOsWebInspectorUtility.ThrowIfUnsupported();
            }

            validator.ValidateAndThrow(startupParameters);
            
            window.Events.OnWindowCreating();

            try {
                if (OperatingSystem.IsWindows()) {
                    InfiniFrameNative.RegisterWin32(window.MainProgramHandle);
                }
                else if (OperatingSystem.IsMacOS()) {
                    InfiniFrameNativeInteropStatus registerStatus = InfiniFrameNative.RegisterMac();
                    if (registerStatus != InfiniFrameNativeInteropStatus.Success) {
                        int lastError = Marshal.GetLastPInvokeError();
                        string nativeMessage = InfiniFrameNative.GetLastErrorMessage() ?? "No native error message provided.";
                        throw new ApplicationException(
                            $"Native registration failed with status {registerStatus}. Error #{lastError}. {nativeMessage}");
                    }
                }
                else if (OperatingSystem.IsLinux()) {} // No specific implementation for Linux
                else throw new PlatformNotSupportedException();

                InfiniFrameNativeInteropStatus status = InfiniFrameNative.Constructor(in startupParameters, out IntPtr handle);
                if (status != InfiniFrameNativeInteropStatus.Success) {
                    int lastError = Marshal.GetLastPInvokeError();
                    string nativeMessage = InfiniFrameNative.GetLastErrorMessage() ?? "No native error message provided.";
                    
                    throw new ApplicationException(
                        $"Native constructor failed with status {status}. Error #{lastError}. {nativeMessage}");
                }
                
                ArgumentOutOfRangeException.ThrowIfZero(handle);
                window.InstanceHandle = handle;
            }
            catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
                int lastError = Marshal.GetLastPInvokeError();

                logger.LogError(ex, "Error #{LastErrorCode} while creating native window", lastError);
                throw new ApplicationException($"Native code exception. Error #{lastError}", ex);
            }

            window.Events.OnWindowCreated();
        }
        finally {
            CustomSchemeNameMemory.FreeAll(startupParameters.CustomSchemeNames);
        }
    }

    /// <inheritdoc cref="IInfiniFrameWindowFeatureLifecycle.WaitForClose"/>
    public void WaitForClose() {
        if (IsClosedOrClosing()) {
            logger.LogDebug("Skipping WaitForClose during shutdown");
            return;
        }

        try {
            logger.LogDebug("Starting message loop for window.");
            window.Features.Invoke.Invoke(() => {
                if (IsClosedOrClosing()) {
                    logger.LogDebug("Lifecycle already started whilst dispatching to window thread. Skipping WaitForExit call.");
                    return;
                }
                
                InfiniFrameNative.WaitForExit(window.InstanceHandle);
            });
        }
        catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
            int lastError = Marshal.GetLastPInvokeError();

            logger.LogError(ex, "Error #{LastErrorCode} while running message loop", lastError);
            throw new ApplicationException(
                $"Native code exception. Error #{lastError}",
                ex);
        }
        finally {
            MarkAsClosed();
        }
    }

    /// <inheritdoc cref="IInfiniFrameWindowFeatureLifecycle.WaitForCloseAsync"/>
    public ValueTask WaitForCloseAsync(CancellationToken ct = default) {
        if (ct.IsCancellationRequested || IsClosedOrClosing())
            return ValueTask.FromCanceled(ct);

        WaitForClose();
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc cref="IInfiniFrameWindowFeatureLifecycle.Close"/>
    public void Close() {
        if (Interlocked.Exchange(ref _lifecycleState, 1) != 0) {
            logger.LogDebug("Skipping Close during shutdown");
            return;
        }

        logger.LogDebug(".Close()");
        window.Events.OnWindowClosingRequested();

        IntPtr handle = window.InstanceHandle;
        if (handle == IntPtr.Zero) {
            logger.LogDebug("Skipping Close because window is not initialized");
            return;
        }

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle, 
            window.ManagedThreadId,
            InfiniFrameNative.Close
        );
        MarkAsClosed();
    }

    /// <inheritdoc cref="IInfiniFrameWindowFeatureLifecycle.CloseAsync"/>
    public ValueTask CloseAsync(CancellationToken ct = default) {
        if (ct.IsCancellationRequested)
            return ValueTask.FromCanceled(ct);

        Close();
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc cref="InfiniFrameWindowFeatureLifecycle.MarkAsClosed"/>
    internal void MarkAsClosed() => window.Features.Lifecycle.MarkAsClosed();

    /// <inheritdoc cref="IInfiniFrameWindowFeatureLifecycle.MarkAsClosed"/>
    void IInfiniFrameWindowFeatureLifecycle.MarkAsClosed() {
        IntPtr handle = window.InstanceHandle;
        window.InstanceHandle = IntPtr.Zero;
        LifecycleState = LifecycleStatus.Closed;

        if (OperatingSystem.IsLinux()) {
            // Destructor is intentionally NOT called here — MarkAsClosed runs inside the GTK "destroy" signal handler.
            // Calling InfiniFrameNative.Destructor from inside a GTK signal handler triggers a SIGABRT in WebKit or a
            // deadlock when the next WebKitWebView is created. The native object is freed later via CleanupNativeHandle.
            _cleanupHandle = handle;
        }
    }

    /// <inheritdoc cref="IInfiniFrameWindowFeatureLifecycle.IsClosedOrClosing"/>
    public bool IsClosedOrClosing() {
        if (LifecycleState is LifecycleStatus.Closed or LifecycleStatus.Closing) return true;
        return window.InstanceHandle == IntPtr.Zero;
    }
}
