// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Builder feature implementation for configuring the initial window position including location, centering,
///     and OS default location.
/// </summary>
public class PositionInfiniFrameWindowBuilderFeature : IPositionInfiniFrameWindowBuilderFeature {
    /// <inheritdoc cref="IPositionInfiniFrameWindowBuilderFeature.Top" />
    public int Top { get; private set; }
    /// <inheritdoc cref="IPositionInfiniFrameWindowBuilderFeature.Left" />
    public int Left { get; private set; }
    /// <inheritdoc cref="IPositionInfiniFrameWindowBuilderFeature.StartAtOsDefaultLocation" />
    public bool StartAtOsDefaultLocation { get; private set; } = true;
    /// <inheritdoc cref="IPositionInfiniFrameWindowBuilderFeature.StartCentered" />
    public bool StartCentered { get; private set; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IPositionInfiniFrameWindowBuilderFeature.SetLocation(int, int)" />
    public void SetLocation(int left, int top) {
        StartAtOsDefaultLocation = false;
        Top = top;
        Left = left;
    }
    /// <inheritdoc cref="IPositionInfiniFrameWindowBuilderFeature.SetLocation(Point)" />
    public void SetLocation(Point location) {
        StartAtOsDefaultLocation = false;
        Top = location.Y;
        Left = location.X;
    }
    /// <inheritdoc cref="IPositionInfiniFrameWindowBuilderFeature.SetLeft" />
    public void SetLeft(int left) {
        StartAtOsDefaultLocation = false;
        Left = left;
    }
    /// <inheritdoc cref="IPositionInfiniFrameWindowBuilderFeature.SetTop" />
    public void SetTop(int top) {
        StartAtOsDefaultLocation = false;
        Top = top;
    }
    /// <inheritdoc cref="IPositionInfiniFrameWindowBuilderFeature.UseOsDefaultLocation" />
    public void UseOsDefaultLocation(bool enabled) {
        StartAtOsDefaultLocation = enabled;
    }
    /// <inheritdoc cref="IPositionInfiniFrameWindowBuilderFeature.CenteredOnMainMonitor" />
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
