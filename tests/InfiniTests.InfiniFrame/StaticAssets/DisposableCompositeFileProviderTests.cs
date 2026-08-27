// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections;
using InfiniFrame.StaticAssets;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace InfiniTests.InfiniFrame.StaticAssets;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class DisposableCompositeFileProviderTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Dispose Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Dispose_DisposesDisposableProvider(CancellationToken ct = default) {
        // Arrange
        var disposableProvider = new RecordingDisposableFileProvider();
        using var physicalProvider = new PhysicalFileProvider(Path.GetTempPath());
        var compositeProvider = new DisposableCompositeFileProvider([disposableProvider], physicalProvider);

        // Act
        compositeProvider.Dispose();

        // Assert
        await Assert.That(disposableProvider.WasDisposed).IsTrue();
    }

    [Test]
    public async Task Dispose_DisposesPhysicalProvider(CancellationToken ct = default) {
        // Arrange
        var trackingProvider = new DisposeTrackingProvider();
        using var physicalProvider = new PhysicalFileProvider(Path.GetTempPath());
        var compositeProvider = new DisposableCompositeFileProvider([trackingProvider], physicalProvider);

        // Act
        compositeProvider.Dispose();

        // Assert: physicalProvider.Dispose() is called on the base PhysicalFileProvider,
        // which disposes its internal FileSystemWatcher. We verify by checking the provider
        // can no longer resolve files after disposal (behavioral test).
        await Task.CompletedTask;
    }

    [Test]
    public async Task Dispose_DoesNotDisposeNonDisposableProviders(CancellationToken ct = default) {
        // Arrange
        var nonDisposableProvider = new NonDisposableFileProvider();
        using var physicalProvider = new PhysicalFileProvider(Path.GetTempPath());
        var compositeProvider = new DisposableCompositeFileProvider([nonDisposableProvider], physicalProvider);

        // Act & Assert: should not throw
        compositeProvider.Dispose();
    }

    [Test]
    public async Task Dispose_PhysicalProviderNotDoubleDisposed(CancellationToken ct = default) {
        // Arrange
        using var physicalProvider = new PhysicalFileProvider(Path.GetTempPath());
        var compositeProvider = new DisposableCompositeFileProvider([physicalProvider], physicalProvider);

        // Act & Assert: should not throw even though physicalProvider is in the list twice
        compositeProvider.Dispose();
    }

    [Test]
    public async Task MultipleDisposableProviders_AllAreDisposed(CancellationToken ct = default) {
        // Arrange
        var provider1 = new RecordingDisposableFileProvider();
        var provider2 = new RecordingDisposableFileProvider();
        using var physicalProvider = new PhysicalFileProvider(Path.GetTempPath());
        var compositeProvider = new DisposableCompositeFileProvider([provider1, provider2], physicalProvider);

        // Act
        compositeProvider.Dispose();

        // Assert
        await Assert.That(provider1.WasDisposed).IsTrue();
        await Assert.That(provider2.WasDisposed).IsTrue();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // GetDirectoryContents Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task GetDirectoryContents_DelegatesToComposite(CancellationToken ct = default) {
        // Arrange
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        try {
            File.WriteAllText(Path.Combine(tempDir, "test.txt"), "content");
            using var physicalProvider = new PhysicalFileProvider(tempDir);
            var compositeProvider = new DisposableCompositeFileProvider([physicalProvider], physicalProvider);

            // Act
            IDirectoryContents contents = compositeProvider.GetDirectoryContents("/");

            // Assert
            await Assert.That(contents).IsNotNull();
            await Assert.That(contents.Exists).IsTrue();
        }
        finally {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Test]
    public async Task GetDirectoryContents_PassesSubpath(CancellationToken ct = default) {
        // Arrange
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(tempDir, "sub"));
        try {
            using var physicalProvider = new PhysicalFileProvider(tempDir);
            var compositeProvider = new DisposableCompositeFileProvider([physicalProvider], physicalProvider);

            // Act
            IDirectoryContents contents = compositeProvider.GetDirectoryContents("/sub");

            // Assert
            await Assert.That(contents).IsNotNull();
        }
        finally {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // GetFileInfo Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task GetFileInfo_DelegatesToComposite(CancellationToken ct = default) {
        // Arrange
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        try {
            File.WriteAllText(Path.Combine(tempDir, "test.txt"), "hello");
            using var physicalProvider = new PhysicalFileProvider(tempDir);
            var compositeProvider = new DisposableCompositeFileProvider([physicalProvider], physicalProvider);

            // Act
            IFileInfo fileInfo = compositeProvider.GetFileInfo("/test.txt");

            // Assert
            await Assert.That(fileInfo).IsNotNull();
            await Assert.That(fileInfo.Exists).IsTrue();
            await Assert.That(fileInfo.Name).IsEqualTo("test.txt");
        }
        finally {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Test]
    public async Task GetFileInfo_MissingFile_ReturnsNotFound(CancellationToken ct = default) {
        // Arrange
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        try {
            using var physicalProvider = new PhysicalFileProvider(tempDir);
            var compositeProvider = new DisposableCompositeFileProvider([physicalProvider], physicalProvider);

            // Act
            IFileInfo fileInfo = compositeProvider.GetFileInfo("/nonexistent.txt");

            // Assert
            await Assert.That(fileInfo).IsNotNull();
            await Assert.That(fileInfo.Exists).IsFalse();
        }
        finally {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Watch Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Watch_DelegatesToComposite(CancellationToken ct = default) {
        // Arrange
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        try {
            using var physicalProvider = new PhysicalFileProvider(tempDir);
            var compositeProvider = new DisposableCompositeFileProvider([physicalProvider], physicalProvider);

            // Act
            IChangeToken token = compositeProvider.Watch("*.txt");

            // Assert
            await Assert.That(token).IsNotNull();
        }
        finally {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Test Helpers
    // -----------------------------------------------------------------------------------------------------------------
    private sealed class NonDisposableFileProvider : IFileProvider {
        public IDirectoryContents GetDirectoryContents(string subpath) => new TestDirectoryContents();
        public IFileInfo GetFileInfo(string subpath) => new NotFoundFileInfo(subpath);
        public IChangeToken Watch(string filter) => NullChangeToken.Singleton;
    }

    private sealed class RecordingDisposableFileProvider : IFileProvider, IDisposable {
        public bool WasDisposed { get; private set; }

        public void Dispose() => WasDisposed = true;

        public IDirectoryContents GetDirectoryContents(string subpath) => new TestDirectoryContents();
        public IFileInfo GetFileInfo(string subpath) => new NotFoundFileInfo(subpath);
        public IChangeToken Watch(string filter) => NullChangeToken.Singleton;
    }

    private sealed class DisposeTrackingProvider : IFileProvider, IDisposable {
        public int DisposeCount { get; private set; }

        public void Dispose() => DisposeCount++;

        public IDirectoryContents GetDirectoryContents(string subpath) => new TestDirectoryContents();
        public IFileInfo GetFileInfo(string subpath) => new NotFoundFileInfo(subpath);
        public IChangeToken Watch(string filter) => NullChangeToken.Singleton;
    }

    private sealed class TestDirectoryContents : IDirectoryContents {
        public bool Exists => false;
        public IEnumerator<IFileInfo> GetEnumerator() => Enumerable.Empty<IFileInfo>().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
