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
    [Arguments(JsHandlerNames.FullscreenEnter)]
    [Arguments(JsHandlerNames.FullscreenExit)]
    [Arguments(JsHandlerNames.FullscreenToggle)]
    [Arguments(JsHandlerNames.RegisterFullScreenChange)]
    [Arguments(JsHandlerNames.OpenExternal)]
    [Arguments(JsHandlerNames.RegisterOpenExternal)]
    [Arguments(JsHandlerNames.TitleChanged)]
    [Arguments(JsHandlerNames.RegisterTitleChange)]
    [Arguments(JsHandlerNames.WindowReady)]
    [Arguments(JsHandlerNames.WindowReadyAck)]
    [Arguments(JsHandlerNames.GetRequest)]
    [Arguments(JsHandlerNames.GetResponse)]
    [Arguments(JsHandlerNames.WebMessageAckRequest)]
    [Arguments(JsHandlerNames.WebMessageAckResponse)]
    [Arguments(JsHandlerNames.WindowFeatureRequest)]
    [Arguments(JsHandlerNames.WindowMinimize)]
    [Arguments(JsHandlerNames.WindowMaximize)]
    [Arguments(JsHandlerNames.WindowClose)]
    [Arguments(JsHandlerNames.WindowToggleMaximize)]
    [Arguments(JsHandlerNames.WindowRestoreFromMaximized)]
    [Arguments(JsHandlerNames.WindowOffsetPosition)]
    [Arguments(JsHandlerNames.WindowResize)]
    [Arguments(JsHandlerNames.RegisterWindowClose)]
    [Arguments(JsHandlerNames.JavaScriptEvalRequest)]
    [Arguments(JsHandlerNames.JavaScriptEvalResult)]
    [Arguments(JsHandlerNames.JavaScriptEvalResponse)]
    public async Task AllConstants_StartWithInfiniFramePrefix(string input, CancellationToken ct = default) {
        // Arrange
        const string expected = "__infiniframe";
        
        // Act

        // Assert
        await Assert.That(input).StartsWith(expected);
    }

    [Test]
    public async Task WindowReady_EqualsExpectedValue(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(JsHandlerNames.WindowReady).IsEqualTo("__infiniframe:ready");
    }

    [Test]
    public async Task GetRequest_EqualsExpectedValue(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(JsHandlerNames.GetRequest).IsEqualTo("__infiniframe:get");
    }

    [Test]
    public async Task GetResponse_EqualsExpectedValue(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(JsHandlerNames.GetResponse).IsEqualTo("__infiniframe:get:response");
    }

    [Test]
    public async Task WindowClose_EqualsExpectedValue(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(JsHandlerNames.WindowClose).IsEqualTo("__infiniframe:window:close");
    }

    [Test]
    public async Task JavaScriptEvalRequest_EqualsExpectedValue(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(JsHandlerNames.JavaScriptEvalRequest).IsEqualTo("__infiniframe:javascript:eval");
    }
}
