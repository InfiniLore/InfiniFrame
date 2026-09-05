// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Threading.Channels;
using InfiniFrame;
using InfiniFrame.BlazorWebView;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace InfiniTests.InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWebViewManagerTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task HandleWebRequest_FragmentAndQueryAreExcludedFromLookup(CancellationToken ct = default) {
        byte[] expected = [.. "settings-page"u8];
        var fileProvider = new RecordingFileProvider("index.html", expected);
        var builder = new InfiniFrameWindowBuilder();
        await using ServiceProvider provider = new ServiceCollection().AddLogging().BuildServiceProvider();
        await using var manager = new TestableInfiniFrameWebViewManager(
            builder,
            provider,
            MockFactory.CreateDispatcherMock().Object,
            fileProvider,
            new JSComponentConfigurationStore(),
            Options.Create(new InfiniFrameBlazorAppConfiguration()),
            NullLogger<InfiniFrameWebViewManager>.Instance
        );

        (Stream? data, string? contentType) = manager.HandleWebRequest(
            null, "app://localhost/index.html?version=7#settings");
        await using (data) {
            using var copy = new MemoryStream();
            await data!.CopyToAsync(copy, ct);
            await Assert.That(copy.ToArray()).IsEquivalentTo(expected);
        }

        await Assert.That(fileProvider.LastSubpath).IsEqualTo("index.html");
        await Assert.That(contentType).IsEqualTo("text/html");
    }

    [Test]
    [Arguments("not a URL")]
    [Arguments("app://other/index.html")]
    [Arguments("app://localhost:4242/index.html")]
    public async Task HandleWebRequest_MalformedOrUntrustedUrlIsRejected(string url, CancellationToken ct = default) {
        var fileProvider = new RecordingFileProvider("index.html", [.. "blocked"u8]);
        await using ServiceProvider provider = new ServiceCollection().AddLogging().BuildServiceProvider();
        await using var manager = new TestableInfiniFrameWebViewManager(
            new InfiniFrameWindowBuilder(),
            provider,
            MockFactory.CreateDispatcherMock().Object,
            fileProvider,
            new JSComponentConfigurationStore(),
            Options.Create(new InfiniFrameBlazorAppConfiguration()),
            NullLogger<InfiniFrameWebViewManager>.Instance
        );

        (Stream? data, string? contentType) = manager.HandleWebRequest(null, url);

        await Assert.That(data).IsNull();
        await Assert.That(contentType).IsNull();
        await Assert.That(fileProvider.LastSubpath).IsNull();
    }

    [Test]
    public async Task SendMessage_AfterDispose_ShouldReturnPromptly(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> windowMock = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> featuresMock = MockFactory.CreateFeaturesMock();
        Mock<IWebMessagingInfiniFrameWindowFeature> webMessagingMock = MockFactory.CreateWebMessagingMock();
        windowMock.Features.Returns(featuresMock.Object);
        featuresMock.WebMessaging.Returns(webMessagingMock.Object);
        webMessagingMock.SendWebMessageAsync(Any<string>(), Any<CancellationToken>())
            .Returns(() => ValueTask.CompletedTask);

        await using ServiceProvider provider = new ServiceCollection()
            .AddLogging()
            .AddSingleton(windowMock.Object)
            .BuildServiceProvider();

        Dispatcher dispatcher = MockFactory.CreateDispatcherMock().Object;
        var manager = new TestableInfiniFrameWebViewManager(
            new InfiniFrameWindowBuilder(),
            provider,
            dispatcher,
            new NullFileProvider(),
            new JSComponentConfigurationStore(),
            Options.Create(new InfiniFrameBlazorAppConfiguration()),
            NullLogger<InfiniFrameWebViewManager>.Instance);

        await manager.DisposeAsync();

        // Act
        Task sendTask = Task.Run(action: () => manager.SendMessageForTest("late-dispose-message"), ct);

        // Assert
        await sendTask.WaitAsync(TimeSpan.FromSeconds(1), ct);
        webMessagingMock.SendWebMessageAsync("late-dispose-message", Any<CancellationToken>()).WasNeverCalled();
    }

    [Test]
    public async Task SendMessage_ShouldSerializeOutgoingMessages(CancellationToken ct = default) {
        // Arrange
        var firstStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var secondStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var firstRelease = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        int invocation = 0;
        Mock<IInfiniFrameWindow> windowMock = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> featuresMock = MockFactory.CreateFeaturesMock();
        Mock<IWebMessagingInfiniFrameWindowFeature> webMessagingMock = MockFactory.CreateWebMessagingMock();
        windowMock.Features.Returns(featuresMock.Object);
        featuresMock.WebMessaging.Returns(webMessagingMock.Object);
        ValueTask returnValue = default;
        webMessagingMock.SendWebMessageAsync(Any<string>(), Any<CancellationToken>())
            .Callback(() => {
                int current = Interlocked.Increment(ref invocation);
                if (current == 1) {
                    firstStarted.TrySetResult(true);
                    returnValue = new ValueTask(firstRelease.Task);
                }
                else {
                    secondStarted.TrySetResult(true);
                    returnValue = ValueTask.CompletedTask;
                }
            }).Returns(() => returnValue);

        await using ServiceProvider provider = new ServiceCollection()
            .AddLogging()
            .AddSingleton(windowMock.Object)
            .BuildServiceProvider();

        var manager = new TestableInfiniFrameWebViewManager(
            new InfiniFrameWindowBuilder(),
            provider,
            MockFactory.CreateDispatcherMock().Object,
            new NullFileProvider(),
            new JSComponentConfigurationStore(),
            Options.Create(new InfiniFrameBlazorAppConfiguration()),
            NullLogger<InfiniFrameWebViewManager>.Instance
        );

        // Act
        manager.SendMessageForTest("batch-1");
        manager.SendMessageForTest("batch-2");

        await firstStarted.Task.WaitAsync(TimeSpan.FromSeconds(1), ct);

        // Assert
        await Assert.That(secondStarted.Task.IsCompleted).IsFalse();

        firstRelease.TrySetResult(true);
        await secondStarted.Task.WaitAsync(TimeSpan.FromSeconds(1), ct);

        await manager.DisposeAsync();
    }

    [Test]
    public async Task SendMessage_WhenBoundedQueueIsFull_ShouldApplyConfiguredBackpressure(CancellationToken ct = default) {
        // Arrange
        var firstStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var firstRelease = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var secondDelivered = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var sentMessages = new List<string>();
        Mock<IInfiniFrameWindow> windowMock = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> featuresMock = MockFactory.CreateFeaturesMock();
        Mock<IWebMessagingInfiniFrameWindowFeature> webMessagingMock = MockFactory.CreateWebMessagingMock();
        windowMock.Features.Returns(featuresMock.Object);
        featuresMock.WebMessaging.Returns(webMessagingMock.Object);
        ValueTask backpressureReturnValue = default;
        webMessagingMock.SendWebMessageAsync(Any<string>(), Any<CancellationToken>())
            .Callback((message, _) => {
                sentMessages.Add(message);
                if (message == "first") firstStarted.TrySetResult(true);
                if (message == "second") secondDelivered.TrySetResult(true);
            }).Returns(() => backpressureReturnValue);

        await using ServiceProvider provider = new ServiceCollection()
            .AddLogging()
            .AddSingleton(windowMock.Object)
            .BuildServiceProvider();
        TestableInfiniFrameWebViewManager manager = CreateManager(provider, new InfiniFrameBlazorAppConfiguration {
            WebMessageQueueCapacity = 1,
            WebMessageQueueFullMode = BoundedChannelFullMode.Wait
        });

        // Act
        manager.SendMessageForTest("first");
        await firstStarted.Task.WaitAsync(TimeSpan.FromSeconds(5), ct);
        manager.SendMessageForTest("second");
        manager.SendMessageForTest("dropped");
        firstRelease.TrySetResult(true);

        await secondDelivered.Task.WaitAsync(TimeSpan.FromSeconds(5), ct);
        await manager.DisposeAsync();

        // Assert
        await Assert.That(sentMessages).IsEquivalentTo(["first", "second"]);
    }

    [Test]
    public async Task DisposeAsync_ShouldCancelAndAwaitActiveMessagePumpWork(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> windowMock = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> featuresMock = MockFactory.CreateFeaturesMock();
        Mock<IWebMessagingInfiniFrameWindowFeature> webMessagingMock = MockFactory.CreateWebMessagingMock();
        windowMock.Features.Returns(featuresMock.Object);
        featuresMock.WebMessaging.Returns(webMessagingMock.Object);
        webMessagingMock.SendWebMessageAsync(Any<string>(), Any<CancellationToken>())
            .Returns(() => new ValueTask());

        await using ServiceProvider provider = new ServiceCollection()
            .AddLogging()
            .AddSingleton(windowMock.Object)
            .BuildServiceProvider();
        TestableInfiniFrameWebViewManager manager = CreateManager(provider);
        manager.SendMessageForTest("pending");
        await Task.Delay(1000, ct);

        // Act
        await manager.DisposeAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(5), ct);

        // Assert
        manager.SendMessageForTest("after-dispose");
        webMessagingMock.SendWebMessageAsync(Any<string>(), Any<CancellationToken>()).WasCalled(Times.Once);
    }

    [Test]
    public async Task SendMessage_ConcurrentWithDispose_ShouldNotSendAfterDispose(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> windowMock = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> featuresMock = MockFactory.CreateFeaturesMock();
        Mock<IWebMessagingInfiniFrameWindowFeature> webMessagingMock = MockFactory.CreateWebMessagingMock();
        windowMock.Features.Returns(featuresMock.Object);
        featuresMock.WebMessaging.Returns(webMessagingMock.Object);
        int sendCount = 0;
        webMessagingMock.SendWebMessageAsync(Any<string>(), Any<CancellationToken>())
            .Callback(() => {Interlocked.Increment(ref sendCount);})
            .Returns(() => ValueTask.CompletedTask);

        await using ServiceProvider provider = new ServiceCollection()
            .AddLogging()
            .AddSingleton(windowMock.Object)
            .BuildServiceProvider();
        TestableInfiniFrameWebViewManager manager = CreateManager(provider, new InfiniFrameBlazorAppConfiguration { WebMessageQueueCapacity = 8 });

        // Act
        Task[] producers = [
            .. Enumerable.Range(0, 8)
                .Select(producer => Task.Run(action: () => {
                    for (int message = 0; message < 250; message++) {
                        manager.SendMessageForTest($"{producer}-{message}");
                    }
                }, ct))
        ];
        Task disposeTask = manager.DisposeAsync().AsTask();
        await Task.WhenAll(producers);
        await disposeTask.WaitAsync(TimeSpan.FromSeconds(2), ct);
        int sendsAtDispose = sendCount;

        manager.SendMessageForTest("late-message");

        // Assert
        int sendsAfterDispose = sendCount;
        await Assert.That(sendsAfterDispose).IsEqualTo(sendsAtDispose);
    }

    private static TestableInfiniFrameWebViewManager CreateManager(
        IServiceProvider provider,
        InfiniFrameBlazorAppConfiguration? configuration = null
    ) => new(
        new InfiniFrameWindowBuilder(),
        provider,
        MockFactory.CreateDispatcherMock().Object,
        new NullFileProvider(),
        new JSComponentConfigurationStore(),
        Options.Create(configuration ?? new InfiniFrameBlazorAppConfiguration()),
        NullLogger<InfiniFrameWebViewManager>.Instance);

    private sealed class TestableInfiniFrameWebViewManager(
        IInfiniFrameWindowBuilder builder,
        IServiceProvider provider,
        Dispatcher dispatcher,
        IFileProvider fileProvider,
        JSComponentConfigurationStore jsComponents,
        IOptions<InfiniFrameBlazorAppConfiguration> config,
        ILogger<InfiniFrameWebViewManager> logger
    ) : InfiniFrameWebViewManager(builder, provider, dispatcher, fileProvider, jsComponents, config, logger) {
        public void SendMessageForTest(string message) => SendMessage(message);
    }

    private sealed class RecordingFileProvider(string expectedPath, byte[] content) : IFileProvider {
        public string? LastSubpath { get; private set; }

        public IFileInfo GetFileInfo(string subpath) {
            LastSubpath = subpath;
            return string.Equals(subpath, expectedPath, StringComparison.Ordinal)
                ? new MemoryFileInfo(expectedPath, content)
                : new NotFoundFileInfo(subpath);
        }

        public IDirectoryContents GetDirectoryContents(string subpath) => NotFoundDirectoryContents.Singleton;
        public IChangeToken Watch(string filter) => NullChangeToken.Singleton;
    }

    private sealed class MemoryFileInfo(string name, byte[] content) : IFileInfo {
        public bool Exists => true;
        public long Length => content.Length;
        public string? PhysicalPath => null;
        public string Name => name;
        public DateTimeOffset LastModified => DateTimeOffset.UnixEpoch;
        public bool IsDirectory => false;
        public Stream CreateReadStream() => new MemoryStream(content, false);
    }
}
