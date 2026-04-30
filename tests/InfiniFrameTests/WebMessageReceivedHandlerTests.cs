// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.HostMessaging;
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
        var events = new InfiniFrameWindowEvents();
        var builder = InfiniFrameWindowBuilder.Create(events: events);
        var service = new TestService();
        var window = new InfiniFrameWindow {
            Logger = NullLogger<IInfiniFrameWindow>.Instance,
            ServiceProvider =  new TestServiceProvider(service),
            CustomSchemes = new InfiniFrameWindowCustomSchemeHandlers(),
            Parent = null,
            Events = events,
            MessageHandlers = new InfiniFrameWindowMessageHandler()
        };
        events.CompleteSetup(window);

        var tcs = new TaskCompletionSource<(string ServiceId, string Message)>();

        builder.RegisterWebMessageReceivedHandler((TestService resolvedService, IInfiniFrameWindow _, string message) => {
            tcs.TrySetResult((resolvedService.Id, message));
        });

        // Act
        builder.Events.OnWebMessageReceived("ping");

        // Assert
        (string ServiceId, string Message) result = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(1));
        await Assert.That(result.ServiceId).IsEqualTo(service.Id);
        await Assert.That(result.Message).IsEqualTo("ping");
    }
}
