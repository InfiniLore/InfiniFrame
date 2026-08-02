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
        var mockWindow = Substitute.For<IInfiniFrameWindow>();
        ILifecycleInfiniFrameWindowFeature lifecycle = mockWindow.Features.Lifecycle;

        WebApplicationBuilder webAppBuilder = WebApplication.CreateBuilder();
        webAppBuilder.Services.Replace(ServiceDescriptor.Singleton<IServer, NoopServer>());
        webAppBuilder.Services.AddSingleton<DisposeProbe>();
        WebApplication webApp = webAppBuilder.Build();
        var server = (NoopServer)webApp.Services.GetRequiredService<IServer>();
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
        await Assert.That(server.StartCount).IsEqualTo(1);
        await Assert.That(server.StopCount).IsEqualTo(1);
        await Assert.That(server.DisposeCount).IsEqualTo(1);
    }

    [Test]
    public async Task RunAsync_WhenWaitFails_StillStopsAndDisposesWebApp() {
        var mockWindow = Substitute.For<IInfiniFrameWindow>();
        mockWindow.Features.Lifecycle.WaitForCloseAsync(Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromException(new InvalidOperationException("wait failed")));
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Services.Replace(ServiceDescriptor.Singleton<IServer, NoopServer>());
        WebApplication webApp = builder.Build();
        var server = (NoopServer)webApp.Services.GetRequiredService<IServer>();
        var app = new InfiniFrameWebApplication {
            Logger = NullLogger<InfiniFrameWebApplication>.Instance,
            WebApp = webApp,
            LazyWindow = new Lazy<IInfiniFrameWindow>(() => mockWindow)
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => app.RunAsync());

        await Assert.That(exception).IsNotNull();
        await Assert.That(exception!.Message).IsEqualTo("wait failed");
        await Assert.That(server.StartCount).IsEqualTo(1);
        await Assert.That(server.StopCount).IsEqualTo(1);
        await Assert.That(server.DisposeCount).IsEqualTo(1);
    }

    private sealed class DisposeProbe : IDisposable {
        public bool IsDisposed { get; private set; }

        public void Dispose() {
            IsDisposed = true;
        }
    }
}