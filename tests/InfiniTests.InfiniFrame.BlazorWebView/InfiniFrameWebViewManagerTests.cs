// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.BlazorWebView;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Threading.Channels;

namespace InfiniTests.InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWebViewManagerTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task SendMessage_AfterDispose_ShouldReturnPromptly(CancellationToken ct = default) {
        // Arrange
        var window = Substitute.For<IInfiniFrameWindow>();
        var features = Substitute.For<IInfiniFrameWindowFeatures>();
        var webMessaging = Substitute.For<IWebMessagingInfiniFrameWindowFeature>();
        window.Features.Returns(features);
        features.WebMessaging.Returns(webMessaging);
        webMessaging.SendWebMessageAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.CompletedTask);

        await using ServiceProvider provider = new ServiceCollection()
            .AddLogging()
            .AddSingleton(window)
            .BuildServiceProvider();

        var dispatcher = Substitute.For<Dispatcher>();
        var manager = new TestableInfiniFrameWebViewManager(
            InfiniFrameWindowBuilder.Create(),
            provider,
            dispatcher,
            new NullFileProvider(),
            new JSComponentConfigurationStore(),
            Options.Create(new InfiniFrameBlazorAppConfiguration()));

        await manager.DisposeAsync();

        // Act
        Task sendTask = Task.Run(action: () => manager.SendMessageForTest("late-dispose-message"), ct);

        // Assert
        await sendTask.WaitAsync(TimeSpan.FromSeconds(1), ct);
        await webMessaging.DidNotReceive().SendWebMessageAsync("late-dispose-message", Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task SendMessage_ShouldSerializeOutgoingMessages(CancellationToken ct = default) {
        // Arrange
        var firstStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var secondStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var firstRelease = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        int invocation = 0;
        var window = Substitute.For<IInfiniFrameWindow>();
        var features = Substitute.For<IInfiniFrameWindowFeatures>();
        var webMessaging = Substitute.For<IWebMessagingInfiniFrameWindowFeature>();
        window.Features.Returns(features);
        features.WebMessaging.Returns(webMessaging);
        webMessaging.SendWebMessageAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(_ => {
                int current = Interlocked.Increment(ref invocation);
                if (current == 1) {
                    firstStarted.TrySetResult(true);
                    return new ValueTask(firstRelease.Task);
                }

                secondStarted.TrySetResult(true);
                return ValueTask.CompletedTask;
            });

        await using ServiceProvider provider = new ServiceCollection()
            .AddLogging()
            .AddSingleton(window)
            .BuildServiceProvider();

        var manager = new TestableInfiniFrameWebViewManager(
            InfiniFrameWindowBuilder.Create(),
            provider,
            Substitute.For<Dispatcher>(),
            new NullFileProvider(),
            new JSComponentConfigurationStore(),
            Options.Create(new InfiniFrameBlazorAppConfiguration())
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
        var window = Substitute.For<IInfiniFrameWindow>();
        var features = Substitute.For<IInfiniFrameWindowFeatures>();
        var webMessaging = Substitute.For<IWebMessagingInfiniFrameWindowFeature>();
        window.Features.Returns(features);
        features.WebMessaging.Returns(webMessaging);
        webMessaging.SendWebMessageAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(call => {
                string message = call.ArgAt<string>(0);
                lock (sentMessages) {
                    sentMessages.Add(message);
                }

                if (message == "second") secondDelivered.TrySetResult(true);
                if (message != "first") return ValueTask.CompletedTask;

                firstStarted.TrySetResult(true);
                return new ValueTask(firstRelease.Task);
            });

        await using ServiceProvider provider = new ServiceCollection()
            .AddLogging()
            .AddSingleton(window)
            .BuildServiceProvider();
        TestableInfiniFrameWebViewManager manager = CreateManager(provider, new InfiniFrameBlazorAppConfiguration {
            WebMessageQueueCapacity = 1,
            WebMessageQueueFullMode = BoundedChannelFullMode.Wait
        });

        // Act: the first message is in flight, the second occupies the only queue slot, and the third is rejected.
        manager.SendMessageForTest("first");
        await firstStarted.Task.WaitAsync(TimeSpan.FromSeconds(1), ct);
        manager.SendMessageForTest("second");
        manager.SendMessageForTest("dropped");
        firstRelease.TrySetResult(true);

        await secondDelivered.Task.WaitAsync(TimeSpan.FromSeconds(1), ct);
        await manager.DisposeAsync();

        // Assert
        await Assert.That(sentMessages).IsEquivalentTo(["first", "second"]);
    }

    [Test]
    public async Task DisposeAsync_ShouldCancelAndAwaitActiveMessagePumpWork(CancellationToken ct = default) {
        // Arrange
        var sendStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var sendStopped = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var window = Substitute.For<IInfiniFrameWindow>();
        var features = Substitute.For<IInfiniFrameWindowFeatures>();
        var webMessaging = Substitute.For<IWebMessagingInfiniFrameWindowFeature>();
        window.Features.Returns(features);
        features.WebMessaging.Returns(webMessaging);
        webMessaging.SendWebMessageAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(call => new ValueTask(WaitForCancellationAsync(
                call.ArgAt<CancellationToken>(1),
                sendStarted,
                sendStopped)));

        await using ServiceProvider provider = new ServiceCollection()
            .AddLogging()
            .AddSingleton(window)
            .BuildServiceProvider();
        TestableInfiniFrameWebViewManager manager = CreateManager(provider);
        manager.SendMessageForTest("pending");
        await sendStarted.Task.WaitAsync(TimeSpan.FromSeconds(1), ct);

        // Act
        await manager.DisposeAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(1), ct);

        // Assert: DisposeAsync does not return before the cancelled native send has exited.
        await Assert.That(sendStopped.Task.IsCompleted).IsTrue();
        manager.SendMessageForTest("after-dispose");
        await webMessaging.Received(1).SendWebMessageAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task SendMessage_ConcurrentWithDispose_ShouldNotSendAfterDispose(CancellationToken ct = default) {
        // Arrange
        var window = Substitute.For<IInfiniFrameWindow>();
        var features = Substitute.For<IInfiniFrameWindowFeatures>();
        var webMessaging = Substitute.For<IWebMessagingInfiniFrameWindowFeature>();
        window.Features.Returns(features);
        features.WebMessaging.Returns(webMessaging);
        webMessaging.SendWebMessageAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.CompletedTask);

        await using ServiceProvider provider = new ServiceCollection()
            .AddLogging()
            .AddSingleton(window)
            .BuildServiceProvider();
        TestableInfiniFrameWebViewManager manager = CreateManager(provider, new InfiniFrameBlazorAppConfiguration { WebMessageQueueCapacity = 8 });

        // Act
        Task[] producers = Enumerable.Range(0, 8)
            .Select(producer => Task.Run(action: () => {
                for (int message = 0; message < 250; message++) {
                    manager.SendMessageForTest($"{producer}-{message}");
                }
            }, ct))
            .ToArray();
        Task disposeTask = manager.DisposeAsync().AsTask();
        await Task.WhenAll(producers);
        await disposeTask.WaitAsync(TimeSpan.FromSeconds(2), ct);
        int sendsAtDispose = webMessaging.ReceivedCalls()
            .Count(call => call.GetMethodInfo().Name == nameof(IWebMessagingInfiniFrameWindowFeature.SendWebMessageAsync));

        manager.SendMessageForTest("late-message");

        // Assert
        int sendsAfterDispose = webMessaging.ReceivedCalls()
            .Count(call => call.GetMethodInfo().Name == nameof(IWebMessagingInfiniFrameWindowFeature.SendWebMessageAsync));
        await Assert.That(sendsAfterDispose).IsEqualTo(sendsAtDispose);
    }

    private static TestableInfiniFrameWebViewManager CreateManager(
        IServiceProvider provider,
        InfiniFrameBlazorAppConfiguration? configuration = null
    ) => new(
        InfiniFrameWindowBuilder.Create(),
        provider,
        Substitute.For<Dispatcher>(),
        new NullFileProvider(),
        new JSComponentConfigurationStore(),
        Options.Create(configuration ?? new InfiniFrameBlazorAppConfiguration()));

    private static async Task WaitForCancellationAsync(
        CancellationToken cancellationToken,
        TaskCompletionSource<bool> started,
        TaskCompletionSource<bool> stopped
    ) {
        started.TrySetResult(true);
        try {
            await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
        }
        finally {
            stopped.TrySetResult(true);
        }
    }

    private sealed class TestableInfiniFrameWebViewManager(
        IInfiniFrameWindowBuilder builder,
        IServiceProvider provider,
        Dispatcher dispatcher,
        IFileProvider fileProvider,
        JSComponentConfigurationStore jsComponents,
        IOptions<InfiniFrameBlazorAppConfiguration> config
    ) : InfiniFrameWebViewManager(builder, provider, dispatcher, fileProvider, jsComponents, config) {
        public void SendMessageForTest(string message) => SendMessage(message);
    }
}
