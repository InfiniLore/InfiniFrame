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
    private const string TestMessageCommand = nameof(TestMessageCommand);
    
    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task OnWebMessageReceived_WithOrigin_PublishesOriginViaEventPayload(CancellationToken ct = default) {
        // Arrange
        var eventsStore = new InfiniFrameEventsStore();
        var events = new InfiniFrameEvents(eventsStore);
        var window = new RecordingInfiniFrameWindowSubstitute();
        var nativeParameters = default(InfiniFrameNativeParameters);
        events.AssignEventCallbacks(ref nativeParameters);
        events.AssignSender(window.Window);

        string? observedMessage = null;
        eventsStore.WebMessagePostData.Add(TestMessageCommand, (_, message) => {
            observedMessage = message;
        });

        // Act
        events.OnWebMessageReceived(CreatePostEnvelope(TestMessageCommand,"TEST" ), "https://webview.example");

        // Assert
        await Assert.That(observedMessage).IsNotNull();
    }

    [Test]
    public async Task OnWebMessageReceived_WithBlazorWebViewMessage_PublishesRawMessage(CancellationToken ct = default) {
        // Arrange
        var eventsStore = new InfiniFrameEventsStore();
        var events = new InfiniFrameEvents(eventsStore);
        var window = new RecordingInfiniFrameWindowSubstitute();
        var nativeParameters = default(InfiniFrameNativeParameters);
        events.AssignEventCallbacks(ref nativeParameters);
        events.AssignSender(window.Window);

        string? observedMessage = null;
        string? observedOrigin = null;
        eventsStore.WebMessageReceived.Add((_, payload) => {
            observedMessage = payload.Message;
            observedOrigin = payload.Origin;
        });

        const string blazorWebViewMessage = "__bwv:[\"AttachPage\",\"app://localhost/\",\"app://localhost/\"]";

        // Act
        events.OnWebMessageReceived(blazorWebViewMessage, "app://localhost/");

        // Assert
        await Assert.That(observedMessage).IsEqualTo(blazorWebViewMessage);
        await Assert.That(observedOrigin).IsEqualTo("app://localhost/");
    }

    private static string CreatePostEnvelope(string id, string? data = null)
        => JsonSerializer.Serialize(new {
            id,
            command = "Post",
            version = 2,
            data
        });
}
