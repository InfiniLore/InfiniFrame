// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.WebMessaging.Handlers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowFeatureWebMessageHandlerTests {
    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("not-json")]
    [Arguments("{}")]
    [Arguments("{\"command\":null}")]
    [Arguments("{\"command\":\"decorations:title\"}")]
    [Arguments("{\"command\":\"__infiniframe:window:features::title\"}")]
    [Arguments("{\"command\":\"__infiniframe:window:features:decorations:\"}")]
    [Arguments("{\"command\":\"__infiniframe:window:features:decorations:title:extra\"}")]
    public async Task TryParseRequest_InvalidPayload_IsRejected(string? payload) {
        bool success = WindowFeatureWebMessageHandler.TryParseRequest(payload, out _);

        await Assert.That(success).IsFalse();
    }

    [Test]
    public async Task TryParseRequest_QualifiedCommand_SeparatesFeatureCommandAndArguments() {
        const string payload = """
            {
              "command": "__infiniframe:window:features:size:setSize",
              "args": { "width": 800, "height": 600 }
            }
            """;

        bool success = WindowFeatureWebMessageHandler.TryParseRequest(payload, out WindowFeatureWebMessageRequest request);

        await Assert.That(success).IsTrue();
        await Assert.That(request.FeatureName).IsEqualTo("size");
        await Assert.That(request.Command).IsEqualTo("setSize");
        await Assert.That(request.Args!.Value.GetProperty("width").GetInt32()).IsEqualTo(800);
        await Assert.That(request.Args.Value.GetProperty("height").GetInt32()).IsEqualTo(600);
    }

    [Test]
    public async Task TryParseRequest_ArgumentsRemainUsableAfterJsonDocumentIsDisposed() {
        const string payload = """{"command":"__infiniframe:window:features:state:setFullScreen","args":{"fullScreen":true}}""";

        WindowFeatureWebMessageHandler.TryParseRequest(payload, out WindowFeatureWebMessageRequest request);

        await Assert.That(request.Args!.Value.GetProperty("fullScreen").GetBoolean()).IsTrue();
    }
}
