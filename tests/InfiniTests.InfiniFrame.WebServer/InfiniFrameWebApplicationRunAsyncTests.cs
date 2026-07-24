// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.WebServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace InfiniTests.InfiniFrame.WebServer;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWebApplicationRunAsyncTests {
    [Test]
    public async Task RunAsync_ShouldStartWebAppBeforeWaitingThenStopAndDisposeIt() {
        // Arrange
        IInfiniFrameWindow mockWindow = Substitute.For<IInfiniFrameWindow>();
        ILifecycleInfiniFrameWindowFeature lifecycle = mockWindow.Features.Lifecycle;

        WebApplicationBuilder webAppBuilder = WebApplication.CreateBuilder();
        webAppBuilder.Services.Replace(ServiceDescriptor.Singleton<IServer, NoopServer>());
        webAppBuilder.Services.AddSingleton<DisposeProbe>();
        WebApplication webApp = webAppBuilder.Build();
        var appLifetime = webApp.Services.GetRequiredService<IHostApplicationLifetime>();
        var disposeProbe = webApp.Services.GetRequiredService<DisposeProbe>();
        bool webAppStartedBeforeWait = false;
        lifecycle.WaitForCloseAsync(Arg.Any<CancellationToken>())
            .Returns(_ => {
                webAppStartedBeforeWait = appLifetime.ApplicationStarted.IsCancellationRequested;
                return ValueTask.CompletedTask;
            });

        var app = new InfiniFrameWebApplication {
            Logger = NullLogger<InfiniFrameWebApplication>.Instance,
            WebApp = webApp,
            LazyWindow = new Lazy<IInfiniFrameWindow>(() => mockWindow)
        };

        // Act
        await app.RunAsync();

        // Assert
        await lifecycle.Received(1).WaitForCloseAsync(Arg.Any<CancellationToken>());
        lifecycle.DidNotReceive().WaitForClose();
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
