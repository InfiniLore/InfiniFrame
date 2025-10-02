// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace InfiniLore.Photino.Js;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniWindowJs(IJSRuntime jsRuntime, ILogger<InfiniWindowJs> logger) : IInfiniWindowJs {
    public async Task SetPointerCaptureAsync(ElementReference elementReference, long pointerId, CancellationToken ct = default) {
        try {
            await jsRuntime.InvokeVoidAsync("infiniWindow.setPointerCapture", ct, elementReference, pointerId);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Something went wrong during setPointerCapture");
        }
    }

    public async Task ReleasePointerCaptureAsync(ElementReference elementReference, long pointerId, CancellationToken ct = default) {
        try {
            await jsRuntime.InvokeVoidAsync("infiniWindow.releasePointerCapture", ct, elementReference, pointerId);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Something went wrong during releasePointerCapture");
        }
    }
}
