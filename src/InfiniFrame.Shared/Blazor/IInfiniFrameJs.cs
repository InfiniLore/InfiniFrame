// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Components;

namespace InfiniFrame.Blazor;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameJs {
    Task SetPointerCaptureAsync(ElementReference elementReference, long pointerId, CancellationToken ct = default);
    Task ReleasePointerCaptureAsync(ElementReference elementReference, long pointerId, CancellationToken ct = default);
}
