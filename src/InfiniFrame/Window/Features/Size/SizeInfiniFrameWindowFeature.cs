// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SizeInfiniFrameWindowFeature(
    IInfiniFrameWindow window,
    ILogger<SizeInfiniFrameWindowFeature> logger
) : ISizeInfiniFrameWindowFeature {

    /// <inheritdoc cref="ISizeInfiniFrameWindowFeature.Size" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Size Size => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window,
        window.ManagedThreadId,
        callback: (IntPtr handle, out Size value) => {
            InfiniFrameNativeInteropStatus status = InfiniFrameNative.GetSize(handle, out int width, out int height);
            value = new Size(width, height);
            return status;
        }
    );

    /// <inheritdoc cref="ISizeInfiniFrameWindowFeature.Height" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Height => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window,
        window.ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetSize(handle, out _, out value)
    );

    /// <inheritdoc cref="ISizeInfiniFrameWindowFeature.Width" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Width => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window,
        window.ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetSize(handle, out value, out _)
    );


    /// <inheritdoc cref="ISizeInfiniFrameWindowFeature.MaxSize" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Size MaxSize => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window,
        window.ManagedThreadId,
        callback: (IntPtr handle, out Size value) => {
            InfiniFrameNativeInteropStatus status = InfiniFrameNative.GetMaxSize(handle, out int width, out int height);
            value = new Size(width, height);
            return status;
        }
    );

    /// <inheritdoc cref="ISizeInfiniFrameWindowFeature.MaxHeight" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int MaxHeight => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window,
        window.ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetMaxSize(handle, out _, out value)
    );

    /// <inheritdoc cref="ISizeInfiniFrameWindowFeature.MaxWidth" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int MaxWidth => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window,
        window.ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetMaxSize(handle, out value, out _)
    );

    /// <inheritdoc cref="ISizeInfiniFrameWindowFeature.MinSize" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Size MinSize => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window,
        window.ManagedThreadId,
        callback: (IntPtr handle, out Size value) => {
            InfiniFrameNativeInteropStatus status = InfiniFrameNative.GetMinSize(handle, out int width, out int height);
            value = new Size(width, height);
            return status;
        }
    );

    /// <inheritdoc cref="ISizeInfiniFrameWindowFeature.MinHeight" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int MinHeight => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window,
        window.ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetMinSize(handle, out _, out value)
    );

    /// <inheritdoc cref="ISizeInfiniFrameWindowFeature.MinWidth" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int MinWidth => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window,
        window.ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetMinSize(handle, out value, out _)
    );

    /// <inheritdoc cref="ISizeInfiniFrameWindowFeature.IsResizable" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsResizable => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window,
        window.ManagedThreadId,
        InfiniFrameNative.GetResizable
    );


    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="ISizeInfiniFrameWindowFeature.SetSize(int, int)" />
    public void SetSize(int width, int height) {
        logger.LogDebug(".SetSize({Width}, {Height})", width, height);

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetSize,
            width,
            height
        );
    }

    // ReSharper disable once InvalidXmlDocComment
    /// <inheritdoc cref="ISizeInfiniFrameWindowFeature.SetSize(Size)" />
    public void SetSize(Size size)
        => SetSize(size.Width, size.Height);

    /// <inheritdoc cref="ISizeInfiniFrameWindowFeature.SetHeight" />
    public void SetHeight(int height) {
        logger.LogDebug(".SetHeight({Height})", height);

        (int width, int _) = NativeInvoke.InvokeSyncWithValidation<int, int>(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.GetSize
        );

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetSize,
            width,
            height
        );
    }

    /// <inheritdoc cref="ISizeInfiniFrameWindowFeature.SetMaxSize(int, int)" />
    public void SetMaxSize(int maxWidth, int maxHeight) {
        logger.LogDebug(".SetMaxSize({MaxWidth}, {MaxHeight})", maxWidth, maxHeight);

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetMaxSize,
            maxWidth,
            maxHeight
        );
    }

    public void SetMaxSize(Size size)
        => SetMaxSize(size.Width, size.Height);

    public void SetMaxHeight(int maxHeight)
        => SetMaxSize(MaxWidth, maxHeight);

    public void SetMaxWidth(int maxWidth)
        => SetMaxSize(maxWidth, MaxHeight);

    public void SetMinSize(int minWidth, int minHeight) {
        logger.LogDebug(".SetMinSize({MinWidth}, {MinHeight})", minWidth, minHeight);

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetMinSize,
            minWidth,
            minHeight
        );
    }

    public void SetMinSize(Size size)
        => SetMinSize(size.Width, size.Height);

    public void SetMinHeight(int minHeight)
        => SetMinSize(MinWidth, minHeight);

    public void SetMinWidth(int minWidth)
        => SetMinSize(minWidth, MinHeight);

    public void SetWidth(int width) {
        logger.LogDebug(".SetWidth({Width})", width);

        (int _, int height) = NativeInvoke.InvokeSyncWithValidation<int, int>(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.GetSize
        );

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetSize,
            width,
            height
        );
    }

    public void Resize(int widthOffset, int heightOffset, ResizeOrigin origin) {

        (int width, int height) = NativeInvoke.InvokeSyncWithValidation<int, int>(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.GetSize
        );
        (int originalX, int originalY) = NativeInvoke.InvokeSyncWithValidation<int, int>(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.GetPosition
        );

        (int x, int y, width, height) = SizeCalculations.ComputeResize(
            originalX, originalY, width, height,
            widthOffset, heightOffset, origin
        );

        (x, y, width, height) = SizeCalculations.ClampResize(
            x, y, width, height,
            originalX, originalY,
            MinSize, MaxSize
        );

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetSize,
            width,
            height
        );
        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetPosition,
            x,
            y
        );
    }

    public void SetResizable(bool resizable = true) {
        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetResizable,
            resizable
        );
    }
}