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
public class InfiniFrameJs(IJSRuntime jsRuntime, ILogger<InfiniFrameJs> logger) : IInfiniFrameJs {
    /// <inheritdoc cref="IInfiniFrameJs.SetPointerCaptureAsync"/>
    public async Task SetPointerCaptureAsync(ElementReference elementReference, long pointerId, CancellationToken ct = default) {
        try {
            await jsRuntime.InvokeVoidAsync("infiniframe.utils.setPointerCapture", ct, elementReference, pointerId);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) {
            // ignore cancellation
        }
        catch (Exception ex) when (ex is JSException or InvalidOperationException) {
            logger.LogError(ex, "Something went wrong during setPointerCapture");
        }
    }

    /// <inheritdoc cref="IInfiniFrameJs.ReleasePointerCaptureAsync"/>
    public async Task ReleasePointerCaptureAsync(ElementReference elementReference, long pointerId, CancellationToken ct = default) {
        try {
            await jsRuntime.InvokeVoidAsync("infiniframe.utils.releasePointerCapture", ct, elementReference, pointerId);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) {
            // ignore cancellation
        }
        catch (Exception ex) when (ex is JSException or InvalidOperationException) {
            logger.LogError(ex, "Something went wrong during releasePointerCapture");
        }
    }
}
