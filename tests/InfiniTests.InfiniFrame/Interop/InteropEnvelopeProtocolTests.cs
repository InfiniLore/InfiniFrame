// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;
using System.Text.Json;

namespace InfiniTests.InfiniFrame.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InteropEnvelopeProtocolTests {
    private const string GoldenVectorsFileName = "interop-envelope-golden-vectors.json";

    private static async Task<JsonDocument> GetGoldenVectorsAsync(CancellationToken ct = default) {
        string path = ResolveGoldenVectorsPath();
        return JsonDocument.Parse(await File.ReadAllTextAsync(path, ct));
    }

    private static string ResolveGoldenVectorsPath() {
        string outputLinkedPath = Path.Combine(AppContext.BaseDirectory, "Interop", GoldenVectorsFileName);
        if (File.Exists(outputLinkedPath))
            return outputLinkedPath;

        DirectoryInfo? current = new(AppContext.BaseDirectory);
        while (current is not null) {
            string candidate = Path.Combine(
                current.FullName,
                "src",
                "InfiniFrame.Js",
                "TypeScript",
                "Interop",
                "EnvelopeProtocol",
                GoldenVectorsFileName
            );
            if (File.Exists(candidate))
                return candidate;

            current = current.Parent;
        }

        throw new FileNotFoundException($"Could not locate {GoldenVectorsFileName}.");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test, Retry(5)]
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

    [Test, Retry(5)]
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

    [Test, Retry(5)]
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
