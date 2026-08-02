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
    /// <param name="request">The HTTP request message.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The HTTP response message.</returns>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
        (Stream? Data, string? ContentType) result = _manager.HandleWebRequest(null, request.RequestUri?.AbsoluteUri);
        if (result is not ( { } content, { } contentType))
            return base.SendAsync(request, cancellationToken);

        var response = new HttpResponseMessage(HttpStatusCode.OK);
        response.Content = new StreamContent(content);
        response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        return Task.FromResult(response);
    }
}