// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.FileProviders;

namespace InfiniFrame.StaticAssets;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class StaticAssetSchemeHandler {
    private static readonly bool TraceEnabled =
        string.Equals(Environment.GetEnvironmentVariable("INFINIFRAME_TRACE_STATIC_ASSETS"), "1", StringComparison.Ordinal);

    public static NetCustomSchemeDelegate Create(IFileProvider fileProvider, string defaultDocument) {
        return (_, _, url, out contentType) => {
            contentType = null;

            if (!TryGetAssetPath(url, defaultDocument, out string assetPath)) {
                Trace($"Rejected URL path: {url}");
                return null;
            }

            IFileInfo file = fileProvider.GetFileInfo(assetPath);
            if (!file.Exists || file.IsDirectory) {
                Trace($"Miss: {assetPath} (from {url})");
                return null;
            }

            contentType = GetContentType(assetPath);
            Trace($"Hit: {assetPath} ({contentType})");
            return file.CreateReadStream();
        };
    }

    public static bool TryResolveUri(
        IFileProvider fileProvider,
        string path,
        string baseUri,
        string defaultDocument,
        out Uri uri
    ) {
        uri = null!;
        if (!TryGetAssetPath(path, defaultDocument, out string assetPath)) return false;

        IFileInfo file = fileProvider.GetFileInfo(assetPath);
        if (!file.Exists || file.IsDirectory) return false;

        uri = new Uri($"{baseUri}{assetPath}", UriKind.Absolute);
        return true;
    }

    private static bool TryGetAssetPath(string rawPath, string defaultDocument, out string assetPath) {
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
        if (assetPath.Contains("..", StringComparison.Ordinal)) return false;

        return true;
    }

    private static string GetContentType(string path) {
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
            _ => "application/octet-stream"
        };
    }

    private static void Trace(string message) {
        if (!TraceEnabled) return;

        try {
            string logPath = Path.Combine(Path.GetTempPath(), "infiniframe-static-assets.log");
            File.AppendAllText(logPath, $"[{DateTime.UtcNow:O}] {message}{Environment.NewLine}");
        }
        catch {
            // Never fail request handling due to tracing.
        }
    }
}
