// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Logging;

namespace InfiniFrame.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>Information passed to <see cref="IInfiniFrameHost.StartAsync" />.</summary>
public sealed class InfiniFrameHostStartContext {
    /// <summary>The application's root services. Owned by InfiniFrame: the provider may borrow them but never disposes them.</summary>
    public required IServiceProvider Services { get; init; }

    /// <summary>The application's logger factory. Owned by InfiniFrame.</summary>
    public required ILoggerFactory LoggerFactory { get; init; }
}
