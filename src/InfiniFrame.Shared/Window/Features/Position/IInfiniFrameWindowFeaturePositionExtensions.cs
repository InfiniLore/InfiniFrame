// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IInfiniFrameWindowFeaturePositionExtensions {
    /// <summary>
    ///     Sets the position of the window using pixel coordinates and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="left">The left coordinate.</param>
    /// <param name="top">The top coordinate.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow SetLocation(this IInfiniFrameWindow window, int left, int top) {
        window.Features.Position.SetLocation(left, top);
        return window;
    }
    
    /// <summary>
    ///     Sets the position of the window using a <see cref="Point"/> and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="location">The location point.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow SetLocation(this IInfiniFrameWindow window, Point location) {
        window.Features.Position.SetLocation(location);
        return window;
    }
    
    /// <summary>
    ///     Sets the left edge position of the window and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="left">The left coordinate.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow SetLeft(this IInfiniFrameWindow window, int left) {
        window.Features.Position.SetLeft(left);
        return window;
    }
    
    /// <summary>
    ///     Sets the top edge position of the window and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="top">The top coordinate.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow SetTop(this IInfiniFrameWindow window, int top) {
        window.Features.Position.SetTop(top);
        return window;
    }
    
    /// <summary>
    ///     Offsets the window position by the specified amount and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="left">The horizontal offset.</param>
    /// <param name="top">The vertical offset.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow Offset(this IInfiniFrameWindow window, int left, int top) {
        window.Features.Position.Offset(left, top);
        return window;
    }
    
    /// <summary>
    ///     Offsets the window position by the specified <see cref="Point"/> and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="offset">The offset point.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow Offset(this IInfiniFrameWindow window, Point offset) {
        window.Features.Position.Offset(offset);
        return window;
    }
    
    /// <summary>
    ///     Offsets the window position by the specified amount and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="left">The horizontal offset.</param>
    /// <param name="top">The vertical offset.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow Offset(this IInfiniFrameWindow window, double left, double top) {
        window.Features.Position.Offset(left, top);
        return window;
    }
    
    /// <summary>
    ///     Centers the window on the screen and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow Center(this IInfiniFrameWindow window) {
        window.Features.Position.Center();
        return window;
    }
    
    /// <summary>
    ///     Centers the window on the current monitor and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow CenterOnCurrentMonitor(this IInfiniFrameWindow window) {
        window.Features.Position.CenterOnCurrentMonitor();
        return window;
    }
    
    /// <summary>
    ///     Centers the window on the specified monitor and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="monitorIndex">The index of the monitor.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow CenterOnMonitor(this IInfiniFrameWindow window, int monitorIndex) {
        window.Features.Position.CenterOnMonitor(monitorIndex);
        return window;
    }
    
    /// <summary>
    ///     Moves the window within the current monitor's work area using pixel coordinates and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="left">The left coordinate.</param>
    /// <param name="top">The top coordinate.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow MoveWithinCurrentMonitorArea(this IInfiniFrameWindow window, int left, int top) {
        window.Features.Position.MoveWithinCurrentMonitorArea(left, top);
        return window;
    }
    
    /// <summary>
    ///     Moves the window within the current monitor's work area using a <see cref="Point"/> and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="location">The location point.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow MoveWithinCurrentMonitorArea(this IInfiniFrameWindow window, Point location) {
        window.Features.Position.MoveWithinCurrentMonitorArea(location);
        return window;
    }
    
    /// <summary>
    ///     Moves the window within the current monitor's work area using pixel coordinates and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="left">The left coordinate.</param>
    /// <param name="top">The top coordinate.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow MoveWithinCurrentMonitorArea(this IInfiniFrameWindow window, double left, double top) {
        window.Features.Position.MoveWithinCurrentMonitorArea(left, top);
        return window;
    }
}
