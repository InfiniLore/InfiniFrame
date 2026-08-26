// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using InfiniFrame;
using InfiniFrame.Interop;
using InfiniFrame.NativeBridge.Parameters;
using InfiniTests.Substitutes;
using Microsoft.Extensions.Logging.Abstractions;

namespace InfiniTests.InfiniFrame.Window.Features.JavaScript;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[SuppressMessage("ReSharper", "AsyncMethodWithoutAwait")]
[SuppressMessage("Usage", "TUnitAssertions0005:Assert.That(...) should not be used with a constant value")]
public class ExecuteJavaScriptTests {
    [Test]
    public async Task EvalResultHandler_CompletesPendingEval(CancellationToken ct) {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameEvents events, RecordingInfiniFrameWindowSubstitute _)
            = CreateWindowHarness();
        builder.RegisterGetWebMessageHandler();

        // Simulate an eval result message coming from JS
        string resultPayload = "{\"requestId\":\"eval_1\",\"result\":\"42\"}";
        string inboundMessage = InteropEnvelopeProtocol.CreateEnvelopeMessage(
            JsHandlerNames.JavaScriptEvalResult,
            resultPayload
        );

        // Act + Assert - Should not throw
        events.OnWebMessageReceived(inboundMessage);
    }

    [Test]
    public async Task EvalResultHandler_WithError_CompletesPendingEvalWithError(CancellationToken ct) {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameEvents events, RecordingInfiniFrameWindowSubstitute _)
            = CreateWindowHarness();
        builder.RegisterGetWebMessageHandler();

        // Simulate an eval error result from JS
        string resultPayload = "{\"requestId\":\"eval_2\",\"error\":\"Syntax error\"}";
        string inboundMessage = InteropEnvelopeProtocol.CreateEnvelopeMessage(
            JsHandlerNames.JavaScriptEvalResult,
            resultPayload
        );

        // Act + Assert - Should not throw
        events.OnWebMessageReceived(inboundMessage);
    }

    [Test]
    public async Task EvalResultHandler_InvalidPayload_DoesNotThrow(CancellationToken ct) {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameEvents events, RecordingInfiniFrameWindowSubstitute _)
            = CreateWindowHarness();
        builder.RegisterGetWebMessageHandler();

        // Simulate an invalid eval result
        string inboundMessage = InteropEnvelopeProtocol.CreateEnvelopeMessage(
            JsHandlerNames.JavaScriptEvalResult,
            "not-valid-json"
        );

        // Act + Assert - Should not throw
        events.OnWebMessageReceived(inboundMessage);
    }

    [Test]
    public async Task EvalResultHandler_MissingRequestId_DoesNotThrow(CancellationToken ct) {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameEvents events, RecordingInfiniFrameWindowSubstitute _)
            = CreateWindowHarness();
        builder.RegisterGetWebMessageHandler();

        // Simulate an eval result without requestId
        string resultPayload = "{\"result\":\"42\"}";
        string inboundMessage = InteropEnvelopeProtocol.CreateEnvelopeMessage(
            JsHandlerNames.JavaScriptEvalResult,
            resultPayload
        );

        // Act + Assert - Should not throw
        events.OnWebMessageReceived(inboundMessage);
    }

    [Test]
    public async Task EvalResultHandler_NullResult_SetsNull(CancellationToken ct) {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameEvents events, RecordingInfiniFrameWindowSubstitute _)
            = CreateWindowHarness();
        builder.RegisterGetWebMessageHandler();

        // Simulate an eval result with null
        string resultPayload = "{\"requestId\":\"eval_3\",\"result\":null}";
        string inboundMessage = InteropEnvelopeProtocol.CreateEnvelopeMessage(
            JsHandlerNames.JavaScriptEvalResult,
            resultPayload
        );

        // Act + Assert - Should not throw
        events.OnWebMessageReceived(inboundMessage);
    }

    [Test]
    public async Task JavaScriptHandlerName_Constants_AreCorrect(CancellationToken ct) {
        // Assert
        await Assert.That(JsHandlerNames.JavaScriptEvalRequest).IsEqualTo("__infiniframe:javascript:eval");
        await Assert.That(JsHandlerNames.JavaScriptEvalResult).IsEqualTo("__infiniframe:javascript:eval:result");
    }

    private static (InfiniFrameWindowBuilder Builder, InfiniFrameEvents Events, RecordingInfiniFrameWindowSubstitute Window) CreateWindowHarness() {
        var builder = InfiniFrameWindowBuilder.Create();
        var eventsStore = (InfiniFrameEventsStore)builder.EventsStore;

        RecordingInfiniFrameWindowSubstitute window = new RecordingInfiniFrameWindowSubstitute()
            .BindToBuilder(builder);

        var events = new InfiniFrameEvents(eventsStore, NullLogger<InfiniFrameEvents>.Instance);
        var nativeParameters = default(InfiniFrameNativeParameters);
        events.AssignToNativeParameters(ref nativeParameters);
        events.AssignToWindow(window.Window);

        return (builder, events, window);
    }
}
