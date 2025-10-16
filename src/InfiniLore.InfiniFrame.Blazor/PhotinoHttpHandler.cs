using System.Net;
using System.Net.Http.Headers;

namespace InfiniLore.InfiniFrame.Blazor;
public class PhotinoHttpHandler : DelegatingHandler {
    private readonly IPhotinoWebViewManager _manager;

    public PhotinoHttpHandler(IPhotinoWebViewManager manager, HttpMessageHandler? innerHandler = null) {
        _manager = manager;

        //the last (inner) handler in the pipeline should be a "real" handler.
        //To make an HTTP request, create a HttpClientHandler instance.
        InnerHandler = innerHandler ?? new HttpClientHandler();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
        Stream? content = _manager.HandleWebRequest(null, null, request.RequestUri?.AbsoluteUri, out string? contentType);
        if (content is null || contentType is null) return await base.SendAsync(request, cancellationToken);

        var response = new HttpResponseMessage(HttpStatusCode.OK);
        response.Content = new StreamContent(content);
        response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        return response;

    }
}
