// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Fluent extension methods for <see cref="IPositionInfiniFrameWindowFeature"/> on <see cref="IInfiniFrameWindow"/>.
/// </summary>
public static class IPositionInfiniFrameWindowFeatureExtensions {
    /// <param name="window">The window instance.</param>
    extension(IInfiniFrameWindow window) {
        /// <summary>
        ///     Sets the position of the window using pixel coordinates and returns the window for chaining.
        /// </summary>
        /// <param name="left">The left coordinate.</param>
        /// <param name="top">The top coordinate.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow SetLocation(int left, int top) {
            window.Features.Position.SetLocation(left, top);
            return window;
        }
        /// <summary>
        ///     Sets the position of the window using a <see cref="Point" /> and returns the window for chaining.
        /// </summary>
        /// <param name="location">The location point.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow SetLocation(Point location) {
            window.Features.Position.SetLocation(location);
            return window;
        }
        /// <summary>
        ///     Sets the left edge position of the window and returns the window for chaining.
        /// </summary>
        /// <param name="left">The left coordinate.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow SetLeft(int left) {
            window.Features.Position.SetLeft(left);
            return window;
        }
        /// <summary>
        ///     Sets the top edge position of the window and returns the window for chaining.
        /// </summary>
        /// <param name="top">The top coordinate.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow SetTop(int top) {
            window.Features.Position.SetTop(top);
            return window;
        }
        /// <summary>
        ///     Offsets the window position by the specified amount and returns the window for chaining.
        /// </summary>
        /// <param name="left">The horizontal offset.</param>
        /// <param name="top">The vertical offset.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow Offset(int left, int top) {
            window.Features.Position.Offset(left, top);
            return window;
        }
        /// <summary>
        ///     Offsets the window position by the specified <see cref="Point" /> and returns the window for chaining.
        /// </summary>
        /// <param name="offset">The offset point.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow Offset(Point offset) {
            window.Features.Position.Offset(offset);
            return window;
        }
        /// <summary>
        ///     Offsets the window position by the specified amount and returns the window for chaining.
        /// </summary>
        /// <param name="left">The horizontal offset.</param>
        /// <param name="top">The vertical offset.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow Offset(double left, double top) {
            window.Features.Position.Offset(left, top);
            return window;
        }
        /// <summary>
        ///     Centers the window on the screen and returns the window for chaining.
        /// </summary>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow Center() {
            window.Features.Position.Center();
            return window;
        }
        /// <summary>
        ///     Centers the window on the current monitor and returns the window for chaining.
        /// </summary>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow CenterOnCurrentMonitor() {
            window.Features.Position.CenterOnCurrentMonitor();
            return window;
        }
        /// <summary>
        ///     Centers the window on the specified monitor and returns the window for chaining.
        /// </summary>
        /// <param name="monitorIndex">The index of the monitor.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow CenterOnMonitor(int monitorIndex) {
            window.Features.Position.CenterOnMonitor(monitorIndex);
            return window;
        }
        /// <summary>
        ///     Moves the window within the current monitor's work area using pixel coordinates and returns the window for
        ///     chaining.
        /// </summary>
        /// <param name="left">The left coordinate.</param>
        /// <param name="top">The top coordinate.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow MoveWithinCurrentMonitorArea(int left, int top) {
            window.Features.Position.MoveWithinCurrentMonitorArea(left, top);
            return window;
        }
        /// <summary>
        ///     Moves the window within the current monitor's work area using a <see cref="Point" /> and returns the window for
        ///     chaining.
        /// </summary>
        /// <param name="location">The location point.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow MoveWithinCurrentMonitorArea(Point location) {
            window.Features.Position.MoveWithinCurrentMonitorArea(location);
            return window;
        }
        /// <summary>
        ///     Moves the window within the current monitor's work area using pixel coordinates and returns the window for
        ///     chaining.
        /// </summary>
        /// <param name="left">The left coordinate.</param>
        /// <param name="top">The top coordinate.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow MoveWithinCurrentMonitorArea(double left, double top) {
            window.Features.Position.MoveWithinCurrentMonitorArea(left, top);
            return window;
        }
    }

}
