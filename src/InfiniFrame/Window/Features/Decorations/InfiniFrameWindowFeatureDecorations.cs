// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowFeatureDecorations(
    IInfiniFrameWindow window,
    IInfiniFrameWindowBuilder originalBuilder,
    ILogger<InfiniFrameWindowFeatureDecorations> logger
) : IInfiniFrameWindowFeatureDecorations {
    
    /// <inheritdoc cref="IInfiniFrameWindowFeatureDecorations.IsChromeless"/>
    public bool IsChromeless => window.Configuration.StartupParameters.Chromeless;

    /// <inheritdoc cref="IInfiniFrameWindowFeatureDecorations.LimitLinuxWindowTitleLength"/>
    public bool LimitLinuxWindowTitleLength { get; set; } = originalBuilder.Features.Decorations.LimitLinuxWindowTitleLength;
    
    /// <inheritdoc cref="IInfiniFrameWindowFeatureDecorations.IsTransparent"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsTransparent {
        get {
            // On Windows, the transparency can only be set at startup
            if (OperatingSystem.IsWindows()) return window.Configuration.StartupParameters.Transparent;
            
            // On other platforms, the transparency can be changed at any time
            if (window.Features.Lifecycle.IsClosedOrClosing()) return false;
            
            return NativeInvoke.InvokeSyncWithValidation<bool>(
                logger,
                window.InstanceHandle,
                window.ManagedThreadId,
                InfiniFrameNative.GetTransparentEnabled
            );
        }
    }
    
    /// <inheritdoc cref="IInfiniFrameWindowFeatureDecorations.Title"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string? Title => NativeInvoke.InvokeSyncWithValidation<string?>(
        logger, 
        window.InstanceHandle, 
        window.ManagedThreadId,
        InfiniFrameNative.GetTitle
    );
    
    /// <inheritdoc cref="IInfiniFrameWindowFeatureDecorations.IconFilePath"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string? IconFilePath => NativeInvoke.InvokeSyncWithValidation<string?>(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        InfiniFrameNative.GetIconFileName
    );


    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInfiniFrameWindowFeatureDecorations.SetTransparent"/>
    public void SetTransparent(bool enabled) {
        if (OperatingSystem.IsWindows()) {
            logger.LogWarning("Transparent can only be set on Windows before the native window is instantiated.");
            return;
        }

        logger.LogDebug("Invoking InfiniFrameNative.SetTransparentEnabled({value})", enabled);
        NativeInvoke.InvokeSyncWithoutValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.SetTransparentEnabled,
            enabled
        );
    }
    
    /// <inheritdoc cref="IInfiniFrameWindowFeatureDecorations.SetTitle"/>
    public void SetTitle(string? title) {
        if (window.Features.Lifecycle.IsClosedOrClosing()) return;
        
        string? oldTitle = NativeInvoke.InvokeSyncWithValidation<string?>(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.GetTitle
        );
        
        if (title == oldTitle) return;
        
        logger.LogDebug("Invoking InfiniFrameNative.SetTitle({title})", title);
        string? newTitle = TitleStringUtility.Validate(title, LimitLinuxWindowTitleLength);
        
        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.SetTitle,
            newTitle
        );

    }

    /// <inheritdoc cref="IInfiniFrameWindowFeatureDecorations.SetIconFile"/>
    public void SetIconFile(string iconFilePath) {
        logger.LogDebug(".SetIconFile({IconFile})", iconFilePath);

        if (!IconFileUtility.TryResolveIconFilePath(iconFilePath, out string? resolvedIconFilePath)) {
            logger.LogWarning("Icon file {IconFile} does not exist or is an invalid file path.", iconFilePath);
            return;
        }

        if (IconFilePath == resolvedIconFilePath) {
            logger.LogDebug("Icon file is already set to {IconFile}, skipping assignment", resolvedIconFilePath);
            return;
        }

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.SetIconFile,
            resolvedIconFilePath
        );

    }

    /// <inheritdoc cref="IInfiniFrameWindowFeatureDecorations.SetLimitLinuxWindowTitleLength"/>
    public void SetLimitLinuxWindowTitleLength(bool enabled = true) {
        LimitLinuxWindowTitleLength = enabled;
    } 

}
