// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Net;
using System.Net.Http.Headers;

namespace InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     An <see cref="HttpMessageHandler" /> that intercepts HTTP requests and routes them through the
///     <see cref="IInfiniFrameWebViewManager" /> for custom scheme handling, falling back to the default handler
///     for unhandled requests.
/// </summary>
public class InfiniFrameHttpHandler : DelegatingHandler {
    private readonly IInfiniFrameWebViewManager _manager;

    /// <summary>
    ///     Initializes a new instance of the <see cref="InfiniFrameHttpHandler" /> class.
    /// </summary>
    /// <param name="manager">The WebView manager used to handle custom scheme requests.</param>
    /// <param name="innerHandler">The inner handler for unhandled HTTP requests. Defaults to <see cref="HttpClientHandler" />.</param>
    public InfiniFrameHttpHandler(IInfiniFrameWebViewManager manager, HttpMessageHandler? innerHandler = null) {
        _manager = manager;

        //the last (inner) handler in the pipeline should be a "real" handler.
        //To make an HTTP request, create a HttpClientHandler instance.
        InnerHandler = innerHandler ?? new HttpClientHandler();
    }

    /// <summary>
    ///     Sends an HTTP request through the handler pipeline, routing custom scheme requests through the WebView manager.
    /// </summary>
    /// <remarks>
    ///     When the request is handled by the WebView manager, the returned <see cref="HttpResponseMessage" />
    ///     owns the underlying <see cref="Stream" />. The caller is responsible for disposing the response.
    ///     If the caller fails to dispose it (e.g., due to an exception during component rendering), the
    ///     stream will remain open until garbage collected. The Blazor framework typically handles disposal
    ///     via its component lifecycle.
    /// </remarks>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The HTTP response message.</returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
        (Stream? Data, string? ContentType) result = _manager.HandleWebRequest(null, request.RequestUri?.AbsoluteUri);
        if (result is not ({ } content, { } contentType))
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        cancellationToken.ThrowIfCancellationRequested();
        var response = new HttpResponseMessage(HttpStatusCode.OK) {
            Content = new StreamContent(content)
        };
        response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        return response;
    }
}
