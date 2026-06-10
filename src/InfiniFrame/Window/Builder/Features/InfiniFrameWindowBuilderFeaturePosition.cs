// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
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
    public void SetStartLocation(int left, int top) {
        StartAtOsDefaultLocation = false;
        Top = top;
        Left = left;
    }
    public void SetStartLocation(Point location) {
        StartAtOsDefaultLocation = false;
        Top = location.Y;
        Left = location.X;
    }
    public void SetStartLeft(int left) {
        StartAtOsDefaultLocation = false;
        Left = left;
    }
    public void SetStartTop(int top) {
        StartAtOsDefaultLocation = false;
        Top = top;   
    }
    public void UseOsDefaultLocation(bool enabled) {
        StartAtOsDefaultLocation = enabled;
    }
    public void StartCenteredOnMainMonitor(bool enabled) {
        if (enabled) StartAtOsDefaultLocation = false;
        StartCentered = enabled;
    }
}