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
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="InfiniFrameWindowFeatureLifecycle.Initialize"/>
    internal void Initialize() => window.Features.Lifecycle.Initialize();

    /// <summary>
    /// Performs the initialization process for an InfiniFrame window feature lifecycle. This includes validating
    /// and configuring startup parameters, setting up debugging capabilities, and invoking specific lifecycle events.
    /// </summary>
    /// <remarks>
    /// The initialization includes validation of platform compatibility and port availability for remote debugging.
    /// It also ensures the proper handling of platform-specific requirements for features such as the Web Inspector.
    /// This method prepares the environment before notifying event hooks for the window's creation lifecycle.
    /// </remarks>
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
                if (OperatingSystem.IsWindows()) InfiniFrameNative.RegisterWin32(window.MainProgramHandle);
                else if (OperatingSystem.IsMacOS()) InfiniFrameNative.RegisterMac();
                else if (OperatingSystem.IsLinux()) {} // No specific implementation for Linux
                else throw new PlatformNotSupportedException();

                InfiniFrameNative.Constructor(in startupParameters, out IntPtr handle);
                ArgumentOutOfRangeException.ThrowIfZero(handle);
                window.InstanceHandle = handle;
            }
            catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
                int lastError = OperatingSystem.IsWindows()
                    ? Marshal.GetLastWin32Error()
                    : 0;

                logger.LogError(ex, "Error #{LastErrorCode} while creating native window", lastError);
                throw new ApplicationException($"Native code exception. Error #{lastError}", ex);
            }

            window.Events.OnWindowCreated();
        }
        finally {
            CustomSchemeNameMemory.FreeAll(startupParameters.CustomSchemeNames);
        }
    }

    /// <summary>
    /// Waits for the associated window to close and ensures the proper handling of the window's lifecycle.
    /// Maintains the operation of the window's message loop until the window is marked as closed.
    /// </summary>
    /// <remarks>
    /// If the window is already in the process of closing or has been closed, this method will exit without processing.
    /// This method encapsulates error handling for non-fatal exceptions during message loop execution and logs relevant details.
    /// </remarks>
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
            int lastError = OperatingSystem.IsWindows()
                ? Marshal.GetLastWin32Error()
                : 0;

            logger.LogError(ex, "Error #{LastErrorCode} while running message loop", lastError);
            throw new ApplicationException(
                $"Native code exception. Error # {lastError}",
                ex);
        }
        finally {
            MarkAsClosed();
        }
    }

    /// <summary>
    /// Asynchronously waits for the primary native window to close.
    /// This method allows for cancellation and ensures that the task is completed
    /// when the window is closed or when cancellation is requested.
    /// </summary>
    /// <param name="ct">
    /// A <see cref="CancellationToken"/> to observe while waiting for the close operation.
    /// If cancellation is requested, the task will complete immediately with the cancellation status.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask"/> representing the asynchronous operation. If the window is already
    /// closed or closing, or if cancellation is requested, the task completes immediately.
    /// </returns>
    public ValueTask WaitForCloseAsync(CancellationToken ct = default) {
        if (ct.IsCancellationRequested || IsClosedOrClosing())
            return ValueTask.FromCanceled(ct);

        WaitForClose();
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Closes the native window and triggers the necessary lifecycle updates.
    /// </summary>
    /// <remarks>
    /// This operation ensures the proper handling of the window's state during the close process.
    /// If the window is not initialized, the close operation is skipped, and relevant log entries are made.
    /// Additionally, lifecycle state transitions are managed to avoid duplicate or conflicting close requests.
    /// </remarks>
    /// <exception cref="ApplicationException">
    /// Thrown when the close operation encounters an uninitialized window.
    /// </exception>
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

    /// <summary>
    /// Initiates the asynchronous closure process for the current window instance.
    /// This includes releasing associated resources and finalizing operations.
    /// </summary>
    /// <param name="ct">
    /// A <see cref="CancellationToken"/> that can be used to cancel the close operation, if necessary.
    /// </param>
    /// <return>
    /// A <see cref="ValueTask"/> that represents the asynchronous operation of closing the window instance.
    /// </return>
    public ValueTask CloseAsync(CancellationToken ct = default) {
        if (ct.IsCancellationRequested)
            return ValueTask.FromCanceled(ct);

        Close();
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc cref="InfiniFrameWindowFeatureLifecycle.MarkAsClosed"/>
    internal void MarkAsClosed() => window.Features.Lifecycle.MarkAsClosed();

    /// <summary>
    /// Marks the current window's lifecycle state as closed and updates the instance handle of the associated window
    /// to an invalid state.
    /// </summary>
    /// <remarks>
    /// This method is used internally to ensure that the lifecycle status of the window is properly transitioned
    /// to the closed state. It sets the window instance handle to <see cref="IntPtr.Zero"/> and changes the lifecycle
    /// state to indicate the closed state.
    /// </remarks>
    void IInfiniFrameWindowFeatureLifecycle.MarkAsClosed() {
        window.InstanceHandle = IntPtr.Zero;
        LifecycleState = LifecycleStatus.Closed;
    }

    /// <summary>
    /// Determines whether the current window instance is in the process of closing or has already been closed.
    /// </summary>
    /// <returns>
    /// A boolean value indicating if the window's lifecycle state is either closing or closed, or if the associated
    /// instance handle is no longer valid.
    /// </returns>
    public bool IsClosedOrClosing() {
        if (LifecycleState is LifecycleStatus.Closed or LifecycleStatus.Closing) return true;
        return window.InstanceHandle == IntPtr.Zero;
    }
}