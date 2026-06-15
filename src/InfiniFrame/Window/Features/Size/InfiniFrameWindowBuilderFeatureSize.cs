// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowBuilderFeatureSize : IInfiniFrameWindowBuilderFeatureSize {
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureSize.Height"/>
    public int Height { get; private set; }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureSize.Width"/>
    public int Width { get; private set; }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureSize.MaxHeight"/>
    public int MaxHeight { get; private set; } = int.MaxValue;
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureSize.MaxWidth"/>
    public int MaxWidth { get; private set; } = int.MaxValue;
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureSize.MinHeight"/>
    public int MinHeight { get; private set; }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureSize.MinWidth"/>
    public int MinWidth { get; private set; }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureSize.IsResizable"/>
    public bool IsResizable { get; private set; } = true;
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureSize.StartWithOsDefaultSize"/>
    public bool StartWithOsDefaultSize { get; private set; } = true;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureSize.SetSize(int, int)"/>
    public void SetSize(int width, int height) {
        StartWithOsDefaultSize = false;
        Width = width;
        Height = height;
    }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureSize.SetSize(Size)"/>
    public void SetSize(Size size) {
        StartWithOsDefaultSize = false;
        Width = size.Width;
        Height = size.Height;
    }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureSize.SetHeight"/>
    public void SetHeight(int height) {
        StartWithOsDefaultSize = false;
        Height = height;
    }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureSize.SetWidth"/>
    public void SetWidth(int width) {
        StartWithOsDefaultSize = false;
        Width = width;
    }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureSize.SetMaxSize(int, int)"/>
    public void SetMaxSize(int maxWidth, int maxHeight) {
        MaxWidth = maxWidth;
        MaxHeight = maxHeight;
    }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureSize.SetMaxSize(Size)"/>
    public void SetMaxSize(Size size) {
        MaxWidth = size.Width;
        MaxHeight = size.Height;
    }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureSize.SetMaxHeight"/>
    public void SetMaxHeight(int maxHeight) {
        MaxHeight = maxHeight;
    }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureSize.SetMaxWidth"/>
    public void SetMaxWidth(int maxWidth) {
        MaxWidth = maxWidth;
    }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureSize.SetMinSize(int, int)"/>
    public void SetMinSize(int minWidth, int minHeight) {
        MinWidth = minWidth;
        MinHeight = minHeight;
    }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureSize.SetMinSize(Size)"/>
    public void SetMinSize(Size size) {
        MinWidth = size.Width;
        MinHeight = size.Height;
    }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureSize.SetMinHeight"/>
    public void SetMinHeight(int minHeight) {
        MinHeight = minHeight;
    }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureSize.SetMinWidth"/>
    public void SetMinWidth(int minWidth) {
        MinWidth = minWidth;
    }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureSize.UseOsDefaultSize"/>
    public void UseOsDefaultSize(bool enabled = true) {
        StartWithOsDefaultSize = enabled;
    }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureSize.SetResizable"/>
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