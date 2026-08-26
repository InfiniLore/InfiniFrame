// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics;
using System.Runtime.Versioning;
using InfiniFrame.NativeBridge;
using Microsoft.Extensions.Logging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class BrowserInfiniFrameWindowFeature(
    IInfiniFrameWindow window,
    ILogger<BrowserInfiniFrameWindowFeature> logger
) : IBrowserInfiniFrameWindowFeature {

    /// <inheritdoc cref="IBrowserInfiniFrameWindowFeature.IsContextMenuEnabled" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsContextMenuEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window,
        window.ManagedThreadId,
        InfiniFrameNative.GetContextMenuEnabled
    );

    /// <inheritdoc cref="IBrowserInfiniFrameWindowFeature.IsMediaAutoplayEnabled" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsMediaAutoplayEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window,
        window.ManagedThreadId,
        InfiniFrameNative.GetMediaAutoplayEnabled
    );

    /// <inheritdoc cref="IBrowserInfiniFrameWindowFeature.UserAgent" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string? UserAgent => NativeInvoke.InvokeSyncWithValidation<string?>(
        logger,
        window,
        window.ManagedThreadId,
        InfiniFrameNative.GetUserAgent);

    /// <inheritdoc cref="IBrowserInfiniFrameWindowFeature.IsFileSystemAccessEnabled" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsFileSystemAccessEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window,
        window.ManagedThreadId,
        InfiniFrameNative.GetFileSystemAccessEnabled
    );

    /// <inheritdoc cref="IBrowserInfiniFrameWindowFeature.IsWebSecurityEnabled" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsWebSecurityEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window,
        window.ManagedThreadId,
        InfiniFrameNative.GetWebSecurityEnabled
    );

    /// <inheritdoc cref="IBrowserInfiniFrameWindowFeature.IsJavascriptClipboardAccessEnabled" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsJavascriptClipboardAccessEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window,
        window.ManagedThreadId,
        InfiniFrameNative.GetJavascriptClipboardAccessEnabled
    );

    /// <inheritdoc cref="IBrowserInfiniFrameWindowFeature.IsMediaStreamEnabled" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsMediaStreamEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window,
        window.ManagedThreadId,
        InfiniFrameNative.GetMediaStreamEnabled
    );

    /// <inheritdoc cref="IBrowserInfiniFrameWindowFeature.IsIgnoreCertificateErrorsEnabled" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsIgnoreCertificateErrorsEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window,
        window.ManagedThreadId,
        InfiniFrameNative.GetIgnoreCertificateErrorsEnabled
    );

    /// <inheritdoc cref="IBrowserInfiniFrameWindowFeature.GrantBrowserPermissions" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool GrantBrowserPermissions => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window,
        window.ManagedThreadId,
        InfiniFrameNative.GetGrantBrowserPermissions
    );

    /// <inheritdoc cref="IBrowserInfiniFrameWindowFeature.IsSmoothScrollingEnabled" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsSmoothScrollingEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window,
        window.ManagedThreadId,
        InfiniFrameNative.GetSmoothScrollingEnabled
    );

    /// <inheritdoc cref="IBrowserInfiniFrameWindowFeature.IsStatusBarEnabled" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsStatusBarEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window,
        window.ManagedThreadId,
        InfiniFrameNative.GetStatusBarEnabled
    );

    /// <inheritdoc cref="IBrowserInfiniFrameWindowFeature.IsBrowserShortcutsEnabled" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsBrowserShortcutsEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window,
        window.ManagedThreadId,
        InfiniFrameNative.GetBrowserShortcutsEnabled
    );

    /// <inheritdoc cref="IBrowserInfiniFrameWindowFeature.BrowserControlInitParameters" />
    public string? BrowserControlInitParameters => window.Configuration.StartupParameters.BrowserControlInitParameters;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IBrowserInfiniFrameWindowFeature.EnableStatusBar" />
    public void EnableStatusBar(bool enabled = true) {
        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetStatusBarEnabled,
            enabled
        );
    }

    /// <inheritdoc cref="IBrowserInfiniFrameWindowFeature.EnableBrowserShortcuts" />
    public void EnableBrowserShortcuts(bool enabled = true) {
        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetBrowserShortcutsEnabled,
            enabled
        );
    }

    /// <inheritdoc cref="IBrowserInfiniFrameWindowFeature.EnableContextMenu" />
    public void EnableContextMenu(bool enabled = true) {
        logger.LogDebug(".EnableContextMenu({Enabled})", enabled);

        bool originalValue = NativeInvoke.InvokeSyncWithValidation<bool>(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.GetContextMenuEnabled
        );

        if (originalValue == enabled) return;

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetContextMenuEnabled,
            enabled
        );
    }

    /// <inheritdoc cref="IBrowserInfiniFrameWindowFeature.EnableMediaAutoplay" />
    public void EnableMediaAutoplay(bool enabled = true) {
        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetMediaAutoplayEnabled,
            enabled
        );
    }

    /// <inheritdoc cref="IBrowserInfiniFrameWindowFeature.SetUserAgent" />
    public void SetUserAgent(string? userAgent) {
        if (string.IsNullOrWhiteSpace(userAgent)) userAgent = string.Empty;

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetUserAgent,
            userAgent
        );
    }

    /// <inheritdoc cref="IBrowserInfiniFrameWindowFeature.Win32SetWebView2Path" />
    [SupportedOSPlatform("windows")]
    public void Win32SetWebView2Path(string data) {
        if (!OperatingSystem.IsWindows()) {
            logger.LogDebug("Win32SetWebView2Path is only supported on the Windows platform");
            return;
        }

        if (window.Features.Lifecycle.IsClosedOrClosing()) return;

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            callback: () => InfiniFrameNative.SetWebView2RuntimePath_win32(window.MainProgramHandle, data)
        );
    }

    /// <inheritdoc cref="IBrowserInfiniFrameWindowFeature.ClearBrowserAutoFill" />
    [SupportedOSPlatform("windows")]
    public void ClearBrowserAutoFill() {
        if (!OperatingSystem.IsWindows()) {
            logger.LogWarning("ClearBrowserAutoFill is only supported on the Windows platform");
            return;
        }

        if (window.Features.Lifecycle.IsClosedOrClosing()) return;

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.ClearBrowserAutoFill
        );
    }
}
