// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;

namespace InfiniTests.InfiniFrame.Shared.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class JsHandlerNamesTests {

    [Test]
    public async Task AllConstants_StartWithInfiniFramePrefix(CancellationToken ct = default) {
        // Arrange
        string[] constants = [
            JsHandlerNames.FullscreenEnter,
            JsHandlerNames.FullscreenExit,
            JsHandlerNames.FullscreenToggle,
            JsHandlerNames.RegisterFullScreenChange,
            JsHandlerNames.OpenExternal,
            JsHandlerNames.RegisterOpenExternal,
            JsHandlerNames.TitleChanged,
            JsHandlerNames.RegisterTitleChange,
            JsHandlerNames.WindowReady,
            JsHandlerNames.WindowReadyAck,
            JsHandlerNames.GetRequest,
            JsHandlerNames.GetResponse,
            JsHandlerNames.WebMessageAckRequest,
            JsHandlerNames.WebMessageAckResponse,
            JsHandlerNames.WindowFeatureRequest,
            JsHandlerNames.WindowMinimize,
            JsHandlerNames.WindowMaximize,
            JsHandlerNames.WindowClose,
            JsHandlerNames.WindowToggleMaximize,
            JsHandlerNames.WindowRestoreFromMaximized,
            JsHandlerNames.WindowOffsetPosition,
            JsHandlerNames.WindowResize,
            JsHandlerNames.RegisterWindowClose,
            JsHandlerNames.JavaScriptEvalRequest,
            JsHandlerNames.JavaScriptEvalResult,
            JsHandlerNames.JavaScriptEvalResponse
        ];

        // Assert
        foreach (string constant in constants) {
            await Assert.That(constant).StartsWith("__infiniframe");
        }
    }

    [Test]
    public async Task WindowReady_EqualsExpectedValue(CancellationToken ct = default) {
        await Assert.That(JsHandlerNames.WindowReady).IsEqualTo("__infiniframe:ready");
    }

    [Test]
    public async Task GetRequest_EqualsExpectedValue(CancellationToken ct = default) {
        await Assert.That(JsHandlerNames.GetRequest).IsEqualTo("__infiniframe:get");
    }

    [Test]
    public async Task GetResponse_EqualsExpectedValue(CancellationToken ct = default) {
        await Assert.That(JsHandlerNames.GetResponse).IsEqualTo("__infiniframe:get:response");
    }

    [Test]
    public async Task WindowClose_EqualsExpectedValue(CancellationToken ct = default) {
        await Assert.That(JsHandlerNames.WindowClose).IsEqualTo("__infiniframe:window:close");
    }

    [Test]
    public async Task JavaScriptEvalRequest_EqualsExpectedValue(CancellationToken ct = default) {
        await Assert.That(JsHandlerNames.JavaScriptEvalRequest).IsEqualTo("__infiniframe:javascript:eval");
    }
}
