// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides taskbar progress and flash functionality for the window.
///     Currently supported on Windows only via ITaskbarList3 and FlashWindowEx.
///     On Linux and macOS, all methods throw <see cref="PlatformNotSupportedException"/>.
/// </summary>
public sealed class TaskbarInfiniFrameWindowFeature(
    IInfiniFrameWindow window,
    ILogger<TaskbarInfiniFrameWindowFeature> logger
) : ITaskbarInfiniFrameWindowFeature {

    private bool? _isSupported;

    /// <inheritdoc cref="ITaskbarInfiniFrameWindowFeature.IsSupported" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsSupported {
        get {
            if (_isSupported.HasValue) return _isSupported.Value;

            if (!OperatingSystem.IsWindows()) {
                _isSupported = false;
                return false;
            }

            _isSupported = NativeInvoke.InvokeSyncWithValidation<bool>(
                logger,
                window,
                window.ManagedThreadId,
                InfiniFrameNative.GetTaskbarProgressSupported
            );
            return _isSupported.Value;
        }
    }

    /// <inheritdoc cref="ITaskbarInfiniFrameWindowFeature.Capabilities" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public InfiniFrameTaskbarCapabilities Capabilities => new() {
        SupportsProgress = IsSupported,
        SupportsFlash = OperatingSystem.IsWindows()
    };

    /// <inheritdoc cref="ITaskbarInfiniFrameWindowFeature.CurrentProgressState" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public TaskbarProgressState CurrentProgressState { get; private set; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="ITaskbarInfiniFrameWindowFeature.SetProgress" />
    public void SetProgress(TaskbarProgressState state, ulong current, ulong total) {
        logger.LogDebug(".SetTaskbarProgress({State}, {Current}, {Total})", state, current, total);

        if (!OperatingSystem.IsWindows()) {
            throw new PlatformNotSupportedException("Taskbar progress is only supported on Windows.");
        }

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetTaskbarProgress,
            (int)state,
            current,
            total
        );
        CurrentProgressState = state;
    }

    /// <inheritdoc cref="ITaskbarInfiniFrameWindowFeature.ClearProgress" />
    public void ClearProgress() {
        logger.LogDebug(".ClearTaskbarProgress()");

        if (!OperatingSystem.IsWindows()) {
            throw new PlatformNotSupportedException("Taskbar progress is only supported on Windows.");
        }

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.ClearTaskbarProgress
        );
        CurrentProgressState = TaskbarProgressState.None;
    }

    /// <inheritdoc cref="ITaskbarInfiniFrameWindowFeature.SetFlash" />
    public void SetFlash(TaskbarFlashMode mode, uint count) {
        logger.LogDebug(".SetTaskbarFlash({Mode}, {Count})", mode, count);

        if (!OperatingSystem.IsWindows()) {
            throw new PlatformNotSupportedException("Taskbar flash is only supported on Windows.");
        }

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetTaskbarFlash,
            (int)mode,
            count
        );
    }

    /// <inheritdoc cref="ITaskbarInfiniFrameWindowFeature.StopFlash" />
    public void StopFlash() {
        logger.LogDebug(".StopTaskbarFlash()");

        if (!OperatingSystem.IsWindows()) {
            throw new PlatformNotSupportedException("Taskbar flash is only supported on Windows.");
        }

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.StopTaskbarFlash
        );
    }
}
