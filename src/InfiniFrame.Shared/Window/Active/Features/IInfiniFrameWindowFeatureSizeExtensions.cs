// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IInfiniFrameWindowFeatureSizeExtensions {
    public static IInfiniFrameWindow SetSize(this IInfiniFrameWindow window, int width, int height) {
        window.Features.Size.SetSize(width, height);
        return window;
    }
    public static IInfiniFrameWindow SetSize(this IInfiniFrameWindow window, Size size) {
        window.Features.Size.SetSize(size);
        return window;
    }
    public static IInfiniFrameWindow SetHeight(this IInfiniFrameWindow window, int height) {
        window.Features.Size.SetHeight(height);
        return window;
    }
    public static IInfiniFrameWindow SetMaxSize(this IInfiniFrameWindow window, int maxWidth, int maxHeight) {
        window.Features.Size.SetMaxSize(maxWidth, maxHeight);
        return window;
    }
    public static IInfiniFrameWindow SetMaxSize(this IInfiniFrameWindow window, Size size) {
        window.Features.Size.SetMaxSize(size);
        return window;
    }
    public static IInfiniFrameWindow SetMaxHeight(this IInfiniFrameWindow window, int maxHeight) {
        window.Features.Size.SetMaxHeight(maxHeight);
        return window;
    }
    public static IInfiniFrameWindow SetMaxWidth(this IInfiniFrameWindow window, int maxWidth) {
        window.Features.Size.SetMaxWidth(maxWidth);
        return window;
    }
    public static IInfiniFrameWindow SetMinSize(this IInfiniFrameWindow window, int minWidth, int minHeight) {
        window.Features.Size.SetMinSize(minWidth, minHeight);
        return window;
    }
    public static IInfiniFrameWindow SetMinSize(this IInfiniFrameWindow window, Size size) {
        window.Features.Size.SetMinSize(size);
        return window;
    }
    public static IInfiniFrameWindow SetMinHeight(this IInfiniFrameWindow window, int minHeight) {
        window.Features.Size.SetMinHeight(minHeight);
        return window;
    }
    public static IInfiniFrameWindow SetMinWidth(this IInfiniFrameWindow window, int minWidth) {
        window.Features.Size.SetMinWidth(minWidth);
        return window;
    }
    public static IInfiniFrameWindow SetWidth(this IInfiniFrameWindow window, int width) {
        window.Features.Size.SetWidth(width);
        return window;
    }
    public static IInfiniFrameWindow Resize(this IInfiniFrameWindow window, int widthOffset, int heightOffset, ResizeOrigin origin) {
        window.Features.Size.Resize(widthOffset, heightOffset, origin);
        return window;
    }
}
