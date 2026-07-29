// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.FileProviders;

namespace InfiniFrame.StaticAssets;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class StaticAssetSchemeHandler {
    public static Func<IInfiniFrameWindow, string, (Stream? Data, string? ContentType)> Create(IFileProvider fileProvider, string defaultDocument) {
        return (_, url) => {
            if (!TryGetAssetPath(url, defaultDocument, out string assetPath)) {
                return default;
            }

            IFileInfo file = fileProvider.GetFileInfo(assetPath);
            if (!file.Exists || file.IsDirectory) {
                return default;
            }

            string contentType = GetContentType(assetPath);
            return ( file.CreateReadStream(), contentType);
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

        Uri resourceUri = new(new Uri(baseUri, UriKind.Absolute), assetPath);
        int suffixStart = path.IndexOfAny(['?', '#']);
        string navigationSuffix = suffixStart >= 0 ? path[suffixStart..] : string.Empty;
        uri = new Uri($"{resourceUri.AbsoluteUri}{navigationSuffix}", UriKind.Absolute);
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
        if (assetPath.Split('/', StringSplitOptions.RemoveEmptyEntries).Any(segment => segment == "..")) return false;

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
}
