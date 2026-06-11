// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IInfiniFrameWindowFeaturePositionExtensions {
    public static IInfiniFrameWindow SetLocation(this IInfiniFrameWindow window, int left, int top) {
        window.Features.Position.SetLocation(left, top);
        return window;
    }
    
    public static IInfiniFrameWindow SetLocation(this IInfiniFrameWindow window, Point location) {
        window.Features.Position.SetLocation(location);
        return window;
    }
    
    public static IInfiniFrameWindow SetLeft(this IInfiniFrameWindow window, int left) {
        window.Features.Position.SetLeft(left);
        return window;
    }
    
    public static IInfiniFrameWindow SetTop(this IInfiniFrameWindow window, int top) {
        window.Features.Position.SetTop(top);
        return window;
    }
    
    public static IInfiniFrameWindow Offset(this IInfiniFrameWindow window, int left, int top) {
        window.Features.Position.Offset(left, top);
        return window;
    }
    
    public static IInfiniFrameWindow Offset(this IInfiniFrameWindow window, Point offset) {
        window.Features.Position.Offset(offset);
        return window;
    }
    
    public static IInfiniFrameWindow Offset(this IInfiniFrameWindow window, double left, double top) {
        window.Features.Position.Offset(left, top);
        return window;
    }
    
    public static IInfiniFrameWindow Center(this IInfiniFrameWindow window) {
        window.Features.Position.Center();
        return window;
    }
    
    public static IInfiniFrameWindow CenterOnCurrentMonitor(this IInfiniFrameWindow window) {
        window.Features.Position.CenterOnCurrentMonitor();
        return window;
    }
    
    public static IInfiniFrameWindow CenterOnMonitor(this IInfiniFrameWindow window, int monitorIndex) {
        window.Features.Position.CenterOnMonitor(monitorIndex);
        return window;
    }
    
    public static IInfiniFrameWindow MoveWithinCurrentMonitorArea(this IInfiniFrameWindow window, int left, int top) {
        window.Features.Position.MoveWithinCurrentMonitorArea(left, top);
        return window;
    }
    
    public static IInfiniFrameWindow MoveWithinCurrentMonitorArea(this IInfiniFrameWindow window, Point location) {
        window.Features.Position.MoveWithinCurrentMonitorArea(location);
        return window;
    }
    
    public static IInfiniFrameWindow MoveWithinCurrentMonitorArea(this IInfiniFrameWindow window, double left, double top) {
        window.Features.Position.MoveWithinCurrentMonitorArea(left, top);
        return window;
    }
}
