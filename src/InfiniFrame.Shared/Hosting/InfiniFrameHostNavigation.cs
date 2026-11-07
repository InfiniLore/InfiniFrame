// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Hosting;

/// <summary>The accepted result of <see cref="IInfiniFrameWindowHost.NavigateAsync" />.</summary>
/// <param name="Document">The document that receives messages after the navigation.</param>
/// <param name="SameDocument">
///     <see langword="true" /> for a same-document navigation (for example, a fragment change): <paramref name="Document" />
///     is the current document. Otherwise the provider reports <see cref="IInfiniFrameWindowHostEvents.DocumentAttached" />
///     for <paramref name="Document" />.
/// </param>
public readonly record struct InfiniFrameHostNavigation(InfiniFrameDocumentId Document, bool SameDocument);
