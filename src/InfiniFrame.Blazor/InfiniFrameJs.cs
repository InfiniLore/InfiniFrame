// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace InfiniFrame.Blazor;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Implementation of <see cref="IInfiniFrameJs"/> providing pointer capture interop for Blazor components.
/// </summary>
public class InfiniFrameJs(IJSRuntime jsRuntime, ILogger<InfiniFrameJs> logger) : IInfiniFrameJs {
    /// <inheritdoc cref="IInfiniFrameJs.SetPointerCaptureAsync" />
    public async Task SetPointerCaptureAsync(ElementReference elementReference, long pointerId, CancellationToken ct = default) {
        try {
            await jsRuntime.InvokeVoidAsync("infiniframe.utils.setPointerCapture", ct, elementReference, pointerId);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) {
            // ignore cancellation
        }
        catch (JSDisconnectedException) {
            // The page is gone (for example a document replaced in a hosted window): nothing to capture, nothing failed.
            logger.LogDebug("setPointerCapture skipped for pointerId {PointerId}: the page is disconnected", pointerId);
        }
        catch (Exception ex) when (ex is JSException or InvalidOperationException) {
            logger.LogError(ex, "setPointerCapture failed for pointerId {PointerId}", pointerId);
        }
    }

    /// <inheritdoc cref="IInfiniFrameJs.ReleasePointerCaptureAsync" />
    public async Task ReleasePointerCaptureAsync(ElementReference elementReference, long pointerId, CancellationToken ct = default) {
        try {
            await jsRuntime.InvokeVoidAsync("infiniframe.utils.releasePointerCapture", ct, elementReference, pointerId);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) {
            // ignore cancellation
        }
        catch (JSDisconnectedException) {
            // The page is gone (for example a document replaced in a hosted window): nothing to release, nothing failed.
            logger.LogDebug("releasePointerCapture skipped for pointerId {PointerId}: the page is disconnected", pointerId);
        }
        catch (Exception ex) when (ex is JSException or InvalidOperationException) {
            logger.LogError(ex, "releasePointerCapture failed for pointerId {PointerId}", pointerId);
        }
    }
}
