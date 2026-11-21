// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Hosting;

/// <summary>Information passed to <see cref="IInfiniFrameHostProvider.CreateHost" />.</summary>
public sealed class InfiniFrameHostCreateContext {
    /// <summary>Identity of the InfiniFrame application that owns the host.</summary>
    public required Guid ApplicationId { get; init; }
}
