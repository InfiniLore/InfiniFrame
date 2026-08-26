// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections;
using InfiniFrame.BlazorWebView.FileProviders;
using Microsoft.Extensions.FileProviders;

namespace InfiniTests.InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ManifestDirectoryFileInfoTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Properties_ShouldReturnDirectoryDefaults(CancellationToken ct = default) {
        // Arrange

        // Act
        var info = new ManifestDirectoryFileInfo("test-dir");

        // Assert
        await Assert.That(info.Exists).IsTrue();
        await Assert.That(info.Length).IsEqualTo(-1);
        await Assert.That(info.PhysicalPath).IsEqualTo(string.Empty);
        await Assert.That(info.Name).IsEqualTo("test-dir");
        await Assert.That(info.LastModified).IsEqualTo(DateTimeOffset.MinValue);
        await Assert.That(info.IsDirectory).IsTrue();
    }

    [Test]
    public async Task CreateReadStream_ShouldThrowInvalidOperationException(CancellationToken ct = default) {
        // Arrange
        var info = new ManifestDirectoryFileInfo("test-dir");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => Task.Run(() => {
            info.CreateReadStream();
        }));
    }
}

public class ManifestDirectoryContentsTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Exists_ShouldAlwaysReturnTrue(CancellationToken ct = default) {
        // Arrange
        // ReSharper disable once CollectionNeverUpdated.Local
        var entries = new List<IFileInfo>();

        // Act
        var contents = new ManifestDirectoryContents(entries);

        // Assert
        await Assert.That(contents.Exists).IsTrue();
    }

    [Test]
    public async Task GetEnumerator_WithEmptyEntries_ShouldReturnEmptyEnumerator(CancellationToken ct = default) {
        // Arrange
        // ReSharper disable once CollectionNeverUpdated.Local
        var entries = new List<IFileInfo>();

        // Act
        var contents = new ManifestDirectoryContents(entries);
        IEnumerator<IFileInfo> enumerator = contents.GetEnumerator();
        using IDisposable enumerator1 = enumerator;

        // Assert
        await Assert.That(enumerator.MoveNext()).IsFalse();
    }

    [Test]
    public async Task GetEnumerator_WithEntries_ShouldEnumerateAll(CancellationToken ct = default) {
        // Arrange
        var file1 = new ManifestDirectoryFileInfo("file1");
        var file2 = new ManifestDirectoryFileInfo("file2");
        var entries = new List<IFileInfo> { file1, file2 };

        // Act
        var contents = new ManifestDirectoryContents(entries);
        List<IFileInfo> result = contents.ToList();

        // Assert
        await Assert.That(result.Count).IsEqualTo(2);
        await Assert.That(result[0].Name).IsEqualTo("file1");
        await Assert.That(result[1].Name).IsEqualTo("file2");
    }

    [Test]
    public async Task NonGenericGetEnumerator_ShouldReturnSameResults(CancellationToken ct = default) {
        // Arrange
        var file1 = new ManifestDirectoryFileInfo("file1");
        var entries = new List<IFileInfo> { file1 };
        var contents = new ManifestDirectoryContents(entries);

        // Act
        IEnumerator enumerator = ((IEnumerable)contents).GetEnumerator();
        using var enumerator1 = enumerator as IDisposable;
        bool moved = enumerator.MoveNext();
        object? current = enumerator.Current;

        // Assert
        await Assert.That(moved).IsTrue();
        await Assert.That(current).IsNotNull();
    }
}
