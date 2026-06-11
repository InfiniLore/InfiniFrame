// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowBuilderFeaturePosition : IInfiniFrameWindowBuilderFeature{
    int Top { get; }
    int Left { get; }
    bool StartAtOsDefaultLocation { get; }
    bool StartCentered { get; }
    
    void SetLocation(int left, int top);
    void SetLocation(Point location);
    void SetLeft(int left);
    void SetTop(int top);
    void UseOsDefaultLocation(bool enabled);
    void CenteredOnMainMonitor(bool enabled);
}
