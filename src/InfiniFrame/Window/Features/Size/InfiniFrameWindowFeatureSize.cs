// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowFeatureSize(
    IInfiniFrameWindow window,
    ILogger<InfiniFrameWindowFeatureSize> logger
) : IInfiniFrameWindowFeatureSize {

    /// <inheritdoc cref="IInfiniFrameWindowFeatureSize.Size"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Size Size => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        callback: (IntPtr handle, out Size value) => {
            InfiniFrameNativeInteropStatus status = InfiniFrameNative.GetSize(handle, out int width, out int height);
            value = new Size(width, height);
            return status;
        }
    );

    /// <inheritdoc cref="IInfiniFrameWindowFeatureSize.Height"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Height => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetSize(handle, out _, out value)
    );

    /// <inheritdoc cref="IInfiniFrameWindowFeatureSize.Width"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Width => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetSize(handle, out value, out _)
    );


    /// <inheritdoc cref="IInfiniFrameWindowFeatureSize.MaxSize"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Size MaxSize => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        callback: (IntPtr handle, out Size value) => {
            InfiniFrameNativeInteropStatus status = InfiniFrameNative.GetMaxSize(handle, out int width, out int height);
            value = new Size(width, height);
            return status;
        }
    );

    /// <inheritdoc cref="IInfiniFrameWindowFeatureSize.MaxHeight"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int MaxHeight => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetMaxSize(handle, out _, out value)
    );

    /// <inheritdoc cref="IInfiniFrameWindowFeatureSize.MaxWidth"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int MaxWidth => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetMaxSize(handle, out value, out _)
    );

    /// <inheritdoc cref="IInfiniFrameWindowFeatureSize.MinSize"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Size MinSize => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        callback: (IntPtr handle, out Size value) => {
            InfiniFrameNativeInteropStatus status = InfiniFrameNative.GetMinSize(handle, out int width, out int height);
            value = new Size(width, height);
            return status;
        }
    );

    /// <inheritdoc cref="IInfiniFrameWindowFeatureSize.MinHeight"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int MinHeight => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetMinSize(handle, out _, out value)
    );

    /// <inheritdoc cref="IInfiniFrameWindowFeatureSize.MinWidth"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int MinWidth => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetMinSize(handle, out value, out _)
    );

    /// <inheritdoc cref="IInfiniFrameWindowFeatureSize.IsResizable"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsResizable => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        InfiniFrameNative.GetResizable
    );


    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInfiniFrameWindowFeatureSize.SetSize(int, int)"/>
    public void SetSize(int width, int height) {
        logger.LogDebug(".SetSize({Width}, {Height})", width, height);

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.SetSize,
            width,
            height
        );
    }

    /// <inheritdoc cref="IInfiniFrameWindowFeatureSize.SetSize(Size)"/>
    public void SetSize(Size size)
        => SetSize(size.Width, size.Height);

    /// <inheritdoc cref="IInfiniFrameWindowFeatureSize.SetHeight"/>
    public void SetHeight(int height) {
        logger.LogDebug(".SetHeight({Height})", height);

        (int width, int _) = NativeInvoke.InvokeSyncWithValidation<int, int>(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.GetSize
        );

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.SetSize,
            width,
            height
        );
    }

    /// <inheritdoc cref="IInfiniFrameWindowFeatureSize.SetMaxSize(int, int)"/>
    public void SetMaxSize(int maxWidth, int maxHeight) {
        logger.LogDebug(".SetMaxSize({MaxWidth}, {MaxHeight})", maxWidth, maxHeight);

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
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
            window.InstanceHandle,
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
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.GetSize
        );

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.SetSize,
            width,
            height
        );
    }

    public void Resize(int widthOffset, int heightOffset, ResizeOrigin origin) {

        (int width, int height) = NativeInvoke.InvokeSyncWithValidation<int, int>(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.GetSize
        );
        (int originalX, int originalY) = NativeInvoke.InvokeSyncWithValidation<int, int>(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.GetPosition
        );

        int x = originalX;
        int y = originalY;
        switch (origin) {
            case ResizeOrigin.TopLeft: {
                x += widthOffset;
                y += heightOffset;
                width -= widthOffset;
                height -= heightOffset;
                break;
            }

            case ResizeOrigin.Top: {
                y += heightOffset;
                height -= heightOffset;
                break;
            }

            case ResizeOrigin.TopRight: {
                y += heightOffset;
                width += widthOffset;
                height -= heightOffset;
                break;
            }

            case ResizeOrigin.Right: {
                width += widthOffset;
                break;
            }

            case ResizeOrigin.BottomRight: {
                width += widthOffset;
                height += heightOffset;
                break;
            }

            case ResizeOrigin.Bottom: {
                height += heightOffset;
                break;
            }

            case ResizeOrigin.BottomLeft: {
                x += widthOffset;
                width -= widthOffset;
                height += heightOffset;
                break;
            }

            case ResizeOrigin.Left: {
                x += widthOffset;
                width -= widthOffset;
                break;
            }

            default: throw new ArgumentOutOfRangeException(nameof(origin), origin, null);
        }

        // Clamping between min and max size
        Size max = MaxSize;
        Size min = MinSize;

        if (width >= max.Width) {
            width = max.Width;
            x = originalX;
        }

        if (height >= max.Height) {
            height = max.Height;
            y = originalY;
        }

        if (width <= min.Width) {
            width = min.Width;
            x = originalX;
        }

        if (height <= min.Height) {
            height = min.Height;
            y = originalY;
        }

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.SetSize,
            width,
            height
        );
        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.SetPosition,
            x,
            y
        );
    }

    public void SetResizable(bool resizable = true) {
        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.SetResizable,
            resizable
        );
    }
}
