// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.StaticAssets;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace InfiniTests.InfiniFrame.StaticAssets;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class StaticAssetSchemeHandlerAdditionalTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Create handler - Content type resolution
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Handler_HtmlExtension_ReturnsHtmlContentType(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("index.html", "<html></html>"u8.ToArray());
        var handler = StaticAssetSchemeHandler.Create(provider, "index.html");

        // Act
        (Stream? data, string? contentType) = handler(null!, "index.html");

        // Assert
        await Assert.That(contentType).IsEqualTo("text/html; charset=utf-8");
        await data!.DisposeAsync();
    }

    [Test]
    public async Task Handler_CssExtension_ReturnsCssContentType(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("style.css", "body{}"u8.ToArray());
        var handler = StaticAssetSchemeHandler.Create(provider, "index.html");

        // Act
        (Stream? data, string? contentType) = handler(null!, "style.css");

        // Assert
        await Assert.That(contentType).IsEqualTo("text/css; charset=utf-8");
        await data!.DisposeAsync();
    }

    [Test]
    public async Task Handler_JsExtension_ReturnsJavaScriptContentType(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("app.js", "console.log()"u8.ToArray());
        var handler = StaticAssetSchemeHandler.Create(provider, "index.html");

        // Act
        (Stream? data, string? contentType) = handler(null!, "app.js");

        // Assert
        await Assert.That(contentType).IsEqualTo("application/javascript; charset=utf-8");
        await data!.DisposeAsync();
    }

    [Test]
    public async Task Handler_JsonExtension_ReturnsJsonContentType(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("data.json", "{}"u8.ToArray());
        var handler = StaticAssetSchemeHandler.Create(provider, "index.html");

        // Act
        (Stream? data, string? contentType) = handler(null!, "data.json");

        // Assert
        await Assert.That(contentType).IsEqualTo("application/json; charset=utf-8");
        await data!.DisposeAsync();
    }

    [Test]
    public async Task Handler_SvgExtension_ReturnsSvgContentType(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("icon.svg", "<svg/>"u8.ToArray());
        var handler = StaticAssetSchemeHandler.Create(provider, "index.html");

        // Act
        (Stream? data, string? contentType) = handler(null!, "icon.svg");

        // Assert
        await Assert.That(contentType).IsEqualTo("image/svg+xml");
        await data!.DisposeAsync();
    }

    [Test]
    public async Task Handler_PngExtension_ReturnsPngContentType(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("image.png", [0x89, 0x50, 0x4E, 0x47]);
        var handler = StaticAssetSchemeHandler.Create(provider, "index.html");

        // Act
        (Stream? data, string? contentType) = handler(null!, "image.png");

        // Assert
        await Assert.That(contentType).IsEqualTo("image/png");
        await data!.DisposeAsync();
    }

    [Test]
    public async Task Handler_JpgExtension_ReturnsJpegContentType(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("photo.jpg", [0xFF, 0xD8, 0xFF]);
        var handler = StaticAssetSchemeHandler.Create(provider, "index.html");

        // Act
        (Stream? data, string? contentType) = handler(null!, "photo.jpg");

        // Assert
        await Assert.That(contentType).IsEqualTo("image/jpeg");
        await data!.DisposeAsync();
    }

    [Test]
    public async Task Handler_JpegExtension_ReturnsJpegContentType(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("photo.jpeg", [0xFF, 0xD8, 0xFF]);
        var handler = StaticAssetSchemeHandler.Create(provider, "index.html");

        // Act
        (Stream? data, string? contentType) = handler(null!, "photo.jpeg");

        // Assert
        await Assert.That(contentType).IsEqualTo("image/jpeg");
        await data!.DisposeAsync();
    }

    [Test]
    public async Task Handler_GifExtension_ReturnsGifContentType(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("animation.gif", [0x47, 0x49, 0x46]);
        var handler = StaticAssetSchemeHandler.Create(provider, "index.html");

        // Act
        (Stream? data, string? contentType) = handler(null!, "animation.gif");

        // Assert
        await Assert.That(contentType).IsEqualTo("image/gif");
        await data!.DisposeAsync();
    }

    [Test]
    public async Task Handler_IcoExtension_ReturnsIconContentType(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("favicon.ico", [0x00, 0x00]);
        var handler = StaticAssetSchemeHandler.Create(provider, "index.html");

        // Act
        (Stream? data, string? contentType) = handler(null!, "favicon.ico");

        // Assert
        await Assert.That(contentType).IsEqualTo("image/x-icon");
        await data!.DisposeAsync();
    }

    [Test]
    public async Task Handler_WoffExtension_ReturnsWoffContentType(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("font.woff", [0x77, 0x4F, 0x46, 0x46]);
        var handler = StaticAssetSchemeHandler.Create(provider, "index.html");

        // Act
        (Stream? data, string? contentType) = handler(null!, "font.woff");

        // Assert
        await Assert.That(contentType).IsEqualTo("font/woff");
        await data!.DisposeAsync();
    }

    [Test]
    public async Task Handler_Woff2Extension_ReturnsWoff2ContentType(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("font.woff2", [0x77, 0x4F, 0x46, 0x32]);
        var handler = StaticAssetSchemeHandler.Create(provider, "index.html");

        // Act
        (Stream? data, string? contentType) = handler(null!, "font.woff2");

        // Assert
        await Assert.That(contentType).IsEqualTo("font/woff2");
        await data!.DisposeAsync();
    }

    [Test]
    public async Task Handler_TtfExtension_ReturnsTtfContentType(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("font.ttf", [0x00, 0x01, 0x00, 0x00]);
        var handler = StaticAssetSchemeHandler.Create(provider, "index.html");

        // Act
        (Stream? data, string? contentType) = handler(null!, "font.ttf");

        // Assert
        await Assert.That(contentType).IsEqualTo("font/ttf");
        await data!.DisposeAsync();
    }

    [Test]
    public async Task Handler_MapExtension_ReturnsJsonContentType(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("app.js.map", "{}"u8.ToArray());
        var handler = StaticAssetSchemeHandler.Create(provider, "index.html");

        // Act
        (Stream? data, string? contentType) = handler(null!, "app.js.map");

        // Assert
        await Assert.That(contentType).IsEqualTo("application/json; charset=utf-8");
        await data!.DisposeAsync();
    }

    [Test]
    public async Task Handler_WasmExtension_ReturnsWasmContentType(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("module.wasm", [0x00, 0x61, 0x73, 0x6D]);
        var handler = StaticAssetSchemeHandler.Create(provider, "index.html");

        // Act
        (Stream? data, string? contentType) = handler(null!, "module.wasm");

        // Assert
        await Assert.That(contentType).IsEqualTo("application/wasm");
        await data!.DisposeAsync();
    }

    [Test]
    public async Task Handler_Mp4Extension_ReturnsMp4ContentType(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("video.mp4", [0x00, 0x00]);
        var handler = StaticAssetSchemeHandler.Create(provider, "index.html");

        // Act
        (Stream? data, string? contentType) = handler(null!, "video.mp4");

        // Assert
        await Assert.That(contentType).IsEqualTo("video/mp4");
        await data!.DisposeAsync();
    }

    [Test]
    public async Task Handler_UnknownExtension_ReturnsOctetStreamContentType(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("file.xyz", [0x00, 0x01]);
        var handler = StaticAssetSchemeHandler.Create(provider, "index.html");

        // Act
        (Stream? data, string? contentType) = handler(null!, "file.xyz");

        // Assert
        await Assert.That(contentType).IsEqualTo("application/octet-stream");
        await data!.DisposeAsync();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Create handler - Path resolution
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Handler_EmptyPath_ReturnsDefaultDocument(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("index.html", "<html></html>"u8.ToArray());
        var handler = StaticAssetSchemeHandler.Create(provider, "index.html");

        // Act
        (Stream? data, string? contentType) = handler(null!, "");

        // Assert
        await Assert.That(data).IsNotNull();
        await data!.DisposeAsync();
    }

    [Test]
    public async Task Handler_NullPath_ReturnsDefaultDocument(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("index.html", "<html></html>"u8.ToArray());
        var handler = StaticAssetSchemeHandler.Create(provider, "index.html");

        // Act
        (Stream? data, string? contentType) = handler(null!, null!);

        // Assert
        await Assert.That(data).IsNotNull();
        await data!.DisposeAsync();
    }

    [Test]
    public async Task Handler_NonExistentFile_ReturnsDefault(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("index.html", "<html></html>"u8.ToArray());
        var handler = StaticAssetSchemeHandler.Create(provider, "index.html");

        // Act
        (Stream? data, string? contentType) = handler(null!, "nonexistent.txt");

        // Assert
        await Assert.That(data).IsNull();
    }

    [Test]
    public async Task Handler_PathWithTrailingSlash_AppendsDefaultDocument(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("subdir/index.html", "<html></html>"u8.ToArray());
        var handler = StaticAssetSchemeHandler.Create(provider, "index.html");

        // Act
        (Stream? data, string? contentType) = handler(null!, "subdir/");

        // Assert
        await Assert.That(data).IsNotNull();
        await data!.DisposeAsync();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Create handler - Path traversal blocking
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Handler_DoubleDotTraversal_ReturnsDefault(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("secret.txt", "secret"u8.ToArray());
        var handler = StaticAssetSchemeHandler.Create(provider, "index.html");

        // Act
        (Stream? data, string? contentType) = handler(null!, "../secret.txt");

        // Assert
        await Assert.That(data).IsNull();
    }

    [Test]
    public async Task Handler_PercentEncodedDoubleDot_ReturnsDefault(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("secret.txt", "secret"u8.ToArray());
        var handler = StaticAssetSchemeHandler.Create(provider, "index.html");

        // Act
        (Stream? data, string? contentType) = handler(null!, "%2e%2e/secret.txt");

        // Assert
        await Assert.That(data).IsNull();
    }

    [Test]
    public async Task Handler_PercentEncodedSlash_ReturnsDefault(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("secret.txt", "secret"u8.ToArray());
        var handler = StaticAssetSchemeHandler.Create(provider, "index.html");

        // Act
        (Stream? data, string? contentType) = handler(null!, "%2fsecret.txt");

        // Assert
        await Assert.That(data).IsNull();
    }

    [Test]
    public async Task Handler_DoubleEncodedTraversal_ReturnsDefault(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("secret.txt", "secret"u8.ToArray());
        var handler = StaticAssetSchemeHandler.Create(provider, "index.html");

        // Act
        (Stream? data, string? contentType) = handler(null!, "%252e%252e/secret.txt");

        // Assert
        await Assert.That(data).IsNull();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Create handler - Absolute URI path
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Handler_AbsoluteUri_ExtractsLocalPath(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("assets/data.txt", "content"u8.ToArray());
        var handler = StaticAssetSchemeHandler.Create(provider, "index.html");

        // Act
        (Stream? data, string? contentType) = handler(null!, "app://localhost/assets/data.txt");

        // Assert
        await Assert.That(data).IsNotNull();
        await data!.DisposeAsync();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // TryResolveUri
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task TryResolveUri_EmptyPath_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("index.html", "<html></html>"u8.ToArray());

        // Act
        bool resolved = StaticAssetSchemeHandler.TryResolveUri(
            provider, "", "app://localhost/", "index.html", out Uri uri);

        // Assert
        // Empty path becomes default document, which exists
        await Assert.That(resolved).IsTrue();
    }

    [Test]
    public async Task TryResolveUri_NonExistentFile_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("index.html", "<html></html>"u8.ToArray());

        // Act
        bool resolved = StaticAssetSchemeHandler.TryResolveUri(
            provider, "nonexistent.html", "app://localhost/", "index.html", out Uri uri);

        // Assert
        await Assert.That(resolved).IsFalse();
    }

    [Test]
    public async Task TryResolveUri_DirectoryPath_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        var provider = new DirectoryTestFileProvider();

        // Act
        bool resolved = StaticAssetSchemeHandler.TryResolveUri(
            provider, "subdir/", "app://localhost/", "index.html", out Uri uri);

        // Assert
        await Assert.That(resolved).IsFalse();
    }

    [Test]
    public async Task TryResolveUri_WithQueryString_PreservesQueryStringInUri(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("page.html", "<html></html>"u8.ToArray());

        // Act
        bool resolved = StaticAssetSchemeHandler.TryResolveUri(
            provider, "page.html?tab=1", "app://localhost/", "index.html", out Uri uri);

        // Assert
        await Assert.That(resolved).IsTrue();
        await Assert.That(uri.Query).IsEqualTo("?tab=1");
    }

    [Test]
    public async Task TryResolveUri_WithFragment_PreservesFragmentInUri(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider("page.html", "<html></html>"u8.ToArray());

        // Act
        bool resolved = StaticAssetSchemeHandler.TryResolveUri(
            provider, "page.html#section", "app://localhost/", "index.html", out Uri uri);

        // Assert
        await Assert.That(resolved).IsTrue();
        await Assert.That(uri.Fragment).IsEqualTo("#section");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------------------------------------------------
    private sealed class TestFileProvider(string expectedPath, byte[] content) : IFileProvider {
        public IFileInfo GetFileInfo(string subpath) {
            return string.Equals(subpath, expectedPath, StringComparison.Ordinal)
                ? new MemoryFileInfo(expectedPath, content)
                : new NotFoundFileInfo(subpath);
        }

        public IDirectoryContents GetDirectoryContents(string subpath) => NotFoundDirectoryContents.Singleton;
        public IChangeToken Watch(string filter) => NullChangeToken.Singleton;
    }

    private sealed class DirectoryTestFileProvider : IFileProvider {
        public IFileInfo GetFileInfo(string subpath) {
            if (subpath == "subdir/" || subpath == "subdir")
                return new TestDirFileInfo("subdir");
            return new NotFoundFileInfo(subpath);
        }

        public IDirectoryContents GetDirectoryContents(string subpath) => NotFoundDirectoryContents.Singleton;
        public IChangeToken Watch(string filter) => NullChangeToken.Singleton;
    }

    private sealed class TestDirFileInfo(string name) : IFileInfo {
        public bool Exists => true;
        public long Length => 0;
        public string? PhysicalPath => null;
        public string Name => name;
        public DateTimeOffset LastModified => DateTimeOffset.UnixEpoch;
        public bool IsDirectory => true;
        public Stream CreateReadStream() => new MemoryStream();
    }

    private sealed class MemoryFileInfo(string name, byte[] content) : IFileInfo {
        public bool Exists => true;
        public long Length => content.Length;
        public string? PhysicalPath => null;
        public string Name => name;
        public DateTimeOffset LastModified => DateTimeOffset.UnixEpoch;
        public bool IsDirectory => false;
        public Stream CreateReadStream() => new MemoryStream(content, writable: false);
    }
}
