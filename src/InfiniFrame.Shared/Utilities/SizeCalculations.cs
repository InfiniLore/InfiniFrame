// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Pure calculation logic for window resize operations.
///     Extracted from <see cref="SizeInfiniFrameWindowFeature"/> for testability.
/// </summary>
internal static class SizeCalculations {

    /// <summary>
    ///     Computes the new window bounds after a resize from a given origin.
    /// </summary>
    public static (int X, int Y, int Width, int Height) ComputeResize(
        int originalX, int originalY, int originalWidth, int originalHeight,
        int widthOffset, int heightOffset, ResizeOrigin origin
    ) {
        int x = originalX;
        int y = originalY;
        int width = originalWidth;
        int height = originalHeight;

        switch (origin) {
            case ResizeOrigin.TopLeft:
                x += widthOffset;
                y += heightOffset;
                width -= widthOffset;
                height -= heightOffset;
                break;

            case ResizeOrigin.Top:
                y += heightOffset;
                height -= heightOffset;
                break;

            case ResizeOrigin.TopRight:
                y += heightOffset;
                width += widthOffset;
                height -= heightOffset;
                break;

            case ResizeOrigin.Right:
                width += widthOffset;
                break;

            case ResizeOrigin.BottomRight:
                width += widthOffset;
                height += heightOffset;
                break;

            case ResizeOrigin.Bottom:
                height += heightOffset;
                break;

            case ResizeOrigin.BottomLeft:
                x += widthOffset;
                width -= widthOffset;
                height += heightOffset;
                break;

            case ResizeOrigin.Left:
                x += widthOffset;
                width -= widthOffset;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(origin), origin, null);
        }

        return (x, y, width, height);
    }

    /// <summary>
    ///     Clamps the computed resize bounds to min/max size constraints,
    ///     resetting position to original when clamped.
    /// </summary>
    public static (int X, int Y, int Width, int Height) ClampResize(
        int x, int y, int width, int height,
        int originalX, int originalY,
        Size minSize, Size maxSize
    ) {
        if (width >= maxSize.Width) {
            width = maxSize.Width;
            x = originalX;
        }

        if (height >= maxSize.Height) {
            height = maxSize.Height;
            y = originalY;
        }

        if (width <= minSize.Width) {
            width = minSize.Width;
            x = originalX;
        }

        if (height <= minSize.Height) {
            height = minSize.Height;
            y = originalY;
        }

        return (x, y, width, height);
    }
}
