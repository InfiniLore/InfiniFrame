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
    public async Task SetPointerCaptureAsync(ElementReference elementReference, long pointerId, CancellationToken ct = default) {
        try {
            await jsRuntime.InvokeVoidAsync("infiniframe.utils.setPointerCapture", ct, elementReference, pointerId);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) {
            // ignore cancellation
        }
        catch (JSException ex) {
            logger.LogError(ex, "Something went wrong during setPointerCapture");
        }
        catch (InvalidOperationException ex) {
            logger.LogError(ex, "Something went wrong during setPointerCapture");
        }
    }

    public async Task ReleasePointerCaptureAsync(ElementReference elementReference, long pointerId, CancellationToken ct = default) {
        try {
            await jsRuntime.InvokeVoidAsync("infiniframe.utils.releasePointerCapture", ct, elementReference, pointerId);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) {
            // ignore cancellation
        }
        catch (JSException ex) {
            logger.LogError(ex, "Something went wrong during releasePointerCapture");
        }
        catch (InvalidOperationException ex) {
            logger.LogError(ex, "Something went wrong during releasePointerCapture");
        }
    }
}
