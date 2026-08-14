// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BlazorWebView.FileProviders.Static;

namespace InfiniTests.InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class StaticWebAssetDataModelTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task StaticWebAsset_DefaultValues_ShouldBeCorrect(CancellationToken ct = default) {
        // Arrange

        // Act
        var asset = new StaticWebAsset();

        // Assert
        await Assert.That(asset.ContentRootIndex).IsEqualTo(0);
        await Assert.That(asset.SubPath).IsEqualTo(string.Empty);
    }

    [Test]
    public async Task StaticWebAsset_SetProperties_ShouldPersist(CancellationToken ct = default) {
        // Arrange

        // Act
        var asset = new StaticWebAsset { ContentRootIndex = 5, SubPath = "/test/path" };

        // Assert
        await Assert.That(asset.ContentRootIndex).IsEqualTo(5);
        await Assert.That(asset.SubPath).IsEqualTo("/test/path");
    }

    [Test]
    public async Task StaticWebAssetNode_DefaultValues_ShouldBeCorrect(CancellationToken ct = default) {
        // Arrange

        // Act
        var node = new StaticWebAssetNode();

        // Assert
        await Assert.That(node.Children).IsNull();
        await Assert.That(node.Asset).IsNull();
        await Assert.That(node.Patterns).IsNull();
    }

    [Test]
    public async Task StaticWebAssetPattern_DefaultValues_ShouldBeCorrect(CancellationToken ct = default) {
        // Arrange

        // Act
        var pattern = new StaticWebAssetPattern();

        // Assert
        await Assert.That(pattern.ContentRootIndex).IsEqualTo(0);
        await Assert.That(pattern.Pattern).IsEqualTo(string.Empty);
    }

    [Test]
    public async Task StaticWebAssetManifest_DefaultValues_ShouldBeCorrect(CancellationToken ct = default) {
        // Arrange

        // Act
        var manifest = new StaticWebAssetManifest();

        // Assert
        await Assert.That(manifest.ContentRoots).IsNull();
        await Assert.That(manifest.Root).IsNull();
    }

    [Test]
    public async Task ScoredManifestCandidate_RecordEquality_ShouldWork(CancellationToken ct = default) {
        // Arrange
        var manifest = new StaticWebAssetManifest();
        var candidate1 = new ScoredManifestCandidate(manifest, 10, "/path1");
        var candidate2 = new ScoredManifestCandidate(manifest, 10, "/path1");
        var candidate3 = new ScoredManifestCandidate(manifest, 20, "/path2");

        // Act & Assert
        await Assert.That(candidate1).IsEqualTo(candidate2);
        await Assert.That(candidate1).IsNotEqualTo(candidate3);
    }

    [Test]
    public async Task NodeTraversalState_RecordEquality_ShouldWork(CancellationToken ct = default) {
        // Arrange
        var node = new StaticWebAssetNode();
        var state1 = new NodeTraversalState(node, 3, "/prefix");
        var state2 = new NodeTraversalState(node, 3, "/prefix");
        var state3 = new NodeTraversalState(node, 5, "/other");

        // Act & Assert
        await Assert.That(state1).IsEqualTo(state2);
        await Assert.That(state1).IsNotEqualTo(state3);
    }

    [Test]
    public async Task ManifestCandidate_RecordEquality_ShouldWork(CancellationToken ct = default) {
        // Arrange
        var candidate1 = new ManifestCandidate("/path", 10);
        var candidate2 = new ManifestCandidate("/path", 10);
        var candidate3 = new ManifestCandidate("/other", 20);

        // Act & Assert
        await Assert.That(candidate1).IsEqualTo(candidate2);
        await Assert.That(candidate1).IsNotEqualTo(candidate3);
    }
}
