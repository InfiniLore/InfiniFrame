// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowFeaturePosition {
    Point Location { get; }
    int Top { get; }
    int Left { get; }
    
    IInfiniFrameWindow SetLocation(int left, int top);
    IInfiniFrameWindow SetLocation(Point location);
    IInfiniFrameWindow SetLeft(int left);
    IInfiniFrameWindow SetTop(int top);
    IInfiniFrameWindow Offset(int left, int top);
    IInfiniFrameWindow Offset(Point offset);
    IInfiniFrameWindow Offset(double left, double top);
    IInfiniFrameWindow Center();
    IInfiniFrameWindow CenterOnCurrentMonitor();
    IInfiniFrameWindow CenterOnMonitor(int monitorIndex);
    IInfiniFrameWindow MoveWithinCurrentMonitorArea(int left, int top);
    IInfiniFrameWindow MoveWithinCurrentMonitorArea(Point location);
    IInfiniFrameWindow MoveWithinCurrentMonitorArea(double left, double top);
}
