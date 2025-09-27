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
public class InfiniWindowJs(IJSRuntime jsRuntime, ILogger<InfiniWindowJs> logger) {

    public async Task SetPointerCaptureAsync(ElementReference elementReference, long pointerId) {
        try {
            await jsRuntime.InvokeVoidAsync("infiniWindow.setPointerCapture", elementReference, pointerId);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Something went wrong during setPointerCapture");
        }
    }

    public async Task ReleasePointerCaptureAsync(ElementReference elementReference, long pointerId) {
        try {
            await jsRuntime.InvokeVoidAsync("infiniWindow.releasePointerCapture", elementReference, pointerId);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Something went wrong during releasePointerCapture");
        }
    }
}
