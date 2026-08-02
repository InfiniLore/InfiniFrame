// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.BlazorWebView;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameBlazorApp : IAsyncDisposable {
    /// <summary>
    ///     Asynchronously runs the Blazor application and waits for the window to close.
    /// </summary>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RunAsync(CancellationToken ct = default);

    /// <summary>
    ///     Runs the Blazor application synchronously, blocking until the window closes.
    /// </summary>
    void Run();
}