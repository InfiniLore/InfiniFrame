// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.BlazorWebView;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace InfiniTests.InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameBlazorAppRunSyncTests {
    [Test]
    public async Task Run_ShouldWaitSynchronouslyAndDisposeServices() {
        // Arrange
        var window = Substitute.For<IInfiniFrameWindow>();
        ILifecycleInfiniFrameWindowFeature lifecycle = window.Features.Lifecycle;
        ServiceProvider services = new ServiceCollection()
            .AddSingleton(window)
            .AddSingleton<DisposeProbe>()
            .BuildServiceProvider();
        var disposeProbe = services.GetRequiredService<DisposeProbe>();
        var app = new InfiniFrameBlazorApp(services, new InfiniFrameRootComponentList());

        // Act
        app.Run();

        // Assert
        lifecycle.Received(1).WaitForClose();
        await lifecycle.DidNotReceive().WaitForCloseAsync(Arg.Any<CancellationToken>());
        await Assert.That(disposeProbe.IsDisposed).IsTrue();
    }

    private sealed class DisposeProbe : IDisposable {
        public bool IsDisposed { get; private set; }

        public void Dispose() {
            IsDisposed = true;
        }
    }
}