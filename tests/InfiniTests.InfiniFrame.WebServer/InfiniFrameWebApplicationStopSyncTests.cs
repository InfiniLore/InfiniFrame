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
public class InfiniFrameWebApplicationStopSyncTests {
    [Test]
    public async Task Stop_ShouldStopWebAppAndCloseWindow() {
        // Arrange
        IInfiniFrameWindow mockWindow = Substitute.For<IInfiniFrameWindow>();
        ILifecycleInfiniFrameWindowFeature lifecycle = mockWindow.Features.Lifecycle;
        WebApplication webApp = WebApplication.CreateBuilder().Build();
        var appLifetime = webApp.Services.GetRequiredService<IHostApplicationLifetime>();
        var app = new InfiniFrameWebApplication {
            Logger = NullLogger<InfiniFrameWebApplication>.Instance,
            WebApp = webApp,
            LazyWindow = new Lazy<IInfiniFrameWindow>(() => mockWindow)
        };

        // Act
        app.Stop();

        // Assert
        await lifecycle.Received(1).CloseAsync(Arg.Any<CancellationToken>());
        await lifecycle.Received(1).WaitForCloseAsync(Arg.Any<CancellationToken>());
        await Assert.That(appLifetime.ApplicationStopping.IsCancellationRequested).IsTrue();

        await webApp.DisposeAsync();
    }
}
