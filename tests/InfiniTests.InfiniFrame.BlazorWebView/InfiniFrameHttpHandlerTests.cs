// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BlazorWebView;

namespace InfiniTests.InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameHttpHandlerTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Constructor_WithNullManager_ShouldThrow(CancellationToken ct = default) {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => Task.Run(() => {
            new InfiniFrameHttpHandler(null!);
        }));

        // Assert
        await Assert.That(exception).IsNotNull();
        await Assert.That(exception!.ParamName).IsEqualTo("manager");
    }

    [Test]
    public async Task Constructor_WithManager_ShouldNotThrow(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWebViewManager> managerMock = MockFactory.CreateWebViewManagerMock();
        var innerHandler = new HttpClientHandler();

        // Act
        var handler = new InfiniFrameHttpHandler(managerMock.Object, innerHandler);

        // Assert
        await Assert.That(handler).IsNotNull();
    }

    [Test]
    public async Task SendAsync_WithHandledRequest_ShouldReturnStreamResponse(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWebViewManager> managerMock = MockFactory.CreateWebViewManagerMock();
        var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        managerMock.HandleWebRequest(Any<global::InfiniFrame.IInfiniFrameWindow?>(), Any<string?>()).Returns((stream, "text/plain"));
        var handler = new InfiniFrameHttpHandler(managerMock.Object, new HttpClientHandler());
        var httpClient = new HttpClient(handler);
        var request = new HttpRequestMessage(HttpMethod.Get, "app://localhost/test");

        // Act
        HttpResponseMessage response = await httpClient.SendAsync(request, CancellationToken.None);

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);
        await Assert.That(response.Content).IsNotNull();
        await Assert.That(response.Content.Headers.ContentType!.MediaType).IsEqualTo("text/plain");
    }

    [Test]
    public async Task SendAsync_WithUnhandledRequest_ShouldFallThroughToInnerHandler(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWebViewManager> managerMock = MockFactory.CreateWebViewManagerMock();
        managerMock.HandleWebRequest(Any<global::InfiniFrame.IInfiniFrameWindow?>(), Any<string?>()).Returns(((Stream?)null, (string?)null));
        var handler = new InfiniFrameHttpHandler(managerMock.Object, new ThrowingHttpHandler());
        var httpClient = new HttpClient(handler);
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com/test");

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(async () => {
            await httpClient.SendAsync(request, CancellationToken.None);
        });
    }

    private sealed class ThrowingHttpHandler : HttpMessageHandler {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            throw new HttpRequestException("inner handler rejected");
        }
    }

    [Test]
    public async Task SendAsync_WithCancellationRequested_ShouldThrow(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWebViewManager> managerMock = MockFactory.CreateWebViewManagerMock();
        var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        managerMock.HandleWebRequest(Any<global::InfiniFrame.IInfiniFrameWindow?>(), Any<string?>()).Returns((stream, "text/plain"));
        var handler = new InfiniFrameHttpHandler(managerMock.Object, new HttpClientHandler());
        var httpClient = new HttpClient(handler);
        var request = new HttpRequestMessage(HttpMethod.Get, "app://localhost/test");
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () => {
            await httpClient.SendAsync(request, cts.Token);
        });
    }
}
