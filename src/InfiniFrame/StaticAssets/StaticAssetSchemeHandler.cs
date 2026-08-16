// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Utilities;
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
            return (file.CreateReadStream(), contentType);
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

    private static bool TryGetAssetPath(string rawPath, string defaultDocument, out string assetPath)
        => AssetPathResolver.TryGetAssetPath(rawPath, defaultDocument, out assetPath);

    private static string GetContentType(string path)
        => AssetPathResolver.GetContentType(path);
}