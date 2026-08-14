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
public class InfiniFrameStaticAssetsTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task DeepCopy_ShouldReturnNewInstanceWithSameValues(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider();
        var assets = new InfiniFrameStaticAssets {
            FileProvider = provider,
            BaseUri = "app://localhost/",
            DefaultDocument = "index.html"
        };

        // Act
        var copy = assets.DeepCopy();

        // Assert
        await Assert.That(copy).IsNotSameReferenceAs(assets);
        await Assert.That(copy.FileProvider).IsSameReferenceAs(provider);
        await Assert.That(copy.BaseUri).IsEqualTo("app://localhost/");
        await Assert.That(copy.DefaultDocument).IsEqualTo("index.html");
    }

    [Test]
    public async Task Properties_ShouldBeSettable(CancellationToken ct = default) {
        // Arrange
        var provider = new TestFileProvider();

        // Act
        var assets = new InfiniFrameStaticAssets {
            FileProvider = provider,
            BaseUri = "custom://host/",
            DefaultDocument = "home.html"
        };

        // Assert
        await Assert.That(assets.FileProvider).IsSameReferenceAs(provider);
        await Assert.That(assets.BaseUri).IsEqualTo("custom://host/");
        await Assert.That(assets.DefaultDocument).IsEqualTo("home.html");
    }

    private sealed class TestFileProvider : IFileProvider {
        public IDirectoryContents GetDirectoryContents(string subpath) => new TestDirectoryContents();
        public IFileInfo GetFileInfo(string subpath) => new NotFoundFileInfo(subpath);
        public IChangeToken Watch(string filter) => NullChangeToken.Singleton;
    }

    private sealed class TestDirectoryContents : IDirectoryContents {
        public bool Exists => false;
        public IEnumerator<IFileInfo> GetEnumerator() => Enumerable.Empty<IFileInfo>().GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
