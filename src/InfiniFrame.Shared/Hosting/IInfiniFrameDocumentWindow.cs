// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Hosting;

/// <summary>
///     Internal document-aware delivery contract between the core hosted-window facade and the Blazor WebView manager.
///     Not implemented or seen by providers. Document ids are <see cref="InfiniFrameDocumentId.Value" />; 0 means none.
/// </summary>
internal interface IInfiniFrameDocumentWindow {
    /// <summary>The current live document, or 0.</summary>
    ulong CurrentDocument { get; }

    /// <summary>
    ///     The document whose inbound message callback produced the current logical call path (flows with the
    ///     execution context), or <see langword="null" /> outside such a callback.
    /// </summary>
    ulong? IncomingMessageDocument { get; }

    /// <summary>Delivers to exactly <paramref name="document" /> or throws <see cref="InfiniFrameHostDocumentRetiredException" />.</summary>
    ValueTask SendWebMessageForDocumentAsync(string message, ulong document, CancellationToken cancellationToken);

    /// <summary>
    ///     The lifetime of <paramref name="document" />: cancelled when that document retires or the window ends, and
    ///     already cancelled when it is not the current document. Work owned by a document is bound to this token.
    /// </summary>
    CancellationToken GetDocumentLifetime(ulong document);

    /// <summary>Faults the window because its delivery path cannot continue without losing messages.</summary>
    void FaultWindow(Exception exception);
}
