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

namespace InfiniTests.InfiniFrame.WebServer;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWebApplicationRunSyncTests {
    [Test]
    public async Task Run_ShouldStartWebAppBeforeWaitingThenStopAndDisposeIt() {
        // Arrange
        var mockWindow = MockFactory.CreateWindowMock();
        var features = MockFactory.CreateFeaturesMock();
        var lifecycle = MockFactory.CreateLifecycleMock();
        mockWindow.Features.Returns(features.Object);
        features.Lifecycle.Returns(lifecycle.Object);

        WebApplicationBuilder webAppBuilder = WebApplication.CreateBuilder();
        webAppBuilder.Services.Replace(ServiceDescriptor.Singleton<IServer, NoopServer>());
        webAppBuilder.Services.AddSingleton<DisposeProbe>();
        WebApplication webApp = webAppBuilder.Build();
        var server = (NoopServer)webApp.Services.GetRequiredService<IServer>();
        var appLifetime = webApp.Services.GetRequiredService<IHostApplicationLifetime>();
        var disposeProbe = webApp.Services.GetRequiredService<DisposeProbe>();
        bool webAppStartedBeforeWait = false;
        lifecycle.WaitForClose().Callback(() => webAppStartedBeforeWait = appLifetime.ApplicationStarted.IsCancellationRequested);

        var app = new InfiniFrameWebApplication {
            Logger = NullLogger<InfiniFrameWebApplication>.Instance,
            WebApp = webApp,
            LazyWindow = new Lazy<IInfiniFrameWindow>(() => mockWindow.Object)
        };

        // Act
        app.Run();

        // Assert
        lifecycle.WaitForClose().WasCalled(Times.Once);
        lifecycle.WaitForCloseAsync(Any<CancellationToken>()).WasNeverCalled();
        await Assert.That(webAppStartedBeforeWait).IsTrue();
        await Assert.That(appLifetime.ApplicationStopping.IsCancellationRequested).IsTrue();
        await Assert.That(disposeProbe.IsDisposed).IsTrue();
        await Assert.That(server.StartCount).IsEqualTo(1);
        await Assert.That(server.StopCount).IsEqualTo(1);
        await Assert.That(server.DisposeCount).IsEqualTo(1);
    }

    [Test]
    public async Task Run_WhenWaitFails_StillStopsAndDisposesWebApp() {
        var mockWindow = MockFactory.CreateWindowMock();
        var features = MockFactory.CreateFeaturesMock();
        var lifecycle = MockFactory.CreateLifecycleMock();
        mockWindow.Features.Returns(features.Object);
        features.Lifecycle.Returns(lifecycle.Object);
        lifecycle.WaitForClose().Callback(() => throw new InvalidOperationException("wait failed"));
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Services.Replace(ServiceDescriptor.Singleton<IServer, NoopServer>());
        WebApplication webApp = builder.Build();
        var server = (NoopServer)webApp.Services.GetRequiredService<IServer>();
        var app = new InfiniFrameWebApplication {
            Logger = NullLogger<InfiniFrameWebApplication>.Instance,
            WebApp = webApp,
            LazyWindow = new Lazy<IInfiniFrameWindow>(() => mockWindow.Object)
        };

        var exception = Assert.Throws<InvalidOperationException>(() => app.Run());

        await Assert.That(exception).IsNotNull();
        await Assert.That(exception.Message).IsEqualTo("wait failed");
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
