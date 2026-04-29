// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.WebServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;

namespace InfiniFrameTests.WebServer;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWebApplicationTests {

    private static IInfiniFrameWindow CreateMockWindow() {
        var mockWindow = Substitute.For<IInfiniFrameWindow>();
        mockWindow.Events.Returns(new InfiniFrameWindowEvents());
        return mockWindow;
    }

    [Test]
    public async Task CreateBuilder_ShouldReturnValidBuilder() {
        // Arrange & Act
        InfiniFrameWebApplicationBuilder builder = InfiniFrameWebApplication.CreateBuilder();

        // Assert
        await Assert.That(builder).IsNotNull();
        await Assert.That(builder.WebApp).IsNotNull();
        await Assert.That(builder.Window).IsNotNull();
    }

    [Test]
    public async Task UseAutoServerClose_WhenWindowNotCreated_ShouldRegisterWithBuilder() {
        // Arrange
        var mockWindowBuilder = Substitute.For<IInfiniFrameWindowBuilder>();
        var mockBuilderEvents = new InfiniFrameWindowEvents();
        mockWindowBuilder.Events.Returns(mockBuilderEvents);

        WebApplicationBuilder webAppBuilder = WebApplication.CreateBuilder();
        webAppBuilder.Services.AddSingleton(mockWindowBuilder);

        WebApplication webApp = webAppBuilder.Build();
        var lazyWindow = new Lazy<IInfiniFrameWindow>(CreateMockWindow);

        var app = new InfiniFrameWebApplication {
            WebApp = webApp,
            LazyWindow = lazyWindow
        };

        // Act
        InfiniFrameWebApplication result = app.UseAutoServerClose();

        // Assert
        await Assert.That(result).IsEqualTo(app);
        await Assert.That(mockBuilderEvents.WindowClosing.Snapshot.Length).IsGreaterThan(0);
    }

    [Test]
    public async Task UseAutoServerClose_WhenWindowCreated_ShouldRegisterWithWindow() {
        // Arrange
        IInfiniFrameWindow mockWindow = CreateMockWindow();
        IInfiniFrameWindowEvents mockEvents = mockWindow.Events;

        WebApplicationBuilder webAppBuilder = WebApplication.CreateBuilder();
        WebApplication webApp = webAppBuilder.Build();

        var lazyWindow = new Lazy<IInfiniFrameWindow>(() => mockWindow);
        // Force window creation
        _ = lazyWindow.Value;

        var app = new InfiniFrameWebApplication {
            WebApp = webApp,
            LazyWindow = lazyWindow
        };

        // Act
        InfiniFrameWebApplication result = app.UseAutoServerClose();

        // Assert
        await Assert.That(result).IsEqualTo(app);
        await Assert.That(mockEvents.WindowClosing.Snapshot.Length).IsGreaterThan(0);
    }

    [Test]
    public async Task UseAutoServerClose_ClosingHandler_ShouldReturnFalse() {
        // Arrange
        IInfiniFrameWindow mockWindow = CreateMockWindow();
        IInfiniFrameWindowEvents mockEvents = mockWindow.Events;

        WebApplicationBuilder webAppBuilder = WebApplication.CreateBuilder();
        WebApplication webApp = webAppBuilder.Build();

        var lazyWindow = new Lazy<IInfiniFrameWindow>(() => mockWindow);
        _ = lazyWindow.Value;

        var app = new InfiniFrameWebApplication {
            WebApp = webApp,
            LazyWindow = lazyWindow
        };

        // Act
        app.UseAutoServerClose();
        NetClosingDelegate? capturedHandler = mockEvents.WindowClosing.Snapshot.LastOrDefault();
        bool? result = capturedHandler?.Invoke(new object(), EventArgs.Empty);

        // Assert
        await Assert.That(capturedHandler).IsNotNull();
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task UseAutoServerClose_ClosingHandler_ShouldInitiateStopAsync() {
        // Arrange
        IInfiniFrameWindow mockWindow = CreateMockWindow();
        IInfiniFrameWindowEvents mockEvents = mockWindow.Events;

        WebApplicationBuilder webAppBuilder = WebApplication.CreateBuilder();
        WebApplication webApp = webAppBuilder.Build();

        var lazyWindow = new Lazy<IInfiniFrameWindow>(() => mockWindow);
        _ = lazyWindow.Value;

        var app = new InfiniFrameWebApplication {
            WebApp = webApp,
            LazyWindow = lazyWindow
        };

        app.UseAutoServerClose();

        // Act
        NetClosingDelegate? capturedHandler = mockEvents.WindowClosing.Snapshot.LastOrDefault();
        capturedHandler?.Invoke(new object(), EventArgs.Empty);

        var appLifetime = webApp.Services.GetRequiredService<IHostApplicationLifetime>();
        DateTime deadline = DateTime.UtcNow.AddSeconds(2);
        while (!appLifetime.ApplicationStopping.IsCancellationRequested && DateTime.UtcNow < deadline) {
            await Task.Delay(50);
        }

        if (!appLifetime.ApplicationStopping.IsCancellationRequested) {
            Console.WriteLine("Timed out waiting for ApplicationStopping after closing handler invocation.");
        }

        // Assert
        // The web app should be in the process of stopping
        await Assert.That(appLifetime.ApplicationStopping.IsCancellationRequested)
            .IsTrue();
    }

    [Test]
    [Retry(5)]
    public async Task Stop_ShouldCloseWindowAndStopWebApp() {
        // Arrange
        IInfiniFrameWindow mockWindow = CreateMockWindow();

        WebApplicationBuilder webAppBuilder = WebApplication.CreateBuilder();
        WebApplication webApp = webAppBuilder.Build();

        var lazyWindow = new Lazy<IInfiniFrameWindow>(() => mockWindow);

        var app = new InfiniFrameWebApplication {
            WebApp = webApp,
            LazyWindow = lazyWindow
        };

        // Act
        app.Stop();

        var appLifetime = webApp.Services.GetRequiredService<IHostApplicationLifetime>();
        DateTime deadline = DateTime.UtcNow.AddSeconds(2);
        while (!appLifetime.ApplicationStopping.IsCancellationRequested && DateTime.UtcNow < deadline) {
            await Task.Delay(50);
        }

        if (!appLifetime.ApplicationStopping.IsCancellationRequested) {
            Console.WriteLine("Timed out waiting for ApplicationStopping after Stop() call.");
        }

        // Assert
        mockWindow.Received(1).Close();
        await Assert.That(appLifetime.ApplicationStopping.IsCancellationRequested)
            .IsTrue();
    }

    [Test]
    public async Task Window_Property_ShouldReturnLazyValue() {
        // Arrange
        IInfiniFrameWindow mockWindow = CreateMockWindow();
        WebApplicationBuilder webAppBuilder = WebApplication.CreateBuilder();
        WebApplication webApp = webAppBuilder.Build();

        var lazyWindow = new Lazy<IInfiniFrameWindow>(() => mockWindow);

        var app = new InfiniFrameWebApplication {
            WebApp = webApp,
            LazyWindow = lazyWindow
        };

        // Act
        IInfiniFrameWindow window = app.Window;

        // Assert
        await Assert.That(window).IsEqualTo(mockWindow);
        await Assert.That(lazyWindow.IsValueCreated).IsTrue();
    }
}
