// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using NSubstitute;

namespace InfiniTests.InfiniFrame.Window.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class RegisterWebMessageReceivedHandlerTests {
    [Test]
    public async Task AtBuilderStage_HandlerWithService_ResolvesServiceFromWindowServiceProvider(CancellationToken ct = default) {
        // Arrange
        var eventsStore = new InfiniFrameEventsStore();
        var builder = InfiniFrameWindowBuilder.Create(events: eventsStore);
        var service = new TestService();
        var window = Substitute.For<IInfiniFrameWindow>();
        window.ServiceProvider.Returns(new TestServiceProvider(service));

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
    public async Task AtBuilderStage_HandlerWithOrigin_ReceivesOriginFromEventPayload(CancellationToken ct = default) {
        // Arrange
        var eventsStore = new InfiniFrameEventsStore();
        var builder = InfiniFrameWindowBuilder.Create(events: eventsStore);
        var window = Substitute.For<IInfiniFrameWindow>();

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
            _services = services.ToDictionary(static service => service.GetType(), static service => service);
        }

        public object? GetService(Type serviceType)
            => _services.TryGetValue(serviceType, out object? service) ? service : null;
    }
}
