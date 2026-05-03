// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.BlazorWebView;
using InfiniFrameTests.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace InfiniFrameTests.BlazorWebView;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWebViewManagerTests {
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

    [Test]
    [Timeout(TimeoutUtility.DefaultTimeout)]
    public async Task SendMessage_AfterDispose_ShouldReturnPromptly(CancellationToken ct) {
        // Arrange
        IInfiniFrameWindow window = Substitute.For<IInfiniFrameWindow>();
        window.SendWebMessageAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

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
        Task sendTask = Task.Run(() => manager.SendMessageForTest("late-dispose-message"), ct);

        // Assert
        await sendTask.WaitAsync(TimeSpan.FromSeconds(1), ct);
        await window.DidNotReceive().SendWebMessageAsync("late-dispose-message", Arg.Any<CancellationToken>());
    }
}
