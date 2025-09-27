// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Components;

namespace InfiniLore.Photino.Js;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniWindowJs {
    Task SetPointerCaptureAsync(ElementReference elementReference, long pointerId, CancellationToken ct = default);
    Task ReleasePointerCaptureAsync(ElementReference elementReference, long pointerId, CancellationToken ct = default);
}
