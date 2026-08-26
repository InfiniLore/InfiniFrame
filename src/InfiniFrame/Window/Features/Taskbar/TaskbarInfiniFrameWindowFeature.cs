// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics;
using InfiniFrame.NativeBridge;
using Microsoft.Extensions.Logging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides taskbar progress and flash functionality for the window.
///     On Windows, uses ITaskbarList3 and FlashWindowEx.
///     On macOS, uses NSDockTile badge and NSRequestUserAttention.
///     On Linux, uses D-Bus StatusNotifierItem or Unity LauncherEntry where available.
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
        SupportsFlash = OperatingSystem.IsWindows() || OperatingSystem.IsMacOS()
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

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.StopTaskbarFlash
        );
    }
}
