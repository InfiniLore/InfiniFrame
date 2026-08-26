// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Components;

namespace InfiniFrame.Blazor;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameJs {
    /// <summary>
    ///     Sets the pointer capture to a specific element for the given pointer ID.
    /// </summary>
    /// <param name="elementReference">The element reference to capture the pointer on.</param>
    /// <param name="pointerId">The pointer ID to capture.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SetPointerCaptureAsync(ElementReference elementReference, long pointerId, CancellationToken ct = default);

    /// <summary>
    ///     Releases the pointer capture for the given pointer ID from the specified element.
    /// </summary>
    /// <param name="elementReference">The element reference to release the pointer capture from.</param>
    /// <param name="pointerId">The pointer ID to release.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ReleasePointerCaptureAsync(ElementReference elementReference, long pointerId, CancellationToken ct = default);
}
