// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;
using System.Text.Json;

namespace InfiniFrameTests.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InteropEnvelopeProtocolTests {
    private static async Task<JsonDocument> GetGoldenVectorsAsync(CancellationToken ct = default) => JsonDocument.Parse(
        await File.ReadAllTextAsync(
            Path.GetFullPath(
                Path.Join("Interop", "interop-envelope-golden-vectors.json"),
                AppContext.BaseDirectory
            ), ct)
    );
    
    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task CreateEnvelope_GoldenVectors(CancellationToken ct = default) {
        // Arrange
        JsonDocument goldenVectorsDocument = await GetGoldenVectorsAsync(ct);
        JsonElement vectors = goldenVectorsDocument.RootElement.GetProperty("createVectors");
        
        // Act & Assert
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
    public async Task ParseEnvelope_GoldenVectors(CancellationToken ct = default) {
        // Arrange
        JsonDocument goldenVectorsDocument = await GetGoldenVectorsAsync(ct);
        JsonElement vectors = goldenVectorsDocument.RootElement.GetProperty("parseVectors");
        
        // Act & Assert
        foreach (JsonElement vector in vectors.EnumerateArray()) {
            string message = vector.GetProperty("message").GetString()!;
            bool expectedSuccess = vector.GetProperty("success").GetBoolean();

            InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage(message);

            await Assert.That(result.IsSuccess).IsEqualTo(expectedSuccess);

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
    public async Task Parse_TooLargeMessage_IsRejected(CancellationToken ct = default) {
        // Arrange
        string message = new('a', InteropEnvelopeProtocol.MaxMessageSizeBytes + 1);

        // Act
        InteropEnvelopeParseResult result = InteropEnvelopeProtocol.ParseIncomingMessage(message);

        // Assert
        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Error).Contains("exceeds max size");
    }
}
