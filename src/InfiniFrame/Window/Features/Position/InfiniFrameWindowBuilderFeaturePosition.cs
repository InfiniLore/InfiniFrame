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
    public int Top { get; private set; }
    public int Left { get; private set; }
    public bool StartAtOsDefaultLocation { get; private set; } = true;
    public bool StartCentered { get; private set; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void SetLocation(int left, int top) {
        StartAtOsDefaultLocation = false;
        Top = top;
        Left = left;
    }
    public void SetLocation(Point location) {
        StartAtOsDefaultLocation = false;
        Top = location.Y;
        Left = location.X;
    }
    public void SetLeft(int left) {
        StartAtOsDefaultLocation = false;
        Left = left;
    }
    public void SetTop(int top) {
        StartAtOsDefaultLocation = false;
        Top = top;   
    }
    public void UseOsDefaultLocation(bool enabled) {
        StartAtOsDefaultLocation = enabled;
    }
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