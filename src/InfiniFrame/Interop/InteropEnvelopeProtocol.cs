// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text;
using System.Text.Json;

namespace InfiniFrame.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class InteropEnvelopeProtocol {
    private const int CurrentVersion = 2;
    internal const int MaxMessageSizeBytes = 1024 * 1024;
    internal const string PostCommand = "Post";
    internal const string GetCommand = "Get";

    private static readonly JsonDocumentOptions JsonDocumentOptions = new() {
        AllowTrailingCommas = false,
        CommentHandling = JsonCommentHandling.Disallow,
        MaxDepth = 64
    };

    internal static string CreateEnvelopeMessage(string id, string? data = null, string command = PostCommand, string? requestId = null) {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(command);

        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);

        writer.WriteStartObject();
        writer.WriteString("id", id);
        writer.WriteString("command", command);
        if (!string.IsNullOrWhiteSpace(requestId))
            writer.WriteString("requestId", requestId);
        if (data is null)
            writer.WriteNull("data");
        else
            writer.WriteString("data", data);
        writer.WriteNumber("version", CurrentVersion);
        writer.WriteEndObject();
        writer.Flush();

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    internal static InteropEnvelopeParseResult ParseIncomingMessage(string message) {
        if (string.IsNullOrWhiteSpace(message)) return InteropEnvelopeParseResult.CreateFailure("Message is empty.");
        if (message.StartsWith("__bwv:", StringComparison.Ordinal)) return InteropEnvelopeParseResult.BlazorMessage;

        message = TryUnwrapJsonEncodedString(message);

        int byteCount = Encoding.UTF8.GetByteCount(message);
        if (byteCount > MaxMessageSizeBytes)
            return InteropEnvelopeParseResult.CreateFailure($"Message exceeds max size of {MaxMessageSizeBytes} bytes.");

        if (!LooksLikeJsonObject(message))
            return InteropEnvelopeParseResult.CreateFailure("Envelope root must be a JSON object.");

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

            if (!root.TryGetProperty("command", out JsonElement commandElement) || commandElement.ValueKind != JsonValueKind.String)
                return InteropEnvelopeParseResult.CreateFailure("Envelope 'command' is required and must be a string.");

            string? command = commandElement.GetString();
            if (!IsSupportedCommand(command))
                return InteropEnvelopeParseResult.CreateFailure("Envelope 'command' must be 'Post' or 'Get'.");

            string? payload = null;
            if (root.TryGetProperty("data", out JsonElement dataElement))
                payload = ConvertDataToPayload(dataElement);

            string? requestId = null;
            if (root.TryGetProperty("requestId", out JsonElement requestIdElement)) {
                if (requestIdElement.ValueKind != JsonValueKind.String)
                    return InteropEnvelopeParseResult.CreateFailure("Envelope 'requestId' must be a string.");

                requestId = requestIdElement.GetString();
            }

            return InteropEnvelopeParseResult.CreateSuccess(messageId, payload, command, requestId);
        }
        catch (JsonException) {
            return InteropEnvelopeParseResult.CreateFailure("Envelope JSON is malformed.");
        }
    }

    private static string TryUnwrapJsonEncodedString(string message) {
        ReadOnlySpan<char> trimmed = message.AsSpan().Trim();
        if (trimmed.Length < 2 || trimmed[0] != '"' || trimmed[^1] != '"')
            return message;

        try {
            using JsonDocument jsonDocument = JsonDocument.Parse(trimmed.ToString(), JsonDocumentOptions);
            if (jsonDocument.RootElement.ValueKind != JsonValueKind.String) return message;

            string? unwrapped = jsonDocument.RootElement.GetString();
            return string.IsNullOrWhiteSpace(unwrapped) ? message : unwrapped;
        }
        catch (JsonException) {
            return message;
        }
    }

    private static bool IsSupportedCommand(string? command)
        => string.Equals(command, PostCommand, StringComparison.Ordinal)
           || string.Equals(command, GetCommand, StringComparison.Ordinal);

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
}
