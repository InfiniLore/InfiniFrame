// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>Optional window-host operation for <see cref="InfiniFrameHostCapabilities.Bounds" />.</summary>
public interface IInfiniFrameWindowHostBounds {
    /// <summary>The actual native window bounds in screen coordinates.</summary>
    ValueTask<Rectangle> GetBoundsAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Completes once the actual native bounds equal <paramref name="bounds" />, or throws
    ///     <see cref="InfiniFrameHostOperationException" /> when the window system refused or the result is unobservable.
    /// </summary>
    ValueTask SetBoundsAsync(Rectangle bounds, CancellationToken cancellationToken);
}
