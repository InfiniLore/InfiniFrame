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
public class DecorationsInfiniFrameWindowFeature(
    IInfiniFrameWindow window,
    IInfiniFrameWindowBuilder originalBuilder,
    ILogger<DecorationsInfiniFrameWindowFeature> logger
) : IDecorationsInfiniFrameWindowFeature {

    /// <inheritdoc cref="IDecorationsInfiniFrameWindowFeature.IsChromeless" />
    public bool IsChromeless => window.Configuration.StartupParameters.Chromeless;

    /// <inheritdoc cref="IDecorationsInfiniFrameWindowFeature.LimitLinuxWindowTitleLength" />
    public bool LimitLinuxWindowTitleLength { get; set; } = originalBuilder.Features.Decorations.LimitLinuxWindowTitleLength;

    /// <inheritdoc cref="IDecorationsInfiniFrameWindowFeature.IsTransparent" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsTransparent {
        get {
            // On Windows, the transparency can only be set at startup
            if (OperatingSystem.IsWindows()) return window.Configuration.StartupParameters.Transparent;

            // On other platforms, the transparency can be changed at any time
            if (window.Features.Lifecycle.IsClosedOrClosing()) return false;

            return NativeInvoke.InvokeSyncWithValidation<bool>(
                logger,
                window,
                window.ManagedThreadId,
                InfiniFrameNative.GetTransparentEnabled
            );
        }
    }

    /// <inheritdoc cref="IDecorationsInfiniFrameWindowFeature.BackgroundColor" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string? BackgroundColor => window.Configuration.StartupParameters.BackgroundColor;

    /// <inheritdoc cref="IDecorationsInfiniFrameWindowFeature.Title" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string? Title => NativeInvoke.InvokeSyncWithValidation<string?>(
        logger,
        window,
        window.ManagedThreadId,
        InfiniFrameNative.GetTitle
    );

    /// <inheritdoc cref="IDecorationsInfiniFrameWindowFeature.IconFilePath" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string? IconFilePath => NativeInvoke.InvokeSyncWithValidation<string?>(
        logger,
        window,
        window.ManagedThreadId,
        InfiniFrameNative.GetIconFileName
    );


    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IDecorationsInfiniFrameWindowFeature.SetTransparent" />
    public void SetTransparent(bool enabled) {
        if (OperatingSystem.IsWindows()) {
            logger.LogWarning("Transparent can only be set on Windows before the native window is instantiated.");
            return;
        }

        logger.LogDebug("Invoking InfiniFrameNative.SetTransparentEnabled({value})", enabled);
        NativeInvoke.InvokeSyncWithoutValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetTransparentEnabled,
            enabled
        );
    }

    /// <inheritdoc cref="IDecorationsInfiniFrameWindowFeature.SetBackgroundColor" />
    public void SetBackgroundColor(string? color) {
        if (color is not null && color != "transparent" && !IsValidBackgroundColor(color)) {
            throw new ArgumentException("Background color must be a valid hex color string (e.g. #RRGGBB or #AARRGGBB), null, or 'transparent'.", nameof(color));
        }

        logger.LogDebug("Invoking InfiniFrameNative.SetBackgroundColor({color})", color);
        NativeInvoke.InvokeSyncWithoutValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetBackgroundColor,
            color
        );
    }

    /// <inheritdoc cref="IDecorationsInfiniFrameWindowFeature.SetTitle" />
    public void SetTitle(string? title) {
        if (window.Features.Lifecycle.IsClosedOrClosing()) return;

        string? oldTitle = NativeInvoke.InvokeSyncWithValidation<string?>(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.GetTitle
        );

        if (title == oldTitle) return;

        logger.LogDebug("Invoking InfiniFrameNative.SetTitle({title})", title);
        string? newTitle = TitleStringUtility.Validate(title, LimitLinuxWindowTitleLength);

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetTitle,
            newTitle
        );

    }

    /// <inheritdoc cref="IDecorationsInfiniFrameWindowFeature.SetIconFile" />
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
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetIconFile,
            resolvedIconFilePath
        );

    }

    /// <inheritdoc cref="IDecorationsInfiniFrameWindowFeature.SetLimitLinuxWindowTitleLength" />
    public void SetLimitLinuxWindowTitleLength(bool enabled = true) {
        LimitLinuxWindowTitleLength = enabled;
    }

    private static bool IsValidBackgroundColor(string? color) {
        if (string.IsNullOrEmpty(color))
            return false;
        if (color.StartsWith('#')) {
            string hex = color[1..];
            return hex.Length is 6 or 8 && hex.All(c => IsHexDigit(c));
        }
        return false;
    }

    private static bool IsHexDigit(char c) =>
        c is >= '0' and <= '9' or >= 'A' and <= 'F' or >= 'a' and <= 'f';

}