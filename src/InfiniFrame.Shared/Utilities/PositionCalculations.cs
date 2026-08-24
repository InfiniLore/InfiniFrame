// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Pure calculation logic for window position operations.
/// </summary>
internal static class PositionCalculations {

    /// <summary>
    ///     Computes the centered position of a window within a monitor area.
    /// </summary>
    public static Point ComputeCenter(Rectangle monitorArea, int windowWidth, int windowHeight)
        => new(
            monitorArea.X + monitorArea.Width / 2 - windowWidth / 2,
            monitorArea.Y + monitorArea.Height / 2 - windowHeight / 2
        );

    /// <summary>
    ///     Clamps a window position so it remains fully within the monitor work area.
    /// </summary>
    public static (int Left, int Top) ClampToMonitorArea(
        int left,
        int top,
        int windowWidth,
        int windowHeight,
        Rectangle workArea
    ) {
        int horizontalWindowEdge = left + windowWidth;
        int verticalWindowEdge = top + windowHeight;

        int leftBound = workArea.X;
        int topBound = workArea.Y;
        int rightBound = workArea.X + workArea.Width;
        int bottomBound = workArea.Y + workArea.Height;

        left = horizontalWindowEdge > rightBound
            ? Math.Max(rightBound - windowWidth, leftBound)
            : Math.Max(left, leftBound);

        top = verticalWindowEdge > bottomBound
            ? Math.Max(bottomBound - windowHeight, topBound)
            : Math.Max(top, topBound);

        return (left, top);
    }
}
