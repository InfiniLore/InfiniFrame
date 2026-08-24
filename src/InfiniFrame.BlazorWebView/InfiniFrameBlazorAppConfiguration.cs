// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;

namespace InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Configuration for the Blazor application hosted inside an InfiniFrame WebView, including the base URI, host page,
///     and global exception handler settings.
/// </summary>
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class InfiniFrameBlazorAppConfiguration {
    /// <summary>Gets or sets the base URI for the Blazor application.</summary>
    public Uri AppBaseUri { get; set; } = new(InfiniFrameWebViewManager.AppBaseUri);

    /// <summary>Gets or sets the host page file name (e.g., <c>index.html</c>).</summary>
    public string HostPage { get; set; } = "index.html";

    /// <summary>Gets or sets whether the global unhandled exception handler is enabled.</summary>
    public bool EnableGlobalUnhandledExceptionHandler { get; set; } = true;

    /// <summary>
    ///     Gets or sets the maximum number of outbound messages waiting to be delivered to the native WebView.
    ///     A positive value is required. The default bounds memory while accommodating normal render bursts.
    ///     Increase this value for applications with high-frequency rendering updates; decrease for memory-constrained
    ///     scenarios.
    /// </summary>
    public int WebMessageQueueCapacity { get; set; } = 1_024;

    /// <summary>
    ///     Gets or sets how outbound messages are handled when <see cref="WebMessageQueueCapacity" /> is reached.
    ///     The default rejects the new message, which provides immediate backpressure to the non-awaitable Blazor API.
    ///     Note: The current implementation always uses <c>TryWrite</c> (non-blocking), so this setting only controls
    ///     diagnostic logging and is reserved for future use with blocking write paths.
    /// </summary>
    public BoundedChannelFullMode WebMessageQueueFullMode { get; set; } = BoundedChannelFullMode.DropWrite;
}
