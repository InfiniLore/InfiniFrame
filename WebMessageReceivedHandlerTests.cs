// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Debugging;
using InfiniFrame.NativeBridge.Parameters;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace InfiniTests.InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WebMessageReceivedHandlerTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Handler_ResolvesServiceFromProvider(CancellationToken ct = default) {
        // Arrange
        var eventsStore = new InfiniFrameEventsStore();
        var events = new InfiniFrameEvents(NullLogger<InfiniFrameEvents>.Instance, eventsStore);
        var debugging = new InfiniFrameWindowDebugging(NullLogger<InfiniFrameWindowDebugging>.Instance);
        var builder = InfiniFrameWindowBuilder.Create(null, eventsStore);
        var service = new TestService();
        var window = new InfiniFrameWindow {
            ServiceProvider = new TestServiceProvider(service), 
            Events = events,
            Debugging = debugging,
            Configuration = Substitute.For<IInfiniFrameWindowConfiguration>()
        };
        var nativeParameters = default(InfiniFrameNativeParameters);
        events.AssignEventCallbacks(ref nativeParameters);
        events.AssignToWindow(window);
        debugging.AssignToWindow(window);

        var tcs = new TaskCompletionSource<(string ServiceId, string Message)>();

        builder.RegisterWebMessageReceivedHandler((IInfiniFrameWindow _, string message, TestService resolvedService) => {
            tcs.TrySetResult((resolvedService.Id, message));
        });

        // Act
        eventsStore.WebMessageReceived.Invoke(window, new InfiniFrameWebMessageReceivedEvent("ping", null));

        // Assert
        (string ServiceId, string Message) result = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(1));
        await Assert.That(result.ServiceId).IsEqualTo(service.Id);
        await Assert.That(result.Message).IsEqualTo("ping");
    }

    [Test]
    public async Task Handler_WithOrigin_ReceivesOriginFromEventPayload(CancellationToken ct = default) {
        // Arrange
        var eventsStore = new InfiniFrameEventsStore();
        var builder = InfiniFrameWindowBuilder.Create(eventsStore);
        var debugging = new InfiniFrameWindowDebugging(NullLogger<InfiniFrameWindowDebugging>.Instance);
        var window = new InfiniFrameWindow {
            Logger = NullLogger<IInfiniFrameWindow>.Instance,
            ServiceProvider = null,
            Events = new InfiniFrameEvents(NullLogger<InfiniFrameEvents>.Instance, eventsStore),
            Debugging = debugging,
            Configuration = Substitute.For<IInfiniFrameWindowConfiguration>(),
            StaticAssets = null
        };

        var tcs = new TaskCompletionSource<string?>();
        builder.RegisterWebMessageReceivedHandler((_, _, origin) => tcs.TrySetResult(origin));

        // Act
        eventsStore.WebMessageReceived.Invoke(window, new InfiniFrameWebMessageReceivedEvent("ping", "https://example.test"));

        // Assert
        string? origin = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(1));
        await Assert.That(origin).IsEqualTo("https://example.test");
    }

    private sealed class TestService {
        public string Id { get; } = Guid.NewGuid().ToString("N");
    }

    private sealed class TestServiceProvider : IServiceProvider {
        private readonly Dictionary<Type, object> _services;

        public TestServiceProvider(params object[] services) {
            _services = services.ToDictionary(keySelector: service => service.GetType(), elementSelector: service => service);
        }

        public object? GetService(Type serviceType) => _services.TryGetValue(serviceType, out object? service) ? service : null;
    }
}
