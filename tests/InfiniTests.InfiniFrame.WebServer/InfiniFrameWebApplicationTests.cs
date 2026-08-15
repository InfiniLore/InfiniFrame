// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Blazor;
using InfiniFrame.WebServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;

namespace InfiniTests.InfiniFrame.WebServer;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWebApplicationTests {
    private const int DefaultGetMessageHandlerCount = 1;

    private static (Mock<IInfiniFrameWindow> Mock, IInfiniFrameWindow Object) CreateMockWindow() {
        Mock<IInfiniFrameWindow> mockWindow = MockFactory.CreateWindowMock();
        var eventsStore = new InfiniFrameEventsStore();
        mockWindow.Events.Returns(new InfiniFrameEvents(eventsStore, NullLogger<InfiniFrameEvents>.Instance));
        mockWindow.EventsStore.Returns(eventsStore);
        return (mockWindow, mockWindow.Object);
    }

    [Test]
    public async Task CreateBuilder_ShouldReturnValidBuilder() {
        // Arrange & Act
        InfiniFrameWebApplicationBuilder builder = InfiniFrameWebApplication.CreateBuilder();

        // Assert
        await Assert.That(builder).IsNotNull();
        await Assert.That(builder.WebApp).IsNotNull();
        await Assert.That(builder.WindowBuilder).IsNotNull();
    }

    [Test]
    public async Task Build_DefaultWebMessageHandlersWithoutBlazorJsRuntime_ShouldPassServiceValidation() {
        // Arrange
        InfiniFrameWebApplicationBuilder builder = InfiniFrameWebApplication.CreateBuilder();
        builder.WebApp.Host.UseDefaultServiceProvider(static options => {
            options.ValidateOnBuild = true;
            options.ValidateScopes = true;
        });

        // Act
        InfiniFrameWebApplication app = builder.Build();

        // Assert
        await Assert.That(builder.Services.Any(static descriptor => descriptor.ServiceType == typeof(IInfiniFrameJs)))
            .IsFalse();
        await Assert.That(builder.WindowBuilder.EventsStore.WebMessageGetData.Count).IsGreaterThanOrEqualTo(DefaultGetMessageHandlerCount);

        await app.WebApp.DisposeAsync();
    }

    [Test]
    public async Task UseAutoServerClose_WhenWindowNotCreated_ShouldRegisterWithBuilder() {
        // Arrange
        Mock<IInfiniFrameWindowBuilder> mockWindowBuilder = MockFactory.CreateWindowBuilderMock();
        var mockBuilderEventsStore = new InfiniFrameEventsStore();
        mockWindowBuilder.EventsStore.Returns(mockBuilderEventsStore);

        WebApplicationBuilder webAppBuilder = WebApplication.CreateBuilder();
        webAppBuilder.Services.AddSingleton<IInfiniFrameWindowBuilder>(mockWindowBuilder.Object);

        WebApplication webApp = webAppBuilder.Build();
        var lazyWindow = new Lazy<IInfiniFrameWindow>(() => CreateMockWindow().Object);

        var app = new InfiniFrameWebApplication {
            Logger = NullLogger<InfiniFrameWebApplication>.Instance,
            WebApp = webApp,
            LazyWindow = lazyWindow
        };

        // Act
        InfiniFrameWebApplication result = app.UseAutoServerClose();

        // Assert
        await Assert.That(result).IsEqualTo(app);
        await Assert.That(mockBuilderEventsStore.Closing.Snapshot.Length).IsGreaterThan(0);
    }

    [Test]
    public async Task UseAutoServerClose_WhenWindowCreated_ShouldRegisterWithWindow() {
        // Arrange
        (_, IInfiniFrameWindow mockWindow) = CreateMockWindow();
        IInfiniFrameEvents mockEvents = mockWindow.Events;

        WebApplicationBuilder webAppBuilder = WebApplication.CreateBuilder();
        WebApplication webApp = webAppBuilder.Build();

        var lazyWindow = new Lazy<IInfiniFrameWindow>(() => mockWindow);
        // Force window creation
        _ = lazyWindow.Value;

        var app = new InfiniFrameWebApplication {
            Logger = NullLogger<InfiniFrameWebApplication>.Instance,
            WebApp = webApp,
            LazyWindow = lazyWindow
        };

        // Act
        InfiniFrameWebApplication result = app.UseAutoServerClose();

        // Assert
        await Assert.That(result).IsEqualTo(app);
        await Assert.That(mockEvents.EventsStore.Closing.Snapshot.Length).IsGreaterThan(0);
    }

    [Test]
    public async Task UseAutoServerClose_ClosingHandler_ShouldReturnFalse() {
        // Arrange
        (_, IInfiniFrameWindow mockWindow) = CreateMockWindow();
        IInfiniFrameEvents mockEvents = mockWindow.Events;

        WebApplicationBuilder webAppBuilder = WebApplication.CreateBuilder();
        WebApplication webApp = webAppBuilder.Build();

        var lazyWindow = new Lazy<IInfiniFrameWindow>(() => mockWindow);
        _ = lazyWindow.Value;

        var app = new InfiniFrameWebApplication {
            Logger = NullLogger<InfiniFrameWebApplication>.Instance,
            WebApp = webApp,
            LazyWindow = lazyWindow
        };

        // Act
        app.UseAutoServerClose();
        Func<IInfiniFrameWindow, EventArgs?, WindowClosingResult>? capturedHandler = mockEvents.EventsStore.Closing.Snapshot.LastOrDefault();
        WindowClosingResult result = capturedHandler?.Invoke(mockWindow, EventArgs.Empty) ?? WindowClosingResult.Cancel;

        // Assert
        await Assert.That(capturedHandler).IsNotNull();
        await Assert.That(result).IsEqualTo(WindowClosingResult.Close);
    }

    [Test]
    public async Task UseAutoServerClose_ClosingHandler_ShouldInitiateStopAsync() {
        // Arrange
        (_, IInfiniFrameWindow mockWindow) = CreateMockWindow();
        IInfiniFrameEvents mockEvents = mockWindow.Events;

        WebApplicationBuilder webAppBuilder = WebApplication.CreateBuilder();
        WebApplication webApp = webAppBuilder.Build();

        var lazyWindow = new Lazy<IInfiniFrameWindow>(() => mockWindow);
        _ = lazyWindow.Value;

        var app = new InfiniFrameWebApplication {
            Logger = NullLogger<InfiniFrameWebApplication>.Instance,
            WebApp = webApp,
            LazyWindow = lazyWindow
        };

        app.UseAutoServerClose();

        // Act
        Func<IInfiniFrameWindow, EventArgs?, WindowClosingResult>? capturedHandler = mockEvents.EventsStore.Closing.Snapshot.LastOrDefault();
        capturedHandler?.Invoke(mockWindow, EventArgs.Empty);

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
    public async Task Window_Property_ShouldReturnLazyValue() {
        // Arrange
        (_, IInfiniFrameWindow mockWindow) = CreateMockWindow();
        WebApplicationBuilder webAppBuilder = WebApplication.CreateBuilder();
        WebApplication webApp = webAppBuilder.Build();

        var lazyWindow = new Lazy<IInfiniFrameWindow>(() => mockWindow);

        var app = new InfiniFrameWebApplication {
            Logger = NullLogger<InfiniFrameWebApplication>.Instance,
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
