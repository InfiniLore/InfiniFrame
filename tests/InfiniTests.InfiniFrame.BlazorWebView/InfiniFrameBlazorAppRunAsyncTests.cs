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
public class InfiniFrameBlazorAppRunAsyncTests {
    [Test]
    public async Task RunAsync_ShouldWaitAsynchronouslyAndDisposeServices(CancellationToken ct) {
        // Arrange
        var window = Substitute.For<IInfiniFrameWindow>();
        ILifecycleInfiniFrameWindowFeature lifecycle = window.Features.Lifecycle;
        lifecycle.WaitForCloseAsync(ct).Returns(ValueTask.CompletedTask);
        ServiceProvider services = new ServiceCollection()
            .AddSingleton(window)
            .AddSingleton<DisposeProbe>()
            .BuildServiceProvider();
        var disposeProbe = services.GetRequiredService<DisposeProbe>();
        var app = new InfiniFrameBlazorApp(services, new InfiniFrameRootComponentList());

        // Act
        await app.RunAsync(ct);

        // Assert
        await lifecycle.Received(1).WaitForCloseAsync(ct);
        lifecycle.DidNotReceive().WaitForClose();
        await Assert.That(disposeProbe.IsDisposed).IsTrue();
    }

    private sealed class DisposeProbe : IDisposable {
        public bool IsDisposed { get; private set; }

        public void Dispose() {
            IsDisposed = true;
        }
    }
}
