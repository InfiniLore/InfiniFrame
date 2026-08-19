// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Pure logic for resolving asset paths from URLs and determining MIME types.
///     Extracted from <see cref="InfiniFrame.StaticAssets.StaticAssetSchemeHandler"/> for testability.
/// </summary>
public static class AssetPathResolver {

    /// <summary>
    ///     Sanitizes a raw URL path into a safe asset path, blocking directory traversal.
    /// </summary>
    public static bool TryGetAssetPath(string rawPath, string defaultDocument, out string assetPath) {
        assetPath = rawPath;
        if (string.IsNullOrWhiteSpace(assetPath)) {
            assetPath = defaultDocument;
            return true;
        }

        int queryStart = assetPath.IndexOfAny(['?', '#']);
        if (queryStart >= 0) assetPath = assetPath[..queryStart];

        if (Uri.TryCreate(assetPath, UriKind.Absolute, out Uri? uri)) {
            assetPath = uri.LocalPath;
        }

        assetPath = assetPath.TrimStart('/');
        if (string.IsNullOrWhiteSpace(assetPath)) assetPath = defaultDocument;
        if (assetPath.EndsWith('/')) assetPath += defaultDocument;

        assetPath = assetPath.Replace('\\', '/');

        // Decode percent-encoded sequences iteratively to catch double/triple encoding.
        string decoded = assetPath;
        for (int i = 0; i < 3; i++) {
            string prev = decoded;
            decoded = Uri.UnescapeDataString(decoded);
            if (string.Equals(decoded, prev, StringComparison.Ordinal))
                break;
        }

        if (decoded.Contains("..", StringComparison.Ordinal))
            return false;

        // Block raw traversal sequences that bypass Uri.UnescapeDataString.
        if (assetPath.Contains("..", StringComparison.Ordinal))
            return false;

        // Block percent-encoded traversal sequences.
        string lowerPath = assetPath.ToLowerInvariant();
        if (lowerPath.Contains("%2e") || lowerPath.Contains("%2f") || lowerPath.Contains("%5c")
            || lowerPath.Contains("%252e") || lowerPath.Contains("%252f") || lowerPath.Contains("%255c"))
            return false;

        // Block Unicode dot-like characters that could bypass the ".." check.
        if (decoded.Contains('\u00B7') || decoded.Contains('\u2024') || decoded.Contains('\u2219'))
            return false;

        return true;
    }

    /// <summary>
    ///     Maps a file extension to its MIME content type.
    /// </summary>
    public static string GetContentType(string path) {
        string extension = Path.GetExtension(path).ToLowerInvariant();

        return extension switch {
            ".html" or ".htm" => "text/html; charset=utf-8",
            ".css" => "text/css; charset=utf-8",
            ".js" or ".mjs" => "application/javascript; charset=utf-8",
            ".json" => "application/json; charset=utf-8",
            ".svg" => "image/svg+xml",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".ico" => "image/x-icon",
            ".woff" => "font/woff",
            ".woff2" => "font/woff2",
            ".ttf" => "font/ttf",
            ".map" => "application/json; charset=utf-8",
            ".wasm" => "application/wasm",
            ".mp4" => "video/mp4",
            ".webm" => "video/webm",
            ".webp" => "image/webp",
            ".avif" => "image/avif",
            _ => "application/octet-stream"
        };
    }
}
