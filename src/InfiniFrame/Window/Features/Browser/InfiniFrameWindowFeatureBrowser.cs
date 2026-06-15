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
    
    /// <inheritdoc cref="IInfiniFrameWindowFeatureBrowser.IsContextMenuEnabled"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsContextMenuEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        InfiniFrameNative.GetContextMenuEnabled
    );

    /// <inheritdoc cref="IInfiniFrameWindowFeatureBrowser.IsMediaAutoplayEnabled"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsMediaAutoplayEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        InfiniFrameNative.GetMediaAutoplayEnabled
    );

    /// <inheritdoc cref="IInfiniFrameWindowFeatureBrowser.UserAgent"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string? UserAgent => NativeInvoke.InvokeSyncWithValidation<string?>(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        InfiniFrameNative.GetUserAgent);

    /// <inheritdoc cref="IInfiniFrameWindowFeatureBrowser.IsFileSystemAccessEnabled"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsFileSystemAccessEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        InfiniFrameNative.GetFileSystemAccessEnabled
    );

    /// <inheritdoc cref="IInfiniFrameWindowFeatureBrowser.IsWebSecurityEnabled"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsWebSecurityEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        InfiniFrameNative.GetWebSecurityEnabled
    );

    /// <inheritdoc cref="IInfiniFrameWindowFeatureBrowser.IsJavascriptClipboardAccessEnabled"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsJavascriptClipboardAccessEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        InfiniFrameNative.GetJavascriptClipboardAccessEnabled
    );

    /// <inheritdoc cref="IInfiniFrameWindowFeatureBrowser.IsMediaStreamEnabled"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsMediaStreamEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        InfiniFrameNative.GetMediaStreamEnabled
    );

    /// <inheritdoc cref="IInfiniFrameWindowFeatureBrowser.IsIgnoreCertificateErrorsEnabled"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsIgnoreCertificateErrorsEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window.InstanceHandle, 
        window.ManagedThreadId,
        InfiniFrameNative.GetIgnoreCertificateErrorsEnabled
    );

    /// <inheritdoc cref="IInfiniFrameWindowFeatureBrowser.GrantBrowserPermissions"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool GrantBrowserPermissions => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        InfiniFrameNative.GetGrantBrowserPermissions
    );

    /// <inheritdoc cref="IInfiniFrameWindowFeatureBrowser.IsSmoothScrollingEnabled"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsSmoothScrollingEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window.InstanceHandle, 
        window.ManagedThreadId,
        InfiniFrameNative.GetSmoothScrollingEnabled
    );

    /// <inheritdoc cref="IInfiniFrameWindowFeatureBrowser.BrowserControlInitParameters"/>
    public string? BrowserControlInitParameters => window.Configuration.StartupParameters.BrowserControlInitParameters;
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInfiniFrameWindowFeatureBrowser.EnableContextMenu"/>
    public void EnableContextMenu(bool enabled = true) {
        logger.LogDebug(".EnableContextMenu({Enabled})", enabled);

        bool originalValue = NativeInvoke.InvokeSyncWithValidation<bool>(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.GetContextMenuEnabled
        );
        
        if (originalValue == enabled) return;
        
        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.SetContextMenuEnabled,
            enabled
        );
    }

    /// <inheritdoc cref="IInfiniFrameWindowFeatureBrowser.EnableMediaAutoplay"/>
    public void EnableMediaAutoplay(bool enabled = true) {
        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.SetMediaAutoplayEnabled,
            enabled
        );
    }
    
    /// <inheritdoc cref="IInfiniFrameWindowFeatureBrowser.SetUserAgent"/>
    public void SetUserAgent(string? userAgent) {
        if (string.IsNullOrWhiteSpace(userAgent)) userAgent = string.Empty;
        
        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.SetUserAgent,
            userAgent
        );
    }

    /// <inheritdoc cref="IInfiniFrameWindowFeatureBrowser.Win32SetWebView2Path"/>
    [SupportedOSPlatform("windows")]
    public void Win32SetWebView2Path(string data) {
        if (!OperatingSystem.IsWindows()) {
            logger.LogDebug("Win32SetWebView2Path is only supported on the Windows platform");
            return;
        }

        if (window.Features.Lifecycle.IsClosedOrClosing()) return;

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            callback: () => InfiniFrameNative.SetWebView2RuntimePath_win32(window.MainProgramHandle, data)
        );
    }

    /// <inheritdoc cref="IInfiniFrameWindowFeatureBrowser.ClearBrowserAutoFill"/>
    [SupportedOSPlatform("windows")]
    public void ClearBrowserAutoFill() {
        if (!OperatingSystem.IsWindows()) {
            logger.LogWarning("ClearBrowserAutoFill is only supported on the Windows platform");
            return;
        }

        if (window.Features.Lifecycle.IsClosedOrClosing()) return;

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.ClearBrowserAutoFill
        );
    }
}
