// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.BlazorWebView;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniTests.InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameBlazorAppRunAsyncTests {
    [Test]
    public async Task RunAsync_ShouldWaitAsynchronouslyAndDisposeServices(CancellationToken ct) {
        // Arrange
        Mock<IInfiniFrameWindow> windowMock = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> featuresMock = MockFactory.CreateFeaturesMock();
        Mock<ILifecycleInfiniFrameWindowFeature> lifecycleMock = MockFactory.CreateLifecycleMock();
        windowMock.Features.Returns(featuresMock.Object);
        featuresMock.Lifecycle.Returns(lifecycleMock.Object);
        lifecycleMock.WaitForCloseAsync(ct).Returns(() => ValueTask.CompletedTask);
        ServiceProvider services = new ServiceCollection()
            .AddSingleton<IInfiniFrameWindow>(windowMock.Object)
            .AddSingleton<DisposeProbe>()
            .BuildServiceProvider();
        var disposeProbe = services.GetRequiredService<DisposeProbe>();
        var app = new InfiniFrameBlazorApp(services, new InfiniFrameRootComponentList());

        // Act
        await app.RunAsync(ct);

        // Assert
        lifecycleMock.WaitForCloseAsync(ct).WasCalled(Times.Once);
        lifecycleMock.WaitForClose().WasNeverCalled();
        await Assert.That(disposeProbe.IsDisposed).IsTrue();
    }

    private sealed class DisposeProbe : IDisposable {
        public bool IsDisposed { get; private set; }

        public void Dispose() {
            IsDisposed = true;
        }
    }
}
