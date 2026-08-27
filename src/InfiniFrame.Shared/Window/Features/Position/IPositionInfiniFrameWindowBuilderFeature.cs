// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Configures initial window position before window creation.
/// </summary>
public interface IPositionInfiniFrameWindowBuilderFeature : IInfiniFrameWindowBuilderFeature {
    /// <summary>
    ///     Gets the configured top position of the window.
    /// </summary>
    int Top { get; }

    /// <summary>
    ///     Gets the configured left position of the window.
    /// </summary>
    int Left { get; }

    /// <summary>
    ///     Gets whether the window should start at the OS default location.
    /// </summary>
    bool StartAtOsDefaultLocation { get; }

    /// <summary>
    ///     Gets whether the window should start centered on the main monitor.
    /// </summary>
    bool StartCentered { get; }

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
    ///     Sets whether the window should use the OS default location.
    /// </summary>
    /// <param name="enabled">Whether to use the OS default location.</param>
    void UseOsDefaultLocation(bool enabled);

    /// <summary>
    ///     Sets whether the window should be centered on the main monitor.
    /// </summary>
    /// <param name="enabled">Whether to center on the main monitor.</param>
    void CenteredOnMainMonitor(bool enabled);
}
