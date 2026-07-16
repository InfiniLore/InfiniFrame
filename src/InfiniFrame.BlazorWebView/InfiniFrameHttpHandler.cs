// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Net;
using System.Net.Http.Headers;

namespace InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameHttpHandler : DelegatingHandler {
    private readonly IInfiniFrameWebViewManager _manager;

    public InfiniFrameHttpHandler(IInfiniFrameWebViewManager manager, HttpMessageHandler? innerHandler = null) {
        _manager = manager;

        //the last (inner) handler in the pipeline should be a "real" handler.
        //To make an HTTP request, create a HttpClientHandler instance.
        InnerHandler = innerHandler ?? new HttpClientHandler();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
        (Stream? Data, string? ContentType) result = _manager.HandleWebRequest(null, request.RequestUri?.AbsoluteUri);
        if (result is not ({} content, {} contentType)) return await base.SendAsync(request, cancellationToken);

        var response = new HttpResponseMessage(HttpStatusCode.OK);
        response.Content = new StreamContent(content);
        response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        return response;

    }
}
