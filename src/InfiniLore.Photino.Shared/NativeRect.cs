using System.Runtime.InteropServices;

namespace InfiniLore.Photino;

/// <summary>
///     Represents a 2D rectangle in a native (integer-based) coordinate system.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct NativeRect
{
    public int x, y;
    public int width, height;
}
