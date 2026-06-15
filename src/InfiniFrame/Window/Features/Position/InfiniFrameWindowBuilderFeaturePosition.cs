// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowBuilderFeaturePosition : IInfiniFrameWindowBuilderFeaturePosition {
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeaturePosition.Top"/>
    public int Top { get; private set; }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeaturePosition.Left"/>
    public int Left { get; private set; }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeaturePosition.StartAtOsDefaultLocation"/>
    public bool StartAtOsDefaultLocation { get; private set; } = true;
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeaturePosition.StartCentered"/>
    public bool StartCentered { get; private set; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeaturePosition.SetLocation(int, int)"/>
    public void SetLocation(int left, int top) {
        StartAtOsDefaultLocation = false;
        Top = top;
        Left = left;
    }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeaturePosition.SetLocation(Point)"/>
    public void SetLocation(Point location) {
        StartAtOsDefaultLocation = false;
        Top = location.Y;
        Left = location.X;
    }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeaturePosition.SetLeft"/>
    public void SetLeft(int left) {
        StartAtOsDefaultLocation = false;
        Left = left;
    }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeaturePosition.SetTop"/>
    public void SetTop(int top) {
        StartAtOsDefaultLocation = false;
        Top = top;   
    }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeaturePosition.UseOsDefaultLocation"/>
    public void UseOsDefaultLocation(bool enabled) {
        StartAtOsDefaultLocation = enabled;
    }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeaturePosition.CenteredOnMainMonitor"/>
    public void CenteredOnMainMonitor(bool enabled) {
        if (enabled) StartAtOsDefaultLocation = false;
        StartCentered = enabled;
    }
    
    public void ApplyToNativeParameters(ref InfiniFrameNativeParameters parameters) {
        parameters.Top = Top;
        parameters.Left = Left;
        parameters.CenterOnInitialize = StartCentered;
        parameters.UseOsDefaultLocation = StartAtOsDefaultLocation;        
    }
}