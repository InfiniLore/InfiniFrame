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
       
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Height => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        (IntPtr handle, out int value) => InfiniFrameNative.GetSize(handle, out _, out value)
    );

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Width => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        (IntPtr handle, out int value) => InfiniFrameNative.GetSize(handle, out value, out _)
    );


    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Size MaxSize => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        (IntPtr handle, out Size value) => {
            InfiniFrameNativeInteropStatus status = InfiniFrameNative.GetMaxSize(handle, out int width, out int height);
            value = new Size(width, height);
            return status;
        }
    );

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int MaxHeight => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        (IntPtr handle, out int value) => InfiniFrameNative.GetMaxSize(handle, out _, out value)
    );

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int MaxWidth => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        (IntPtr handle, out int value) => InfiniFrameNative.GetMaxSize(handle, out value, out _)
    );

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Size MinSize => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        (IntPtr handle, out Size value) => {
            InfiniFrameNativeInteropStatus status = InfiniFrameNative.GetMinSize(handle, out int width, out int height);
            value = new Size(width, height);
            return status;
        }
    );

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int MinHeight => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        (IntPtr handle, out int value) => InfiniFrameNative.GetMinSize(handle, out _, out value)
    );

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int MinWidth => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        (IntPtr handle, out int value) => InfiniFrameNative.GetMinSize(handle, out value, out _)
    );
    
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
    public IInfiniFrameWindow SetSize(int width, int height) {
        logger.LogDebug(".SetSize({Width}, {Height})", width, height);

        window.Invoke(() => InfiniFrameNative.SetSize(window.InstanceHandle, width, height));
        return window;
    }

    public IInfiniFrameWindow SetSize(Size size)
        => SetSize(size.Width, size.Height);

    public IInfiniFrameWindow SetHeight(int height) {
        logger.LogDebug(".SetHeight({Height})", height);

        window.Invoke(() => {
            InfiniFrameNative.GetSize(window.InstanceHandle, out int width, out _);
            InfiniFrameNative.SetSize(window.InstanceHandle, width, height);
        });

        return window;
    }

    public IInfiniFrameWindow SetMaxSize(int maxWidth, int maxHeight) {
        logger.LogDebug(".SetMaxSize({MaxWidth}, {MaxHeight})", maxWidth, maxHeight);
        window.Invoke(() => InfiniFrameNative.SetMaxSize(window.InstanceHandle, maxWidth, maxHeight));
        return window;
    }

    public IInfiniFrameWindow SetMaxSize(Size size)
        => SetMaxSize(size.Width, size.Height);

    public IInfiniFrameWindow SetMaxHeight(int maxHeight)
        => SetMaxSize(MaxWidth, maxHeight);

    public IInfiniFrameWindow SetMaxWidth(int maxWidth)
        => SetMaxSize(maxWidth, MaxHeight);



    public IInfiniFrameWindow SetMinSize(int minWidth, int minHeight) {
        logger.LogDebug(".SetMinSize({MinWidth}, {MinHeight})", minWidth, minHeight);
        window.Invoke(() => InfiniFrameNative.SetMinSize(window.InstanceHandle, minWidth, minHeight));
        return window;
    }

    public IInfiniFrameWindow SetMinSize(Size size)
        => SetMinSize(size.Width, size.Height);

    public IInfiniFrameWindow SetMinHeight(int minHeight)
        => SetMinSize(MinWidth, minHeight);

    public IInfiniFrameWindow SetMinWidth(int minWidth)
        => SetMinSize(minWidth, MinHeight);

    

    public IInfiniFrameWindow SetWidth(int width) {
        logger.LogDebug(".SetWidth({Width})", width);

        window.Invoke(() => {
            InfiniFrameNative.GetSize(window.InstanceHandle, out _, out int height);
            InfiniFrameNative.SetSize(window.InstanceHandle, width, height);
        });

        return window;
    }

    public IInfiniFrameWindow Resize(int widthOffset, int heightOffset, ResizeOrigin origin) {
        window.Invoke(() => {
            InfiniFrameNative.GetSize(window.InstanceHandle, out int width, out int height);
            InfiniFrameNative.GetPosition(window.InstanceHandle, out int originalX, out int originalY);

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

            InfiniFrameNative.SetSize(window.InstanceHandle, width, height);
            InfiniFrameNative.SetPosition(window.InstanceHandle, x, y);

        });
        return window;
    }
    
    public IInfiniFrameWindow SetResizable(bool resizable) {
        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.SetResizable,
            resizable
        );
        return window;
    }
}
