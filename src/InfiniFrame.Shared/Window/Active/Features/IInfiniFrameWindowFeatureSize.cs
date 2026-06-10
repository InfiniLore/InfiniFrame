// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowFeatureSize{
    Size Size { get; }
    int Height { get; } 
    int Width { get; }
    Size MaxSize { get; }
    int MaxHeight { get; }
    int MaxWidth { get; }
    Size MinSize { get; }
    int MinHeight { get; }
    int MinWidth { get; }
    bool IsResizable { get; }
    
    IInfiniFrameWindow SetSize(int width, int height);
    IInfiniFrameWindow SetSize(Size size);
    IInfiniFrameWindow SetHeight(int height);
    IInfiniFrameWindow SetMaxSize(int maxWidth, int maxHeight);
    IInfiniFrameWindow SetMaxSize(Size size);
    IInfiniFrameWindow SetMaxHeight(int maxHeight);
    IInfiniFrameWindow SetMaxWidth(int maxWidth);
    IInfiniFrameWindow SetMinSize(int minWidth, int minHeight);
    IInfiniFrameWindow SetMinSize(Size size);
    IInfiniFrameWindow SetMinHeight(int minHeight);
    IInfiniFrameWindow SetMinWidth(int minWidth);
    IInfiniFrameWindow SetWidth(int width);
    IInfiniFrameWindow Resize(int widthOffset, int heightOffset, ResizeOrigin origin);
}
