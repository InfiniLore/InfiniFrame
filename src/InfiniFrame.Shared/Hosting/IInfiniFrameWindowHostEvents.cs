// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Implemented by InfiniFrame for each window and passed to <see cref="IInfiniFrameHost.CreateWindowAsync" />. The
///     provider reports native facts through it.
/// </summary>
/// <remarks>
///     <para>
///         Every member is thread-safe, may be called from any provider thread, and never blocks on application code:
///         lifecycle and document state change immediately, while application callbacks are queued onto InfiniFrame's
///         bounded managed dispatcher in call order. A queue overflow faults that window; nothing is silently dropped.
///     </para>
///     <para>
///         <see cref="Closed" /> and <see cref="Faulted" /> are terminal. The first terminal call wins; later or duplicate
///         calls of any member are ignored and logged, and no window-host member is called after its disposal.
///     </para>
/// </remarks>
public interface IInfiniFrameWindowHostEvents {
    /// <summary>
    ///     The native window exists and the request's initial bounds, state, title and icon are applied. Call exactly
    ///     once. The window becomes ready (<c>WaitForReadyAsync</c>) only after this call.
    /// </summary>
    void Created();

    /// <summary>
    ///     A main-frame document is live and its bridge script is installed. <paramref name="document" /> must be non-zero
    ///     and greater than every id previously attached for this window; <paramref name="url" /> is the browser-reported
    ///     document URL (not a page-supplied claim) and supplies the origin InfiniFrame validates messages against.
    ///     Attaching implicitly retires the previous document.
    /// </summary>
    void DocumentAttached(InfiniFrameDocumentId document, Uri url);

    /// <summary>
    ///     The document is gone (navigation, reload or renderer loss). Its queued outbound work is cancelled; it is never
    ///     redirected to a later document.
    /// </summary>
    void DocumentRetired(InfiniFrameDocumentId document);

    /// <summary>
    ///     A message posted by the main frame of <paramref name="document" />. The provider must have validated natively
    ///     that it came from that live main-frame document. Messages for a document that is not current are rejected.
    /// </summary>
    void MessageReceived(InfiniFrameDocumentId document, string message);

    /// <summary>
    ///     Resolves one request for a scheme listed in <see cref="InfiniFrameHostWindowRequest.CustomSchemes" />.
    ///     Asset failures never throw: they become a status code (the resource's own status when its handler reports one,
    ///     for example 404 or 403 from the Blazor WebView manager; otherwise 404 missing or unhandled; 405 unsupported
    ///     method; 500 handler failure or invalid content type; 503 after the window closed). Throws
    ///     <see cref="OperationCanceledException" /> only when <paramref name="cancellationToken" /> is cancelled. The
    ///     application's handler runs on a thread-pool thread, never on the caller's thread, so it cannot block the
    ///     provider's loop. The returned response, including its body stream, is owned by the provider, which disposes it
    ///     once.
    /// </summary>
    ValueTask<InfiniFrameHostResourceResponse> GetResourceAsync(
        InfiniFrameHostResourceRequest request,
        CancellationToken cancellationToken
    );

    /// <summary>Native bounds changed. Call only when <see cref="InfiniFrameHostCapabilities.BoundsEvents" /> is declared.</summary>
    void BoundsChanged(Rectangle bounds);

    /// <summary>
    ///     Native state changed. Call only when <see cref="InfiniFrameHostCapabilities.StateEvents" /> or
    ///     <see cref="InfiniFrameHostCapabilities.FocusEvents" /> is declared.
    /// </summary>
    void StateChanged(InfiniFrameHostWindowState state);

    /// <summary>
    ///     The user or window system asked to close the window. Returns <see langword="false" /> when the application
    ///     cancelled closing. Call only when <see cref="InfiniFrameHostCapabilities.CloseCancellation" /> is declared, and
    ///     never block a thread InfiniFrame might need while awaiting it. Cancelling <paramref name="cancellationToken" />
    ///     abandons the request: the window stays open and the call throws <see cref="OperationCanceledException" />.
    /// </summary>
    ValueTask<bool> CloseRequestedAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Terminal window-level failure (for example, renderer loss or a corrupt per-window channel). InfiniFrame closes
    ///     this window's facade, raises its closed callbacks once and disposes the window host. Host-wide failure is
    ///     reported through <see cref="IInfiniFrameHost.Completion" /> instead.
    /// </summary>
    void Faulted(Exception exception);

    /// <summary>Terminal: the native window is gone. No further call will be made for this window.</summary>
    void Closed();
}
