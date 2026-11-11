// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Hosting;

/// <summary>Optional window-host operation for <see cref="InfiniFrameHostCapabilities.Title" />.</summary>
public interface IInfiniFrameWindowHostTitle {
    /// <summary>The actual native window title.</summary>
    ValueTask<string> GetTitleAsync(CancellationToken cancellationToken);

    /// <summary>Completes once the native title is applied, or throws <see cref="InfiniFrameHostOperationException" />.</summary>
    ValueTask SetTitleAsync(string title, CancellationToken cancellationToken);
}
