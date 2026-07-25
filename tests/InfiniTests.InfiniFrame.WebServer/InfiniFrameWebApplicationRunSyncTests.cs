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
public class InfiniFrameWebApplicationRunSyncTests {
    [Test]
    public async Task Run_ShouldStartWebAppBeforeWaitingThenStopAndDisposeIt() {
        // Arrange
        IInfiniFrameWindow mockWindow = Substitute.For<IInfiniFrameWindow>();
        ILifecycleInfiniFrameWindowFeature lifecycle = mockWindow.Features.Lifecycle;

        WebApplicationBuilder webAppBuilder = WebApplication.CreateBuilder();
        webAppBuilder.Services.Replace(ServiceDescriptor.Singleton<IServer, NoopServer>());
        webAppBuilder.Services.AddSingleton<DisposeProbe>();
        WebApplication webApp = webAppBuilder.Build();
        var server = (NoopServer)webApp.Services.GetRequiredService<IServer>();
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
        await Assert.That(server.StartCount).IsEqualTo(1);
        await Assert.That(server.StopCount).IsEqualTo(1);
        await Assert.That(server.DisposeCount).IsEqualTo(1);
    }

    [Test]
    public async Task Run_WhenWaitFails_StillStopsAndDisposesWebApp() {
        IInfiniFrameWindow mockWindow = Substitute.For<IInfiniFrameWindow>();
        mockWindow.Features.Lifecycle.When(static feature => feature.WaitForClose())
            .Do(_ => throw new InvalidOperationException("wait failed"));
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Services.Replace(ServiceDescriptor.Singleton<IServer, NoopServer>());
        WebApplication webApp = builder.Build();
        var server = (NoopServer)webApp.Services.GetRequiredService<IServer>();
        var app = new InfiniFrameWebApplication {
            Logger = NullLogger<InfiniFrameWebApplication>.Instance,
            WebApp = webApp,
            LazyWindow = new Lazy<IInfiniFrameWindow>(() => mockWindow)
        };

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => app.Run());

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
