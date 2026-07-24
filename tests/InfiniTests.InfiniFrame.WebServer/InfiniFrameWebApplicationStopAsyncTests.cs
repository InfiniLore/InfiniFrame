// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.WebServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace InfiniTests.InfiniFrame.WebServer;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWebApplicationStopAsyncTests {
    [Test]
    public async Task StopAsync_ShouldStopWebAppAndCloseWindow(CancellationToken ct) {
        // Arrange
        IInfiniFrameWindow mockWindow = Substitute.For<IInfiniFrameWindow>();
        IInfiniFrameWindowFeatureLifecycle lifecycle = mockWindow.Features.Lifecycle;
        WebApplication webApp = WebApplication.CreateBuilder().Build();
        var appLifetime = webApp.Services.GetRequiredService<IHostApplicationLifetime>();
        var app = new InfiniFrameWebApplication {
            Logger = NullLogger<InfiniFrameWebApplication>.Instance,
            WebApp = webApp,
            LazyWindow = new Lazy<IInfiniFrameWindow>(() => mockWindow)
        };

        // Act
        await app.StopAsync(ct);

        // Assert
        await lifecycle.Received(1).CloseAsync(ct);
        await lifecycle.Received(1).WaitForCloseAsync(ct);
        await Assert.That(appLifetime.ApplicationStopping.IsCancellationRequested).IsTrue();

        await webApp.DisposeAsync();
    }
}
