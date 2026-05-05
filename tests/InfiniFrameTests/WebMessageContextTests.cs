// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Native;
using InfiniFrameTests.Shared.TestDoubles;
using System.Text.Json;

namespace InfiniFrameTests;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WebMessageContextTests {
    [Test]
    public async Task OnWebMessageReceived_WithOrigin_PublishesOriginViaEventPayload() {
        // Arrange
        var eventsStore = new InfiniFrameWindowEventsStore();
        var events = new InfiniFrameWindowEvents(eventsStore);
        var window = new RecordingInfiniFrameWindowSubstitute();
        var nativeParameters = default(InfiniFrameNativeParameters);
        events.CompleteSetup(window.Window, ref nativeParameters);

        string? observedMessage = null;
        string? observedOrigin = null;
        eventsStore.WebMessageReceived.Add((_, payload) => {
            observedMessage = payload.Message;
            observedOrigin = payload.Origin;
        });

        // Act
        events.OnWebMessageReceived(CreatePostEnvelope("ping"), "https://webview.example");

        // Assert
        await Assert.That(observedMessage).IsNotNull();
        await Assert.That(observedOrigin).IsEqualTo("https://webview.example");
    }

    [Test]
    public async Task OnWebMessageReceived_WithoutOrigin_PublishesNullOrigin() {
        // Arrange
        var eventsStore = new InfiniFrameWindowEventsStore();
        var events = new InfiniFrameWindowEvents(eventsStore);
        var window = new RecordingInfiniFrameWindowSubstitute();
        var nativeParameters = default(InfiniFrameNativeParameters);
        events.CompleteSetup(window.Window, ref nativeParameters);

        string? observedOrigin = "unset";
        eventsStore.WebMessageReceived.Add((_, payload) => observedOrigin = payload.Origin);

        // Act
        events.OnWebMessageReceived(CreatePostEnvelope("ping"));

        // Assert
        await Assert.That(observedOrigin).IsNull();
    }

    private static string CreatePostEnvelope(string id)
        => JsonSerializer.Serialize(new {
            id,
            command = "Post",
            version = 2,
            data = (string?)null
        });
}
