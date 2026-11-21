// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Hosting;

/// <summary>
///     Work bound to a document was cancelled because that document is no longer current. This is expected during
///     navigation and reload; the work is never redirected to a later document.
/// </summary>
public sealed class InfiniFrameHostDocumentRetiredException : OperationCanceledException {
    /// <summary>Creates the exception for <paramref name="document" />.</summary>
    public InfiniFrameHostDocumentRetiredException(InfiniFrameDocumentId document)
        : base($"Document {document.Value} is no longer current.") {
        Document = document;
    }

    /// <summary>The retired document, or the empty id when no document was ever attached.</summary>
    public InfiniFrameDocumentId Document { get; }
}
