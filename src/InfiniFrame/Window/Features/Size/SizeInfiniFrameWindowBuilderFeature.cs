// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SizeInfiniFrameWindowBuilderFeature : ISizeInfiniFrameWindowBuilderFeature {
    /// <inheritdoc cref="ISizeInfiniFrameWindowBuilderFeature.Height"/>
    public int Height { get; private set; }
    /// <inheritdoc cref="ISizeInfiniFrameWindowBuilderFeature.Width"/>
    public int Width { get; private set; }
    /// <inheritdoc cref="ISizeInfiniFrameWindowBuilderFeature.MaxHeight"/>
    public int MaxHeight { get; private set; } = int.MaxValue;
    /// <inheritdoc cref="ISizeInfiniFrameWindowBuilderFeature.MaxWidth"/>
    public int MaxWidth { get; private set; } = int.MaxValue;
    /// <inheritdoc cref="ISizeInfiniFrameWindowBuilderFeature.MinHeight"/>
    public int MinHeight { get; private set; }
    /// <inheritdoc cref="ISizeInfiniFrameWindowBuilderFeature.MinWidth"/>
    public int MinWidth { get; private set; }
    /// <inheritdoc cref="ISizeInfiniFrameWindowBuilderFeature.IsResizable"/>
    public bool IsResizable { get; private set; } = true;
    /// <inheritdoc cref="ISizeInfiniFrameWindowBuilderFeature.StartWithOsDefaultSize"/>
    public bool StartWithOsDefaultSize { get; private set; } = true;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="ISizeInfiniFrameWindowBuilderFeature.SetSize(int, int)"/>
    public void SetSize(int width, int height) {
        StartWithOsDefaultSize = false;
        Width = width;
        Height = height;
    }
    /// <inheritdoc cref="ISizeInfiniFrameWindowBuilderFeature.SetSize(Size)"/>
    public void SetSize(Size size) {
        StartWithOsDefaultSize = false;
        Width = size.Width;
        Height = size.Height;
    }
    /// <inheritdoc cref="ISizeInfiniFrameWindowBuilderFeature.SetHeight"/>
    public void SetHeight(int height) {
        StartWithOsDefaultSize = false;
        Height = height;
    }
    /// <inheritdoc cref="ISizeInfiniFrameWindowBuilderFeature.SetWidth"/>
    public void SetWidth(int width) {
        StartWithOsDefaultSize = false;
        Width = width;
    }
    /// <inheritdoc cref="ISizeInfiniFrameWindowBuilderFeature.SetMaxSize(int, int)"/>
    public void SetMaxSize(int maxWidth, int maxHeight) {
        MaxWidth = maxWidth;
        MaxHeight = maxHeight;
    }
    /// <inheritdoc cref="ISizeInfiniFrameWindowBuilderFeature.SetMaxSize(Size)"/>
    public void SetMaxSize(Size size) {
        MaxWidth = size.Width;
        MaxHeight = size.Height;
    }
    /// <inheritdoc cref="ISizeInfiniFrameWindowBuilderFeature.SetMaxHeight"/>
    public void SetMaxHeight(int maxHeight) {
        MaxHeight = maxHeight;
    }
    /// <inheritdoc cref="ISizeInfiniFrameWindowBuilderFeature.SetMaxWidth"/>
    public void SetMaxWidth(int maxWidth) {
        MaxWidth = maxWidth;
    }
    /// <inheritdoc cref="ISizeInfiniFrameWindowBuilderFeature.SetMinSize(int, int)"/>
    public void SetMinSize(int minWidth, int minHeight) {
        MinWidth = minWidth;
        MinHeight = minHeight;
    }
    /// <inheritdoc cref="ISizeInfiniFrameWindowBuilderFeature.SetMinSize(Size)"/>
    public void SetMinSize(Size size) {
        MinWidth = size.Width;
        MinHeight = size.Height;
    }
    /// <inheritdoc cref="ISizeInfiniFrameWindowBuilderFeature.SetMinHeight"/>
    public void SetMinHeight(int minHeight) {
        MinHeight = minHeight;
    }
    /// <inheritdoc cref="ISizeInfiniFrameWindowBuilderFeature.SetMinWidth"/>
    public void SetMinWidth(int minWidth) {
        MinWidth = minWidth;
    }
    /// <inheritdoc cref="ISizeInfiniFrameWindowBuilderFeature.UseOsDefaultSize"/>
    public void UseOsDefaultSize(bool enabled = true) {
        StartWithOsDefaultSize = enabled;
    }
    /// <inheritdoc cref="ISizeInfiniFrameWindowBuilderFeature.SetResizable"/>
    public void SetResizable(bool enabled = true) {
        IsResizable = enabled;
    }
    
    public void ApplyToNativeParameters(ref InfiniFrameNativeParameters parameters) {
        parameters.Height = Height;
        parameters.Width = Width;
        parameters.MaxHeight = MaxHeight;
        parameters.MaxWidth = MaxWidth;
        parameters.MinHeight = MinHeight;
        parameters.MinWidth = MinWidth;
        parameters.Resizable = IsResizable;
        parameters.UseOsDefaultSize = StartWithOsDefaultSize;
    }
}