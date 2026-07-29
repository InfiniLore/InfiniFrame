// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.StaticAssets;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace InfiniTests.InfiniFrame.StaticAssets;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class StaticAssetSchemeHandlerTests {
    [Test]
    public async Task TryResolveUri_FragmentIsPreservedButExcludedFromLookup(CancellationToken ct = default) {
        var provider = new RecordingFileProvider("index.html", "<html></html>"u8.ToArray());

        bool resolved = StaticAssetSchemeHandler.TryResolveUri(
            provider, "index.html#settings", "app://localhost/", "index.html", out Uri uri);

        await Assert.That(resolved).IsTrue();
        await Assert.That(provider.LastSubpath).IsEqualTo("index.html");
        await Assert.That(uri.AbsoluteUri).IsEqualTo("app://localhost/index.html#settings");
    }

    [Test]
    public async Task Handler_QueryAndFragmentAreExcludedOnlyFromResourceLookup(CancellationToken ct = default) {
        byte[] expected = "fragment-safe"u8.ToArray();
        var provider = new RecordingFileProvider("assets/data.txt", expected);
        var handler = StaticAssetSchemeHandler.Create(provider, "index.html");

        (Stream? data, string? contentType) = handler(null!, "app://localhost/assets/data.txt?version=7#section");
        await using (data) {
            using var buffer = new MemoryStream();
            await data!.CopyToAsync(buffer, ct);
            await Assert.That(buffer.ToArray()).IsEquivalentTo(expected);
        }

        await Assert.That(provider.LastSubpath).IsEqualTo("assets/data.txt");
        await Assert.That(contentType).IsEqualTo("application/octet-stream");
    }

    private sealed class RecordingFileProvider(string expectedPath, byte[] content) : IFileProvider {
        public string? LastSubpath { get; private set; }

        public IFileInfo GetFileInfo(string subpath) {
            LastSubpath = subpath;
            return string.Equals(subpath, expectedPath, StringComparison.Ordinal)
                ? new MemoryFileInfo(expectedPath, content)
                : new NotFoundFileInfo(subpath);
        }

        public IDirectoryContents GetDirectoryContents(string subpath) => NotFoundDirectoryContents.Singleton;
        public IChangeToken Watch(string filter) => NullChangeToken.Singleton;
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
