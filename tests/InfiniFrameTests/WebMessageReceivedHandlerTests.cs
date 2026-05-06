// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Native;
using Microsoft.Extensions.Logging.Abstractions;

namespace InfiniFrameTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WebMessageReceivedHandlerTests {
    private sealed class TestService {
        public string Id { get; } = Guid.NewGuid().ToString("N");
    }

    private sealed class TestServiceProvider : IServiceProvider {
        private readonly Dictionary<Type, object> _services;

        public TestServiceProvider(params object[] services) {
            _services = services.ToDictionary(service => service.GetType(), service => service);
        }

        public object? GetService(Type serviceType) {
            return _services.TryGetValue(serviceType, out object? service) ? service : null;
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Handler_ResolvesServiceFromProvider() {
        // Arrange
        var eventsStore = new InfiniFrameWindowEventsStore();
        var events = new InfiniFrameWindowEvents(eventsStore);
        var builder = InfiniFrameWindowBuilder.Create(eventsStore);
        var service = new TestService();
        var window = new InfiniFrameWindow {
            Logger = NullLogger<IInfiniFrameWindow>.Instance,
            ServiceProvider =  new TestServiceProvider(service),
            Parent = null,
            Events = events
        };
        var nativeParameters = default(InfiniFrameNativeParameters);
        events.CompleteSetup(window, ref nativeParameters);

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
    public async Task Handler_WithOrigin_ReceivesOriginFromEventPayload() {
        // Arrange
        var eventsStore = new InfiniFrameWindowEventsStore();
        var builder = InfiniFrameWindowBuilder.Create(eventsStore);
        var window = new InfiniFrameWindow {
            Logger = NullLogger<IInfiniFrameWindow>.Instance,
            ServiceProvider = null,
            Parent = null,
            Events = new InfiniFrameWindowEvents(eventsStore)
        };

        var tcs = new TaskCompletionSource<string?>();
        builder.RegisterWebMessageReceivedHandler((_, _, origin) => tcs.TrySetResult(origin));

        // Act
        eventsStore.WebMessageReceived.Invoke(window, new InfiniFrameWebMessageReceivedEvent("ping", "https://example.test"));

        // Assert
        string? origin = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(1));
        await Assert.That(origin).IsEqualTo("https://example.test");
    }
}
