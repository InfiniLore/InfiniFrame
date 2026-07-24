// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.WebServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace InfiniTests.InfiniFrame.WebServer;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWebApplicationRunSyncTests {
    [Test]
    public async Task Run_ShouldStartWebAppBeforeWaitingThenStopAndDisposeIt() {
        // Arrange
        IInfiniFrameWindow mockWindow = Substitute.For<IInfiniFrameWindow>();
        ILifecycleInfiniFrameWindowFeature lifecycle = mockWindow.Features.Lifecycle;

        WebApplicationBuilder webAppBuilder = WebApplication.CreateBuilder();
        webAppBuilder.WebHost.UseUrls("http://127.0.0.1:0");
        webAppBuilder.Services.AddSingleton<DisposeProbe>();
        WebApplication webApp = webAppBuilder.Build();
        var appLifetime = webApp.Services.GetRequiredService<IHostApplicationLifetime>();
        var disposeProbe = webApp.Services.GetRequiredService<DisposeProbe>();
        bool webAppStartedBeforeWait = false;
        lifecycle.When(static feature => feature.WaitForClose())
            .Do(_ => webAppStartedBeforeWait = appLifetime.ApplicationStarted.IsCancellationRequested);

        var app = new InfiniFrameWebApplication {
            Logger = NullLogger<InfiniFrameWebApplication>.Instance,
            WebApp = webApp,
            LazyWindow = new Lazy<IInfiniFrameWindow>(() => mockWindow)
        };

        // Act
        app.Run();

        // Assert
        lifecycle.Received(1).WaitForClose();
        await lifecycle.DidNotReceive().WaitForCloseAsync(Arg.Any<CancellationToken>());
        await Assert.That(webAppStartedBeforeWait).IsTrue();
        await Assert.That(appLifetime.ApplicationStopping.IsCancellationRequested).IsTrue();
        await Assert.That(disposeProbe.IsDisposed).IsTrue();
    }

    private sealed class DisposeProbe : IDisposable {
        public bool IsDisposed { get; private set; }

        public void Dispose() {
            IsDisposed = true;
        }
    }
}
