// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Hosting;

/// <summary>
///     One provider-owned native window with one main-frame browser, returned by
///     <see cref="IInfiniFrameHost.CreateWindowAsync" />. Optional operations are separate interfaces
///     (<see cref="IInfiniFrameWindowHostBounds" />, <see cref="IInfiniFrameWindowHostState" />,
///     <see cref="IInfiniFrameWindowHostTitle" />, <see cref="IInfiniFrameWindowHostIcon" />) and are used only when the
///     matching <see cref="InfiniFrameHostCapabilities" /> flag is declared.
/// </summary>
/// <remarks>
///     Threading: InfiniFrame may call any member from any thread, including its managed dispatcher thread, and may
///     block that thread on the returned task. A provider must therefore complete these tasks without waiting for the
///     InfiniFrame dispatcher (for example, never wait for an event callback to be processed).
///     Ownership: InfiniFrame calls <see cref="IAsyncDisposable.DisposeAsync" /> exactly once, after
///     <see cref="IInfiniFrameWindowHostEvents.Closed" /> or <see cref="IInfiniFrameWindowHostEvents.Faulted" /> has been
///     processed (or when creation is rolled back), and starts no further calls afterwards.
///     Every call is bounded: InfiniFrame cancels the token of a call it stops waiting for, and waits only a bounded
///     time for an in-flight <see cref="SendMessageAsync" /> before disposing. A call that ignores its token past that
///     deadline can therefore still be running when <see cref="IAsyncDisposable.DisposeAsync" /> starts; a provider
///     must tolerate that overlap (the late call may fail; its result is ignored).
/// </remarks>
public interface IInfiniFrameWindowHost : IAsyncDisposable {
    /// <summary>
    ///     Requests a native close without further confirmation. Completes when the request is delivered; completion of
    ///     the close is reported by <see cref="IInfiniFrameWindowHostEvents.Closed" />. Cancelling the token never
    ///     recalls a delivered request. It may be called again (for example by disposal after an application close);
    ///     later calls must be harmless.
    /// </summary>
    ValueTask RequestCloseAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Navigates the main frame to <paramref name="url" />, which InfiniFrame has checked against the window's trust
    ///     policy. Returns the document that will receive messages afterwards. For a cross-document navigation the
    ///     provider later reports <see cref="IInfiniFrameWindowHostEvents.DocumentAttached" /> with the same id.
    /// </summary>
    ValueTask<InfiniFrameHostNavigation> NavigateAsync(Uri url, CancellationToken cancellationToken);

    /// <summary>
    ///     Delivers one message, in order, to <paramref name="document" />'s receive callback (see
    ///     <see cref="InfiniFrameHostContract.BridgeContractVersion" />). Throws
    ///     <see cref="InfiniFrameHostDocumentRetiredException" /> when that document is no longer current; the message is
    ///     then not delivered to any other document. InfiniFrame serializes calls per window.
    /// </summary>
    ValueTask SendMessageAsync(InfiniFrameDocumentId document, string message, CancellationToken cancellationToken);

    /// <summary>The actual main-frame URL (not a cached request), or <see langword="null" /> before the first document.</summary>
    ValueTask<Uri?> GetCurrentUrlAsync(CancellationToken cancellationToken);
}
