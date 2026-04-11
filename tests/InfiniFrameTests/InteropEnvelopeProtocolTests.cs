// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;
using System.Text.Json;

namespace InfiniFrameTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InteropEnvelopeProtocolTests {
    [Test]
    [DisplayName($"{nameof(InteropEnvelopeProtocolTests)}.{nameof(RoundTrip_StringPayload)}")]
    public async Task RoundTrip_StringPayload() {
        // Arrange
        string message = InteropEnvelopeProtocol.CreateEnvelopeMessage("ping", "hello");

        // Act
        InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage(message);

        // Assert
        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.MessageId).IsEqualTo("ping");
        await Assert.That(result.Payload).IsEqualTo("hello");
    }

    [Test]
    [DisplayName($"{nameof(InteropEnvelopeProtocolTests)}.{nameof(RoundTrip_NestedPayload)}")]
    public async Task RoundTrip_NestedPayload() {
        // Arrange
        var payload = new {
            name = "München",
            values = new[] { 1, 2, 3 },
            nested = new { ok = true }
        };
        string message = InteropEnvelopeProtocol.CreateEnvelopeMessage("complex", payload);

        // Act
        InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage(message);

        // Assert
        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.MessageId).IsEqualTo("complex");
        await Assert.That(result.Payload).IsNotNull();

        using JsonDocument parsedPayload = JsonDocument.Parse(result.Payload!);
        await Assert.That(parsedPayload.RootElement.GetProperty("name").GetString()).IsEqualTo("München");
        await Assert.That(parsedPayload.RootElement.GetProperty("values").GetArrayLength()).IsEqualTo(3);
        await Assert.That(parsedPayload.RootElement.GetProperty("nested").GetProperty("ok").GetBoolean()).IsTrue();
    }

    [Test]
    [DisplayName($"{nameof(InteropEnvelopeProtocolTests)}.{nameof(Parse_InvalidEnvelope_MissingId_IsRejected)}")]
    public async Task Parse_InvalidEnvelope_MissingId_IsRejected() {
        // Arrange
        const string message = """{"data":"x","version":1}""";

        // Act
        InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage(message);

        // Assert
        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.Error).Contains("id");
    }

    [Test]
    [DisplayName($"{nameof(InteropEnvelopeProtocolTests)}.{nameof(Parse_InvalidEnvelope_UnsupportedVersion_IsRejected)}")]
    public async Task Parse_InvalidEnvelope_UnsupportedVersion_IsRejected() {
        // Arrange
        const string message = """{"id":"ping","data":"x","version":2}""";

        // Act
        InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage(message);

        // Assert
        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.Error).Contains("Unsupported envelope version");
    }

    [Test]
    [DisplayName($"{nameof(InteropEnvelopeProtocolTests)}.{nameof(Parse_LegacyMessage_IsRejected)}")]
    public async Task Parse_LegacyMessage_IsRejected() {
        // Arrange
        const string message = "set-title;New Title";

        // Act
        InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage(message);

        // Assert
        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.Error).Contains("not a valid JSON envelope");
    }

    [Test]
    [DisplayName($"{nameof(InteropEnvelopeProtocolTests)}.{nameof(Parse_Envelope_WithDelimitersInStringPayload_IsPreserved)}")]
    public async Task Parse_Envelope_WithDelimitersInStringPayload_IsPreserved() {
        // Arrange
        const string message = """{"id":"event","data":"value;with;semicolons","version":1}""";

        // Act
        InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage(message);

        // Assert
        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.MessageId).IsEqualTo("event");
        await Assert.That(result.Payload).IsEqualTo("value;with;semicolons");
    }

    [Test]
    [DisplayName($"{nameof(InteropEnvelopeProtocolTests)}.{nameof(Parse_MalformedJsonWithoutLegacySignature_IsRejected)}")]
    public async Task Parse_MalformedJsonWithoutLegacySignature_IsRejected() {
        // Arrange
        const string message = "{not-json";

        // Act
        InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage(message);

        // Assert
        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.Error).Contains("malformed");
    }

    [Test]
    [DisplayName($"{nameof(InteropEnvelopeProtocolTests)}.{nameof(Parse_TooLargeMessage_IsRejected)}")]
    public async Task Parse_TooLargeMessage_IsRejected() {
        // Arrange
        string message = new('a', InteropEnvelopeProtocol.MaxMessageSizeBytes + 1);

        // Act
        InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage(message);

        // Assert
        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.Error).Contains("exceeds max size");
    }
}
