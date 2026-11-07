// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Hosting;

/// <summary>
///     Optional window-host operation for <see cref="InfiniFrameHostCapabilities.WindowState" /> and
///     <see cref="InfiniFrameHostCapabilities.Focus" />.
/// </summary>
public interface IInfiniFrameWindowHostState {
    /// <summary>The actual native window state.</summary>
    ValueTask<InfiniFrameHostWindowState> GetStateAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Completes once the requested state is observable, or throws <see cref="InfiniFrameHostOperationException" />
    ///     when the window system refused it or it cannot be observed.
    /// </summary>
    ValueTask SetStateAsync(InfiniFrameHostWindowStateChange change, CancellationToken cancellationToken);
}
