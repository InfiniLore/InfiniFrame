// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowBuilderFeatureSize : IInfiniFrameWindowBuilderFeatureSize {
    public int Height { get; private set; }
    public int Width { get; private set; }
    public int MaxHeight { get; private set; } = int.MaxValue;
    public int MaxWidth { get; private set; } = int.MaxValue;
    public int MinHeight { get; private set; }
    public int MinWidth { get; private set; }
    public bool IsResizable { get; private set; } = true;
    public bool StartWithOsDefaultSize { get; private set; } = true;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void SetStartSize(int width, int height) {
        StartWithOsDefaultSize = false;
        Width = width;
        Height = height;
    }
    public void SetStartSize(Size size) {
        StartWithOsDefaultSize = false;
        Width = size.Width;
        Height = size.Height;
    }
    public void SetStartHeight(int height) {
        StartWithOsDefaultSize = false;
        Height = height;
    }
    public void SetStartWidth(int width) {
        StartWithOsDefaultSize = false;
        Width = width;
    }
    public void SetMaxSize(int maxWidth, int maxHeight) {
        MaxWidth = maxWidth;
        MaxHeight = maxHeight;
    }
    public void SetMaxSize(Size size) {
        MaxWidth = size.Width;
        MaxHeight = size.Height;
    }
    public void SetMaxHeight(int maxHeight) {
        MaxHeight = maxHeight;
    }
    public void SetMaxWidth(int maxWidth) {
        MaxWidth = maxWidth;
    }
    public void SetMinSize(int minWidth, int minHeight) {
        MinWidth = minWidth;
        MinHeight = minHeight;
    }
    public void SetMinSize(Size size) {
        MinWidth = size.Width;
        MinHeight = size.Height;
    }
    public void SetMinHeight(int minHeight) {
        MinHeight = minHeight;
    }
    public void SetMinWidth(int minWidth) {
        MinWidth = minWidth;
    }
    public void UseOsDefaultSize(bool enabled) {
        StartWithOsDefaultSize = enabled;
    }
    public void SetResizable(bool enabled) {
        IsResizable = enabled;
    }
}