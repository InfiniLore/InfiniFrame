// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Components;

namespace InfiniLore.InfiniFrame.Js;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameJs {
    Task SetPointerCaptureAsync(ElementReference elementReference, long pointerId, CancellationToken ct = default);
    Task ReleasePointerCaptureAsync(ElementReference elementReference, long pointerId, CancellationToken ct = default);
}
