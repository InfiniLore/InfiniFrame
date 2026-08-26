// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides runtime control over window position including absolute placement, centering, and monitor-aware positioning.
/// </summary>
public interface IPositionInfiniFrameWindowFeature {
    /// <summary>
    ///     Gets the current position of the window.
    /// </summary>
    Point Location { get; }

    /// <summary>
    ///     Gets the top edge position of the window.
    /// </summary>
    int Top { get; }

    /// <summary>
    ///     Gets the left edge position of the window.
    /// </summary>
    int Left { get; }

    /// <summary>
    ///     Sets the position of the window using pixel coordinates.
    /// </summary>
    /// <param name="left">The left coordinate.</param>
    /// <param name="top">The top coordinate.</param>
    void SetLocation(int left, int top);

    /// <summary>
    ///     Sets the position of the window using a <see cref="Point" />.
    /// </summary>
    /// <param name="location">The location point.</param>
    void SetLocation(Point location);

    /// <summary>
    ///     Sets the left edge position of the window.
    /// </summary>
    /// <param name="left">The left coordinate.</param>
    void SetLeft(int left);

    /// <summary>
    ///     Sets the top edge position of the window.
    /// </summary>
    /// <param name="top">The top coordinate.</param>
    void SetTop(int top);

    /// <summary>
    ///     Offsets the window position by the specified amount.
    /// </summary>
    /// <param name="left">The horizontal offset.</param>
    /// <param name="top">The vertical offset.</param>
    void Offset(int left, int top);

    /// <summary>
    ///     Offsets the window position by the specified amount.
    /// </summary>
    /// <param name="offset">The offset point.</param>
    void Offset(Point offset);

    /// <summary>
    ///     Offsets the window position by the specified amount.
    /// </summary>
    /// <param name="left">The horizontal offset.</param>
    /// <param name="top">The vertical offset.</param>
    void Offset(double left, double top);

    /// <summary>
    ///     Centers the window on the screen.
    /// </summary>
    void Center();

    /// <summary>
    ///     Centers the window on the current monitor.
    /// </summary>
    void CenterOnCurrentMonitor();

    /// <summary>
    ///     Centers the window on the specified monitor.
    /// </summary>
    /// <param name="monitorIndex">The index of the monitor.</param>
    void CenterOnMonitor(int monitorIndex);

    /// <summary>
    ///     Moves the window within the current monitor's work area using pixel coordinates.
    /// </summary>
    /// <param name="left">The left coordinate.</param>
    /// <param name="top">The top coordinate.</param>
    void MoveWithinCurrentMonitorArea(int left, int top);

    /// <summary>
    ///     Moves the window within the current monitor's work area using a <see cref="Point" />.
    /// </summary>
    /// <param name="location">The location point.</param>
    void MoveWithinCurrentMonitorArea(Point location);

    /// <summary>
    ///     Moves the window within the current monitor's work area using pixel coordinates.
    /// </summary>
    /// <param name="left">The left coordinate.</param>
    /// <param name="top">The top coordinate.</param>
    void MoveWithinCurrentMonitorArea(double left, double top);
}
