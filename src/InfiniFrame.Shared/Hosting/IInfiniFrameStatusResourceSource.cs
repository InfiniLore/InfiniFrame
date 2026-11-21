// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Hosting;

/// <summary>
///     Internal: a core custom-scheme handler owner (the Blazor WebView manager) that can report the real status of a
///     resource. The public handler contract, <c>(Stream? Data, string? ContentType)</c>, cannot carry a status, so a
///     hosted window asks the handler's owner directly. Not visible to, or implemented by, providers.
/// </summary>
internal interface IInfiniFrameStatusResourceSource {
    /// <summary>Resolves <paramref name="url" /> with its status. A null body means no content for that status.</summary>
    (int StatusCode, Stream? Body, string? ContentType) GetResource(IInfiniFrameWindow? window, string? url);
}
