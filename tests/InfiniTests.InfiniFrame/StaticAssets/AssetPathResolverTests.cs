// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Utilities;

namespace InfiniTests.InfiniFrame.StaticAssets;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class AssetPathResolverTests {

    // -----------------------------------------------------------------------------------------------------------------
    // TryGetAssetPath
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task TryGetAssetPath_EmptyInput_ReturnsDefaultDocument(CancellationToken ct = default) {
        bool result = AssetPathResolver.TryGetAssetPath("", "index.html", out string path);
        await Assert.That(result).IsTrue();
        await Assert.That(path).IsEqualTo("index.html");
    }

    [Test]
    public async Task TryGetAssetPath_NullInput_ReturnsDefaultDocument(CancellationToken ct = default) {
        bool result = AssetPathResolver.TryGetAssetPath(null!, "default.html", out string path);
        await Assert.That(result).IsTrue();
        await Assert.That(path).IsEqualTo("default.html");
    }

    [Test]
    public async Task TryGetAssetPath_SimplePath_ReturnsTrimmedPath(CancellationToken ct = default) {
        bool result = AssetPathResolver.TryGetAssetPath("/css/style.css", "index.html", out string path);
        await Assert.That(result).IsTrue();
        await Assert.That(path).IsEqualTo("css/style.css");
    }

    [Test]
    public async Task TryGetAssetPath_WithQueryString_StripsQuery(CancellationToken ct = default) {
        bool result = AssetPathResolver.TryGetAssetPath("/page.html?v=2#section", "index.html", out string path);
        await Assert.That(result).IsTrue();
        await Assert.That(path).IsEqualTo("page.html");
    }

    [Test]
    public async Task TryGetAssetPath_TrailingSlash_AppendsDefaultDocument(CancellationToken ct = default) {
        bool result = AssetPathResolver.TryGetAssetPath("/assets/", "index.html", out string path);
        await Assert.That(result).IsTrue();
        await Assert.That(path).IsEqualTo("assets/index.html");
    }

    [Test]
    public async Task TryGetAssetPath_Backslashes_NormalizesToForwardSlash(CancellationToken ct = default) {
        bool result = AssetPathResolver.TryGetAssetPath("/css\\style.css", "index.html", out string path);
        await Assert.That(result).IsTrue();
        await Assert.That(path).IsEqualTo("css/style.css");
    }

    [Test]
    public async Task TryGetAssetPath_PercentEncoded_PreservesPath(CancellationToken ct = default) {
        // Uri.UnescapeDataString does not decode %20 (space) since it's valid URI encoding
        bool result = AssetPathResolver.TryGetAssetPath("/path%20with%20spaces/file.html", "index.html", out string path);
        await Assert.That(result).IsTrue();
        await Assert.That(path).IsEqualTo("path%20with%20spaces/file.html");
    }

    [Test]
    public async Task TryGetAssetPath_PercentEncodedSlash_BlockedAsTraversal(CancellationToken ct = default) {
        // %2F (encoded slash) is blocked by the security check
        bool result = AssetPathResolver.TryGetAssetPath("/path%2Ffile.html", "index.html", out _);
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryGetAssetPath_DoubleEncoded_BlockedAsTraversal(CancellationToken ct = default) {
        // %252e decodes to %2e which the raw traversal check catches
        bool result = AssetPathResolver.TryGetAssetPath("/%252e%252e/secret", "index.html", out _);
        await Assert.That(result).IsFalse();
    }

    [Test]
    [Arguments("..")]
    [Arguments("/../etc/passwd")]
    [Arguments("/path/../../secret")]
    [Arguments("/%2e%2e/secret")]
    [Arguments("/%2e%2e/")]
    [Arguments("/%252e%252e/secret")]
    [Arguments("/path%2f..%2fsecret")]
    [Arguments("/path%5c..%5csecret")]
    public async Task TryGetAssetPath_DirectoryTraversal_ReturnsFalse(string path, CancellationToken ct) {
        bool result = AssetPathResolver.TryGetAssetPath(path, "index.html", out _);
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryGetAssetPath_JustSlash_ReturnsDefaultDocument(CancellationToken ct = default) {
        bool result = AssetPathResolver.TryGetAssetPath("/", "index.html", out string path);
        await Assert.That(result).IsTrue();
        await Assert.That(path).IsEqualTo("index.html");
    }

    [Test]
    public async Task TryGetAssetPath_AbsoluteUri_ExtractsLocalPath(CancellationToken ct = default) {
        bool result = AssetPathResolver.TryGetAssetPath("https://example.com/page.html", "index.html", out string path);
        await Assert.That(result).IsTrue();
        await Assert.That(path).Contains("page.html");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // GetContentType
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [Arguments(".html", "text/html; charset=utf-8")]
    [Arguments(".htm", "text/html; charset=utf-8")]
    [Arguments(".css", "text/css; charset=utf-8")]
    [Arguments(".js", "application/javascript; charset=utf-8")]
    [Arguments(".mjs", "application/javascript; charset=utf-8")]
    [Arguments(".json", "application/json; charset=utf-8")]
    [Arguments(".svg", "image/svg+xml")]
    [Arguments(".png", "image/png")]
    [Arguments(".jpg", "image/jpeg")]
    [Arguments(".jpeg", "image/jpeg")]
    [Arguments(".gif", "image/gif")]
    [Arguments(".ico", "image/x-icon")]
    [Arguments(".woff", "font/woff")]
    [Arguments(".woff2", "font/woff2")]
    [Arguments(".ttf", "font/ttf")]
    [Arguments(".map", "application/json; charset=utf-8")]
    [Arguments(".wasm", "application/wasm")]
    [Arguments(".mp4", "video/mp4")]
    [Arguments(".webm", "video/webm")]
    [Arguments(".webp", "image/webp")]
    [Arguments(".avif", "image/avif")]
    [Arguments(".unknown", "application/octet-stream")]
    [Arguments(".xyz", "application/octet-stream")]
    public async Task GetContentType_ReturnsCorrectMimeType(string extension, string expected, CancellationToken ct) {
        string result = AssetPathResolver.GetContentType($"file{extension}");
        await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    public async Task GetContentType_CaseInsensitive(CancellationToken ct = default) {
        string result = AssetPathResolver.GetContentType("FILE.HTML");
        await Assert.That(result).IsEqualTo("text/html; charset=utf-8");
    }
}
