// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace InfiniLore.InfiniFrame.Native;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents a 2D rectangle in a native (integer-based) coordinate system.
/// </summary>
[StructLayout(LayoutKind.Sequential), SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public struct NativeRect {
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}
