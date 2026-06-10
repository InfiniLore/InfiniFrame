// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowBuilderFeaturePosition {
    int Top { get; }
    int Left { get; }
    bool StartAtOsDefaultLocation { get; }
    bool StartCentered { get; }
    
    void SetStartLocation(int left, int top);
    void SetStartLocation(Point location);
    void SetStartLeft(int left);
    void SetStartTop(int top);
    void UseOsDefaultLocation(bool enabled);
    void StartCenteredOnMainMonitor(bool enabled);
}
