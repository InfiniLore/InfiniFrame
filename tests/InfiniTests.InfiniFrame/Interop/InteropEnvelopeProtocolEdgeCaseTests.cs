// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;
using InfiniFrame.Interop;

namespace InfiniTests.InfiniFrame.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InteropEnvelopeProtocolEdgeCaseTests {

    [Test]
    public async Task ParseEmptyMessage_ReturnsFailure(CancellationToken ct = default) {
        // Arrange & Act
        InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage("");

        // Assert
        await Assert.That(result.IsFailure).IsTrue();
    }

    [Test]
    public async Task ParseWhitespaceMessage_ReturnsFailure(CancellationToken ct = default) {
        // Arrange & Act
        InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage("   ");

        // Assert
        await Assert.That(result.IsFailure).IsTrue();
    }

    [Test]
    public async Task ParseBlazorMessage_ReturnsBlazor(CancellationToken ct = default) {
        // Arrange & Act
        InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage("__bwv:some-data");

        // Assert
        await Assert.That(result.IsBlazor).IsTrue();
    }

    [Test]
    public async Task ParseNonJsonObject_ReturnsFailure(CancellationToken ct = default) {
        // Arrange & Act
        InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage("not-json");

        // Assert
        await Assert.That(result.IsFailure).IsTrue();
    }

    [Test]
    public async Task ParseJsonArray_ReturnsFailure(CancellationToken ct = default) {
        // Arrange & Act
        InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage("[]");

        // Assert
        await Assert.That(result.IsFailure).IsTrue();
    }

    [Test]
    public async Task ParseMissingId_ReturnsFailure(CancellationToken ct = default) {
        // Arrange
        string message = """{"command":"Post","version":2}""";

        // Act
        InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage(message);

        // Assert
        await Assert.That(result.IsFailure).IsTrue();
    }

    [Test]
    public async Task ParseEmptyId_ReturnsFailure(CancellationToken ct = default) {
        // Arrange
        string message = """{"id":"","command":"Post","version":2}""";

        // Act
        InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage(message);

        // Assert
        await Assert.That(result.IsFailure).IsTrue();
    }

    [Test]
    public async Task ParseMissingVersion_ReturnsFailure(CancellationToken ct = default) {
        // Arrange
        string message = """{"id":"test","command":"Post"}""";

        // Act
        InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage(message);

        // Assert
        await Assert.That(result.IsFailure).IsTrue();
    }

    [Test]
    public async Task ParseWrongVersion_ReturnsFailure(CancellationToken ct = default) {
        // Arrange
        string message = """{"id":"test","command":"Post","version":1}""";

        // Act
        InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage(message);

        // Assert
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error).Contains("Unsupported envelope version");
    }

    [Test]
    public async Task ParseMissingCommand_ReturnsFailure(CancellationToken ct = default) {
        // Arrange
        string message = """{"id":"test","version":2}""";

        // Act
        InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage(message);

        // Assert
        await Assert.That(result.IsFailure).IsTrue();
    }

    [Test]
    public async Task ParseUnsupportedCommand_ReturnsFailure(CancellationToken ct = default) {
        // Arrange
        string message = """{"id":"test","command":"Delete","version":2}""";

        // Act
        InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage(message);

        // Assert
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error).Contains("must be 'Post' or 'Get'");
    }

    [Test]
    public async Task ParseMalformedJson_ReturnsFailure(CancellationToken ct = default) {
        // Arrange & Act
        InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage("{broken");

        // Assert
        await Assert.That(result.IsFailure).IsTrue();
    }

    [Test]
    public async Task ParseNonStringRequestId_ReturnsFailure(CancellationToken ct = default) {
        // Arrange
        string message = """{"id":"test","command":"Post","version":2,"requestId":123}""";

        // Act
        InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage(message);

        // Assert
        await Assert.That(result.IsFailure).IsTrue();
    }

    [Test]
    public async Task ParseJsonObjectData_ReturnsRawText(CancellationToken ct = default) {
        // Arrange
        string message = """{"id":"test","command":"Post","version":2,"data":{"key":"value"}}""";

        // Act
        InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage(message);

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Payload).Contains("key");
    }

    [Test]
    public async Task ParseStringData_ReturnsStringValue(CancellationToken ct = default) {
        // Arrange
        string message = """{"id":"test","command":"Post","version":2,"data":"hello"}""";

        // Act
        InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage(message);

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Payload).IsEqualTo("hello");
    }

    [Test]
    public async Task ParseNullData_ReturnsNullPayload(CancellationToken ct = default) {
        // Arrange
        string message = """{"id":"test","command":"Post","version":2,"data":null}""";

        // Act
        InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage(message);

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Payload).IsNull();
    }

    [Test]
    public async Task ParseNoData_ReturnsNullPayload(CancellationToken ct = default) {
        // Arrange
        string message = """{"id":"test","command":"Post","version":2}""";

        // Act
        InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage(message);

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Payload).IsNull();
    }

    [Test]
    public async Task ParseJsonEncodedString_UnwrapsAndParses(CancellationToken ct = default) {
        // Arrange, a JSON-encoded string containing a valid envelope
        string innerEnvelope = """{"id":"test","command":"Post","version":2,"data":"hello"}""";
        string encoded = JsonSerializer.Serialize(innerEnvelope);

        // Act
        InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage(encoded);

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.MessageId).IsEqualTo("test");
    }
}
