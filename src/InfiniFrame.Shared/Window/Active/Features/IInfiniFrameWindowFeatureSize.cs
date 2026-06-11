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
    
    void SetSize(int width, int height);
    void SetSize(Size size);
    void SetHeight(int height);
    void SetMaxSize(int maxWidth, int maxHeight);
    void SetMaxSize(Size size);
    void SetMaxHeight(int maxHeight);
    void SetMaxWidth(int maxWidth);
    void SetMinSize(int minWidth, int minHeight);
    void SetMinSize(Size size);
    void SetMinHeight(int minHeight);
    void SetMinWidth(int minWidth);
    void SetWidth(int width);
    void Resize(int widthOffset, int heightOffset, ResizeOrigin origin);
}
