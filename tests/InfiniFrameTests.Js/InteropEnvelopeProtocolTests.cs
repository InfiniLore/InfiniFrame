// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;
using InfiniFrame.Interop;
using InfiniFrame.Js.Interop;

namespace InfiniFrameTests.Js;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InteropEnvelopeProtocolTests {
    private static readonly JsonDocument GoldenVectors = JsonDocument.Parse(
        File.ReadAllText(
            Path.GetFullPath(
                Path.Join("TypeScript", "Interop", "interop-envelope-golden-vectors.json"),
                AppContext.BaseDirectory
            )
        )
    );

    [Test]
    [DisplayName($"{nameof(InteropEnvelopeProtocolTests)}.{nameof(CreateEnvelope_GoldenVectors)}")]
    public async Task CreateEnvelope_GoldenVectors() {
        JsonElement vectors = GoldenVectors.RootElement.GetProperty("createVectors");
        foreach (JsonElement vector in vectors.EnumerateArray()) {
            string id = vector.GetProperty("id").GetString()!;
            string expectedMessage = vector.GetProperty("expectedMessage").GetString()!;
            string? data = vector.GetProperty("data").ValueKind == JsonValueKind.Null
                ? null
                : vector.GetProperty("data").GetString();

            string message = InteropEnvelopeProtocol.CreateEnvelopeMessage(id, data);
            await Assert.That(message).IsEqualTo(expectedMessage);
        }
    }

    [Test]
    [DisplayName($"{nameof(InteropEnvelopeProtocolTests)}.{nameof(ParseEnvelope_GoldenVectors)}")]
    public async Task ParseEnvelope_GoldenVectors() {
        JsonElement vectors = GoldenVectors.RootElement.GetProperty("parseVectors");
        foreach (JsonElement vector in vectors.EnumerateArray()) {
            string message = vector.GetProperty("message").GetString()!;
            bool expectedSuccess = vector.GetProperty("success").GetBoolean();

            InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage(message);

            await Assert.That(result.Success).IsEqualTo(expectedSuccess);

            if (!expectedSuccess) {
                string errorContains = vector.GetProperty("errorContains").GetString()!;
                await Assert.That(result.Error).Contains(errorContains);
                continue;
            }

            string expectedMessageId = vector.GetProperty("messageId").GetString()!;
            string? expectedPayload = vector.GetProperty("payload").ValueKind == JsonValueKind.Null
                ? null
                : vector.GetProperty("payload").GetString();

            await Assert.That(result.MessageId).IsEqualTo(expectedMessageId);
            await Assert.That(result.Payload).IsEqualTo(expectedPayload);
        }
    }

    [Test]
    [DisplayName($"{nameof(InteropEnvelopeProtocolTests)}.{nameof(Parse_TooLargeMessage_IsRejected)}")]
    public async Task Parse_TooLargeMessage_IsRejected() {
        string message = new('a', InteropEnvelopeProtocol.MaxMessageSizeBytes + 1);
        InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage(message);

        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.Error).Contains("exceeds max size");
    }
}
