// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

namespace InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
/// <summary>
///     Configuration for the Blazor application hosted inside an InfiniFrame WebView, including the base URI, host page,
///     and global exception handler settings.
/// </summary>
public class InfiniFrameBlazorAppConfiguration {
    /// <summary>Gets or sets the base URI for the Blazor application.</summary>
    public Uri AppBaseUri { get; set; } = new(InfiniFrameWebViewManager.AppBaseUri);
    /// <summary>Gets or sets the host page file name (e.g., <c>index.html</c>).</summary>
    public string HostPage { get; set; } = "index.html";
    /// <summary>Gets or sets whether the global unhandled exception handler is enabled.</summary>
    public bool EnableGlobalUnhandledExceptionHandler { get; set; } = true;
}
