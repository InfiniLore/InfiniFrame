// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.WebServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;

namespace InfiniTests.InfiniFrame.WebServer;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWebApplicationStopAsyncTests {
    [Test]
    public async Task StopAsync_ShouldStopWebAppAndCloseWindow(CancellationToken ct) {
        // Arrange
        Mock<IInfiniFrameWindow> mockWindow = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<ILifecycleInfiniFrameWindowFeature> lifecycle = MockFactory.CreateLifecycleMock();
        mockWindow.Features.Returns(features.Object);
        features.Lifecycle.Returns(lifecycle.Object);
        WebApplication webApp = WebApplication.CreateBuilder().Build();
        var appLifetime = webApp.Services.GetRequiredService<IHostApplicationLifetime>();
        var app = new InfiniFrameWebApplication {
            Logger = NullLogger<InfiniFrameWebApplication>.Instance,
            WebApp = webApp,
            LazyWindow = new Lazy<IInfiniFrameWindow>(() => mockWindow.Object)
        };

        // Act
        await app.StopAsync(ct);

        // Assert
        lifecycle.CloseAsync(ct).WasCalled(Times.Once);
        lifecycle.WaitForCloseAsync(ct).WasCalled(Times.Once);
        await Assert.That(appLifetime.ApplicationStopping.IsCancellationRequested).IsTrue();

        await webApp.DisposeAsync();
    }
}
