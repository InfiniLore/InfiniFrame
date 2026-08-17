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
public class InfiniFrameBlazorAppRunSyncTests {
    [Test]
    public async Task Run_ShouldWaitSynchronouslyAndDisposeServices() {
        // Arrange
        Mock<IInfiniFrameWindow> windowMock = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> featuresMock = MockFactory.CreateFeaturesMock();
        Mock<ILifecycleInfiniFrameWindowFeature> lifecycleMock = MockFactory.CreateLifecycleMock();
        windowMock.Features.Returns(featuresMock.Object);
        featuresMock.Lifecycle.Returns(lifecycleMock.Object);
        ServiceProvider services = new ServiceCollection()
            .AddSingleton(windowMock.Object)
            .AddSingleton<DisposeProbe>()
            .BuildServiceProvider();
        var disposeProbe = services.GetRequiredService<DisposeProbe>();
        var app = new InfiniFrameBlazorApp(services, new InfiniFrameRootComponentList());

        // Act
        app.Run();

        // Assert
        lifecycleMock.WaitForClose().WasCalled(Times.Once);
        lifecycleMock.WaitForCloseAsync(Any<CancellationToken>()).WasNeverCalled();
        await Assert.That(disposeProbe.IsDisposed).IsTrue();
    }

    private sealed class DisposeProbe : IDisposable {
        public bool IsDisposed { get; private set; }

        public void Dispose() {
            IsDisposed = true;
        }
    }
}
