// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace InfiniFrame.StaticAssets;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class StaticAssetSchemeHandler {
    public static NetCustomSchemeDelegate Create(IFileProvider fileProvider, string defaultDocument) {
        #if NET10_0_OR_GREATER
        return (sender, scheme, url, out contentType) => {
        #else
        // yes, C# 14 and such have out parameters in their lambas, but we need to support .NET 8.0 which does not natively have this yet
        return NetCustomSchemeDelegateWrapper;

        Stream? NetCustomSchemeDelegateWrapper(IInfiniFrameWindow sender, string scheme, string url, out string? contentType) {
            #endif
            contentType = null;

            if (!TryGetAssetPath(url, defaultDocument, out string assetPath)) {
                sender.Logger.LogDebug("Rejected custom scheme path for {Scheme}: {Url}", scheme, url);
                return null;
            }

            IFileInfo file = fileProvider.GetFileInfo(assetPath);
            if (!file.Exists || file.IsDirectory) {
                sender.Logger.LogDebug("Custom scheme miss for {Scheme}: {AssetPath} (from {Url})", scheme, assetPath,
                    url);
                return null;
            }

            contentType = GetContentType(assetPath);
            sender.Logger.LogDebug("Custom scheme hit for {Scheme}: {AssetPath} ({ContentType})", scheme, assetPath,
                contentType);
            return file.CreateReadStream();

            #if NET10_0_OR_GREATER
        };
            #else
        }
        #endif
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
