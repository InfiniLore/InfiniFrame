// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowFeatureBrowser(
    IInfiniFrameWindow window,
    ILogger<InfiniFrameWindowFeatureBrowser> logger
) : IInfiniFrameWindowFeatureBrowser {

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsContextMenuEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        InfiniFrameNative.GetContextMenuEnabled
    );

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsMediaAutoplayEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        InfiniFrameNative.GetMediaAutoplayEnabled
    );

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string? UserAgent => NativeInvoke.InvokeSyncWithValidation<string?>(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        InfiniFrameNative.GetUserAgent);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsFileSystemAccessEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        InfiniFrameNative.GetFileSystemAccessEnabled
    );

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsWebSecurityEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        InfiniFrameNative.GetWebSecurityEnabled
    );

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsJavascriptClipboardAccessEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        InfiniFrameNative.GetJavascriptClipboardAccessEnabled
    );

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsMediaStreamEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        InfiniFrameNative.GetMediaStreamEnabled
    );

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsIgnoreCertificateErrorsEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window.InstanceHandle, 
        window.ManagedThreadId,
        InfiniFrameNative.GetIgnoreCertificateErrorsEnabled
    );

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool GrantBrowserPermissions => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        InfiniFrameNative.GetGrantBrowserPermissions
    );

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsSmoothScrollingEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window.InstanceHandle, 
        window.ManagedThreadId,
        InfiniFrameNative.GetSmoothScrollingEnabled
    );
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public IInfiniFrameWindow SetContextMenuEnabled(bool enabled) {
        logger.LogDebug(".SetContextMenuEnabled({Enabled})", enabled);

        window.Invoke(() => {
            InfiniFrameNative.GetContextMenuEnabled(window.InstanceHandle, out bool isEnabled);
            if (isEnabled == enabled) {
                return;
            }

            InfiniFrameNative.SetContextMenuEnabled(window.InstanceHandle, enabled);
        });

        return window;
    }

    [SupportedOSPlatform("windows")]
    public IInfiniFrameWindow Win32SetWebView2Path(string data) {
        if (!OperatingSystem.IsWindows()) {
            logger.LogDebug("Win32SetWebView2Path is only supported on the Windows platform");
            return window;
        }

        if (window.Features.Lifecycle.IsClosedOrClosing()) return window;

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            callback: () => InfiniFrameNative.SetWebView2RuntimePath_win32(window.MainProgramHandle, data)
        );

        return window;
    }

    [SupportedOSPlatform("windows")]
    public IInfiniFrameWindow ClearBrowserAutoFill() {
        if (!OperatingSystem.IsWindows()) {
            logger.LogWarning("ClearBrowserAutoFill is only supported on the Windows platform");
            return window;
        }

        if (window.Features.Lifecycle.IsClosedOrClosing()) {
            return window;
        }

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.ClearBrowserAutoFill
        );

        return window;
    }
}
