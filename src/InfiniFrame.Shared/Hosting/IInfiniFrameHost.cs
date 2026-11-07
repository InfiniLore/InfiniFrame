// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Hosting;

/// <summary>
///     The process-wide host created by an <see cref="IInfiniFrameHostProvider" />. It owns the native windows, their
///     native loop or companion process, and the browser engine. InfiniFrame never runs application callbacks on the
///     provider's threads: every notification the provider raises is queued onto InfiniFrame's managed dispatcher.
/// </summary>
/// <remarks>
///     Ownership: InfiniFrame calls <see cref="IAsyncDisposable.DisposeAsync" /> exactly once, after every window host
///     has been disposed and after <see cref="ShutdownAsync" />, including when start-up fails part-way. InfiniFrame never
///     calls <see cref="StartAsync" />, <see cref="CreateWindowAsync" /> or <see cref="ShutdownAsync" /> concurrently with
///     each other or with disposal.
/// </remarks>
public interface IInfiniFrameHost : IAsyncDisposable {
    /// <summary>
    ///     Starts native resources (for example, launches and authenticates a companion process). Called once from
    ///     <c>InfiniFrameApplication.Run()</c>/<c>RunAsync()</c> before any window is created. A failure fails the run.
    /// </summary>
    ValueTask StartAsync(InfiniFrameHostStartContext context, CancellationToken cancellationToken);

    /// <summary>
    ///     Creates one native window and its main-frame browser for <paramref name="request" /> and binds it to
    ///     <paramref name="events" />. Completes once the request is accepted; readiness is reported later through
    ///     <see cref="IInfiniFrameWindowHostEvents.Created" />. Throws <see cref="NotSupportedException" /> before creating
    ///     anything when the request cannot be honoured exactly. Events may be raised before this method returns.
    /// </summary>
    ValueTask<IInfiniFrameWindowHost> CreateWindowAsync(
        InfiniFrameHostWindowRequest request,
        IInfiniFrameWindowHostEvents events,
        CancellationToken cancellationToken
    );

    /// <summary>
    ///     Completes after an orderly stop. Faults when the host can no longer serve any window (process loss, corrupt
    ///     or untrusted session). Completion before InfiniFrame requests shutdown ends the application run; a fault is
    ///     rethrown from <c>Run()</c>. A recoverable per-request failure (for example, one resource) must not fault it.
    /// </summary>
    Task Completion { get; }

    /// <summary>Begins an orderly stop of the host after InfiniFrame has closed its windows. Idempotent.</summary>
    ValueTask ShutdownAsync(CancellationToken cancellationToken);
}
