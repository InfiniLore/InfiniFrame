// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class ILifecycleInfiniFrameWindowFeatureExtensions {
    public static ValueTask WaitForReadyAsync(this IInfiniFrameWindow window, CancellationToken ct = default)
        => window.Features.Lifecycle.WaitForReadyAsync(ct);

    public static void WaitForClose(this IInfiniFrameWindow window)
        => window.Features.Lifecycle.WaitForClose();

    public static ValueTask WaitForCloseAsync(this IInfiniFrameWindow window, CancellationToken ct = default)
        => window.Features.Lifecycle.WaitForCloseAsync(ct);

    public static void Close(this IInfiniFrameWindow window)
        => window.Features.Lifecycle.Close();

    public static ValueTask CloseAsync(this IInfiniFrameWindow window, CancellationToken ct = default)
        => window.Features.Lifecycle.CloseAsync(ct);

    public static ValueTask WaitForClosedCallbacksAsync(this IInfiniFrameWindow window, CancellationToken ct = default)
        => window.Features.Lifecycle.WaitForClosedCallbacksAsync(ct);

    public static ValueTask WaitForTeardownAsync(this IInfiniFrameWindow window, CancellationToken ct = default)
        => window.Features.Lifecycle.WaitForTeardownAsync(ct);

    public static bool IsClosedOrClosing(this IInfiniFrameWindow window)
        => window.Features.Lifecycle.IsClosedOrClosing();
}