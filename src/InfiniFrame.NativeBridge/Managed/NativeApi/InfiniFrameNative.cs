// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;
using System.Runtime.InteropServices;

namespace InfiniFrame.NativeBridge;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class InfiniFrameNative {
    internal static string? PtrToNativeString(IntPtr ptr) {
        if (ptr == IntPtr.Zero) return null;

        return OperatingSystem.IsWindows()
            ? Marshal.PtrToStringUni(ptr)
            : Marshal.PtrToStringUTF8(ptr);
    }

    internal static InfiniFrameNativeInteropStatus GetHeight(IntPtr instance, out int height)
        => GetSize(instance, out _, out height);

    internal static InfiniFrameNativeInteropStatus GetWidth(IntPtr instance, out int width)
        => GetSize(instance, out width, out _);

    internal static InfiniFrameNativeInteropStatus GetMaxHeight(IntPtr instance, out int maxHeight)
        => GetMaxSize(instance, out _, out maxHeight);

    internal static InfiniFrameNativeInteropStatus GetMaxWidth(IntPtr instance, out int maxWidth)
        => GetMaxSize(instance, out maxWidth, out _);

    internal static InfiniFrameNativeInteropStatus GetMinHeight(IntPtr instance, out int minHeight)
        => GetMinSize(instance, out _, out minHeight);

    internal static InfiniFrameNativeInteropStatus GetMinWidth(IntPtr instance, out int minWidth)
        => GetMinSize(instance, out minWidth, out _);

    internal static InfiniFrameNativeInteropStatus GetLeft(IntPtr instance, out int left)
        => GetPosition(instance, out left, out _);

    internal static InfiniFrameNativeInteropStatus GetTop(IntPtr instance, out int top)
        => GetPosition(instance, out _, out top);

    internal static InfiniFrameNativeInteropStatus GetSize(IntPtr instance, out Size size) {
        InfiniFrameNativeInteropStatus status = GetSize(instance, out int width, out int height);
        size = new Size(width, height);
        return status;
    }

    internal static InfiniFrameNativeInteropStatus GetMaxSize(IntPtr instance, out Size size) {
        InfiniFrameNativeInteropStatus status = GetMaxSize(instance, out int width, out int height);
        size = new Size(width, height);
        return status;
    }

    internal static InfiniFrameNativeInteropStatus GetMinSize(IntPtr instance, out Size size) {
        InfiniFrameNativeInteropStatus status = GetMinSize(instance, out int width, out int height);
        size = new Size(width, height);
        return status;
    }

    internal static InfiniFrameNativeInteropStatus GetPosition(IntPtr instance, out Point position) {
        InfiniFrameNativeInteropStatus status = GetPosition(instance, out int left, out int top);
        position = new Point(left, top);
        return status;
    }

    internal static InfiniFrameNativeInteropStatus GetWindowRectangle(IntPtr instance, out int x, out int y, out int width, out int height) {
        InfiniFrameNativeInteropStatus sizeStatus = GetSize(instance, out width, out height);
        if (sizeStatus != InfiniFrameNativeInteropStatus.Success) {
            x = 0;
            y = 0;
            return sizeStatus;
        }

        return GetPosition(instance, out x, out y);
    }

    internal static InfiniFrameNativeInteropStatus GetWindowRectangle(IntPtr instance, out Rectangle rectangle) {
        InfiniFrameNativeInteropStatus status = GetWindowRectangle(instance, out int x, out int y, out int width, out int height);
        rectangle = new Rectangle(x, y, width, height);
        return status;
    }
}
