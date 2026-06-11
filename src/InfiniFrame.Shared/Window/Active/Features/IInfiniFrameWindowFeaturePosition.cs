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
    
    void SetLocation(int left, int top);
    void SetLocation(Point location);
    void SetLeft(int left);
    void SetTop(int top);
    void Offset(int left, int top);
    void Offset(Point offset);
    void Offset(double left, double top);
    void Center();
    void CenterOnCurrentMonitor();
    void CenterOnMonitor(int monitorIndex);
    void MoveWithinCurrentMonitorArea(int left, int top);
    void MoveWithinCurrentMonitorArea(Point location);
    void MoveWithinCurrentMonitorArea(double left, double top);
}
