// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace InfiniFrame.NativeBridge;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents a 2D rectangle in a native (integer-based) coordinate system.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public struct NativeRect {
    /// <summary>
    ///     The x-coordinate of the top-left corner of the rectangle.
    /// </summary>
    public int X { get; set; }
    /// <summary>
    ///     The y-coordinate of the top-left corner of the rectangle.
    /// </summary>
    public int Y { get; set; }
    /// <summary>
    ///     The width of the rectangle in pixels.
    /// </summary>
    public int Width { get; set; }
    /// <summary>
    ///     The height of the rectangle in pixels.
    /// </summary>
    public int Height { get; set; }
}
