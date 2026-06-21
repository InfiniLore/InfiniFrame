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

namespace InfiniTests.InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[NotInParallelInfiniTests]
public class InfiniFrameWebViewManagerTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task SendMessage_AfterDispose_ShouldReturnPromptly(CancellationToken ct = default) {
        // Arrange
        var window = Substitute.For<IInfiniFrameWindow>();
        var features = Substitute.For<IInfiniFrameWindowFeatures>();
        var webMessaging = Substitute.For<IInfiniFrameWindowFeatureWebMessaging>();
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
        var webMessaging = Substitute.For<IInfiniFrameWindowFeatureWebMessaging>();
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
