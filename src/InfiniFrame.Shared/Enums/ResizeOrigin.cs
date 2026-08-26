// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Specifies the origin edge or corner from which a window resize originates.
/// </summary>
public enum ResizeOrigin {
    /// <summary>
    ///     The top-left corner.
    /// </summary>
    TopLeft,
    /// <summary>
    ///     The top edge.
    /// </summary>
    Top,
    /// <summary>
    ///     The top-right corner.
    /// </summary>
    TopRight,
    /// <summary>
    ///     The right edge.
    /// </summary>
    Right,
    /// <summary>
    ///     The bottom-right corner.
    /// </summary>
    BottomRight,
    /// <summary>
    ///     The bottom edge.
    /// </summary>
    Bottom,
    /// <summary>
    ///     The bottom-left corner.
    /// </summary>
    BottomLeft,
    /// <summary>
    ///     The left edge.
    /// </summary>
    Left
}
