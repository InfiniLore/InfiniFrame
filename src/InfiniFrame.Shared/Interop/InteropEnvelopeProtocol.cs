// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InfiniFrame.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class InteropEnvelopeProtocol {
    internal const int CurrentVersion = 1;
    internal const int MaxMessageSizeBytes = 1024 * 1024;

    private static readonly JsonDocumentOptions JsonDocumentOptions = new() {
        AllowTrailingCommas = false,
        CommentHandling = JsonCommentHandling.Disallow,
        MaxDepth = 64
    };

    internal static string CreateEnvelopeMessage(string id, object? data = null) {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var envelope = new InteropEnvelopeV1 {
            Id = id,
            Data = data,
            Version = CurrentVersion
        };

        return JsonSerializer.Serialize(envelope);
    }

    internal static InteropEnvelopeParseResult ParseIncomingMessage(string message) {
        if (string.IsNullOrWhiteSpace(message))
            return InteropEnvelopeParseResult.CreateFailure("Message is empty.");

        int byteCount = Encoding.UTF8.GetByteCount(message);
        if (byteCount > MaxMessageSizeBytes)
            return InteropEnvelopeParseResult.CreateFailure($"Message exceeds max size of {MaxMessageSizeBytes} bytes.");

        if (!LooksLikeJsonObject(message))
            return InteropEnvelopeParseResult.CreateFailure("Message is not a valid JSON envelope.");

        try {
            using JsonDocument jsonDocument = JsonDocument.Parse(message, JsonDocumentOptions);
            JsonElement root = jsonDocument.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
                return InteropEnvelopeParseResult.CreateFailure("Envelope root must be a JSON object.");

            if (!root.TryGetProperty("id", out JsonElement idElement) || idElement.ValueKind != JsonValueKind.String)
                return InteropEnvelopeParseResult.CreateFailure("Envelope 'id' is required and must be a string.");

            string? messageId = idElement.GetString();
            if (string.IsNullOrWhiteSpace(messageId))
                return InteropEnvelopeParseResult.CreateFailure("Envelope 'id' cannot be empty.");

            if (!root.TryGetProperty("version", out JsonElement versionElement) || versionElement.ValueKind != JsonValueKind.Number)
                return InteropEnvelopeParseResult.CreateFailure("Envelope 'version' is required and must be a number.");

            if (!versionElement.TryGetInt32(out int version))
                return InteropEnvelopeParseResult.CreateFailure("Envelope 'version' must be a 32-bit integer.");

            if (version != CurrentVersion)
                return InteropEnvelopeParseResult.CreateFailure($"Unsupported envelope version '{version}'.");

            string? payload = null;
            if (root.TryGetProperty("data", out JsonElement dataElement))
                payload = ConvertDataToPayload(dataElement);

            return InteropEnvelopeParseResult.CreateSuccess(messageId, payload);
        }
        catch (JsonException) {
            return InteropEnvelopeParseResult.CreateFailure("Envelope JSON is malformed.");
        }
    }

    private static bool LooksLikeJsonObject(string message) {
        ReadOnlySpan<char> span = message.AsSpan().TrimStart();
        return !span.IsEmpty && span[0] == '{';
    }

    private static string? ConvertDataToPayload(JsonElement dataElement) {
        return dataElement.ValueKind switch {
            JsonValueKind.Null => null,
            JsonValueKind.String => dataElement.GetString(),
            _ => dataElement.GetRawText()
        };
    }

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    private sealed class InteropEnvelopeV1 {
        [JsonPropertyName("id")]
        public required string Id { get; init; }

        [JsonPropertyName("data")]
        public object? Data { get; init; }

        [JsonPropertyName("version")]
        public int Version { get; init; }
    }
}
