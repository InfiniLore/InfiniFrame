// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Pure validation logic for custom scheme responses.
///     Extracted from <see cref="InfiniFrameEvents.CustomScheme"/> for testability.
/// </summary>
public static class CustomSchemeResponseValidator {

    /// <summary>
    ///     Validates and normalizes a content type string for custom scheme responses.
    /// </summary>
    /// <returns>The normalized content type.</returns>
    /// <exception cref="InvalidDataException">Thrown if the content type is invalid.</exception>
    public static string ValidateContentType(string? contentType) {
        string normalized = string.IsNullOrWhiteSpace(contentType)
            ? "application/octet-stream"
            : contentType;

        if (normalized.IndexOfAny(['\r', '\n', '\0', '\t']) >= 0)
            throw new InvalidDataException("Custom scheme content type contains invalid control characters.");

        byte[] contentTypeBytes = System.Text.Encoding.UTF8.GetBytes(normalized);
        if (contentTypeBytes.Length > 256)
            throw new InvalidDataException("Custom scheme content type exceeds the 256-byte limit.");

        return normalized;
    }

    /// <summary>
    ///     Validates that a response body length is within the allowed limit.
    /// </summary>
    /// <exception cref="InvalidDataException">Thrown if the body is too large.</exception>
    public static void ValidateBodyLength(long? bodyLength) {
        if (bodyLength is < 0 || (ulong)(bodyLength ?? 0) > 2 * 1024 * 1024)
            throw new InvalidDataException("Custom scheme response exceeds the 2MB limit.");
    }
}
