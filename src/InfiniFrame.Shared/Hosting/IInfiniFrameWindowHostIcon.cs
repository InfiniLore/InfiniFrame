// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Hosting;

/// <summary>Optional window-host operation for <see cref="InfiniFrameHostCapabilities.Icon" />.</summary>
public interface IInfiniFrameWindowHostIcon {
    /// <summary>
    ///     Applies a PNG icon. The provider copies <paramref name="png" /> before the task completes; InfiniFrame keeps
    ///     ownership of the memory. Throws <see cref="InfiniFrameHostOperationException" /> when it cannot be applied.
    /// </summary>
    ValueTask SetIconAsync(ReadOnlyMemory<byte> png, CancellationToken cancellationToken);
}
