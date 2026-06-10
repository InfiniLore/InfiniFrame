// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowBuilderFeatureSize{
    int Height { get; } 
    int Width { get; }
    int MaxHeight { get; }
    int MaxWidth { get; }
    int MinHeight { get; }
    int MinWidth { get; }
    bool IsResizable { get; }
    bool StartWithOsDefaultSize { get; }
    
    void SetStartSize(int width, int height);
    void SetStartSize(Size size);
    void SetStartHeight(int height);
    void SetStartWidth(int width);
    void SetMaxSize(int maxWidth, int maxHeight);
    void SetMaxSize(Size size);
    void SetMaxHeight(int maxHeight);
    void SetMaxWidth(int maxWidth);
    void SetMinSize(int minWidth, int minHeight);
    void SetMinSize(Size size);
    void SetMinHeight(int minHeight);
    void SetMinWidth(int minWidth);
    void UseOsDefaultSize(bool enabled);
    void SetResizable(bool resizable);
}
