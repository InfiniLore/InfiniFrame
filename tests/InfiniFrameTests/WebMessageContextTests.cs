// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Shared.TestDoubles;

namespace InfiniFrameTests;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WebMessageContextTests {
    [Test]
    public async Task Push_RestoresPreviousOriginOnDispose() {
        // Arrange
        string? originInsideScope;
        string? originAfterScope;

        // Act
        using (InfiniFrameWebMessageContext.Push("https://outer.example")) {
            using (InfiniFrameWebMessageContext.Push("https://inner.example")) {
                originInsideScope = InfiniFrameWebMessageContext.CurrentOrigin;
            }

            originAfterScope = InfiniFrameWebMessageContext.CurrentOrigin;
        }

        // Assert
        await Assert.That(originInsideScope).IsEqualTo("https://inner.example");
        await Assert.That(originAfterScope).IsEqualTo("https://outer.example");
        await Assert.That(InfiniFrameWebMessageContext.CurrentOrigin).IsNull();
    }

    [Test]
    public async Task OnWebMessageReceived_WithOrigin_SetsAmbientOriginOnlyDuringHandlerInvocation() {
        // Arrange
        var events = new InfiniFrameWindowEvents();
        var window = new RecordingInfiniFrameWindowSubstitute();
        events.CompleteSetup(window.Window);

        string? observedInsideHandler = "not-set";
        events.WebMessageReceived.Add((_, _) => {
            observedInsideHandler = InfiniFrameWebMessageContext.CurrentOrigin;
        });

        // Act
        events.OnWebMessageReceived("ping", "https://webview.example");
        string? observedAfterHandler = InfiniFrameWebMessageContext.CurrentOrigin;

        // Assert
        await Assert.That(observedInsideHandler).IsEqualTo("https://webview.example");
        await Assert.That(observedAfterHandler).IsNull();
    }

    [Test]
    public async Task OnWebMessageReceived_WithoutOrigin_DoesNotLeakOuterAmbientOrigin() {
        // Arrange
        var events = new InfiniFrameWindowEvents();
        var window = new RecordingInfiniFrameWindowSubstitute();
        events.CompleteSetup(window.Window);

        string? observedInsideHandler = "not-set";
        string? observedAfterHandler;
        events.WebMessageReceived.Add((_, _) => {
            observedInsideHandler = InfiniFrameWebMessageContext.CurrentOrigin;
        });

        // Act
        using (InfiniFrameWebMessageContext.Push("https://outer.example")) {
            events.OnWebMessageReceived("ping");
            observedAfterHandler = InfiniFrameWebMessageContext.CurrentOrigin;
        }

        // Assert
        await Assert.That(observedInsideHandler).IsNull();
        await Assert.That(observedAfterHandler).IsEqualTo("https://outer.example");
    }
}
