// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowFeatureSize{
    IInfiniFrameWindow SetSize(int width, int height);
    IInfiniFrameWindow SetSize(Size size);
    IInfiniFrameWindow SetHeight(int height);
    IInfiniFrameWindow SetMaximized(bool maximized);
    IInfiniFrameWindow ToggleMaximized();
    IInfiniFrameWindow SetMaxSize(int maxWidth, int maxHeight);
    IInfiniFrameWindow SetMaxSize(Size size);
    IInfiniFrameWindow SetMaxHeight(int maxHeight);
    IInfiniFrameWindow SetMaxWidth(int maxWidth);
    IInfiniFrameWindow SetMinimized(bool minimized);
    IInfiniFrameWindow SetMinSize(int minWidth, int minHeight);
    IInfiniFrameWindow SetMinSize(Size size);
    IInfiniFrameWindow SetMinHeight(int minHeight);
    IInfiniFrameWindow SetMinWidth(int minWidth);
    IInfiniFrameWindow SetFullScreen(bool fullScreen);
    IInfiniFrameWindow SetWidth(int width);
    IInfiniFrameWindow Resize(int widthOffset, int heightOffset, ResizeOrigin origin);
}
