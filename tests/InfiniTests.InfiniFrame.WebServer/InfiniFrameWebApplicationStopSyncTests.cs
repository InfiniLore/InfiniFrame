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
public class InfiniFrameWebApplicationStopSyncTests {
    [Test]
    public async Task Stop_ShouldStopWebAppAndCloseWindow() {
        // Arrange
        var mockWindow = MockFactory.CreateWindowMock();
        var features = MockFactory.CreateFeaturesMock();
        var lifecycle = MockFactory.CreateLifecycleMock();
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
        app.Stop();

        // Assert
        lifecycle.CloseAsync(Any<CancellationToken>()).WasCalled(Times.Once);
        lifecycle.WaitForCloseAsync(Any<CancellationToken>()).WasCalled(Times.Once);
        await Assert.That(appLifetime.ApplicationStopping.IsCancellationRequested).IsTrue();

        await webApp.DisposeAsync();
    }
}
