// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BlazorWebView.FileProviders;

namespace InfiniTests.InfiniFrame.Shared.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ScoredManifestCandidateTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Constructor
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Constructor_ShouldSetAllProperties(CancellationToken ct = default) {
        // Arrange
        var manifest = new StaticWebAssetManifest();

        // Act
        var candidate = new ScoredManifestCandidate(manifest, 10, "/path");

        // Assert
        await Assert.That(candidate.Manifest).IsSameReferenceAs(manifest);
        await Assert.That(candidate.Score).IsEqualTo(10);
        await Assert.That(candidate.ManifestPath).IsEqualTo("/path");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Record equality
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Equality_SameManifestAndScoreAndPath_ShouldBeEqual(CancellationToken ct = default) {
        // Arrange
        var manifest = new StaticWebAssetManifest();
        var candidate1 = new ScoredManifestCandidate(manifest, 10, "/path1");
        var candidate2 = new ScoredManifestCandidate(manifest, 10, "/path1");

        // Act & Assert
        await Assert.That(candidate1).IsEqualTo(candidate2);
    }

    [Test]
    public async Task Equality_DifferentScores_ShouldNotBeEqual(CancellationToken ct = default) {
        // Arrange
        var manifest = new StaticWebAssetManifest();
        var candidate1 = new ScoredManifestCandidate(manifest, 10, "/path1");
        var candidate2 = new ScoredManifestCandidate(manifest, 20, "/path1");

        // Act & Assert
        await Assert.That(candidate1).IsNotEqualTo(candidate2);
    }

    [Test]
    public async Task Equality_DifferentPaths_ShouldNotBeEqual(CancellationToken ct = default) {
        // Arrange
        var manifest = new StaticWebAssetManifest();
        var candidate1 = new ScoredManifestCandidate(manifest, 10, "/path1");
        var candidate2 = new ScoredManifestCandidate(manifest, 10, "/path2");

        // Act & Assert
        await Assert.That(candidate1).IsNotEqualTo(candidate2);
    }

    [Test]
    public async Task Equality_DifferentManifests_ShouldNotBeEqual(CancellationToken ct = default) {
        // Arrange
        var manifest1 = new StaticWebAssetManifest();
        var manifest2 = new StaticWebAssetManifest();
        var candidate1 = new ScoredManifestCandidate(manifest1, 10, "/path");
        var candidate2 = new ScoredManifestCandidate(manifest2, 10, "/path");

        // Act & Assert
        await Assert.That(candidate1).IsNotEqualTo(candidate2);
    }

    [Test]
    public async Task Equality_SameManifestReference_ShouldBeEqual(CancellationToken ct = default) {
        // Arrange
        var manifest = new StaticWebAssetManifest { ContentRoots = ["/a"] };
        var candidate1 = new ScoredManifestCandidate(manifest, 5, "/x");
        var candidate2 = new ScoredManifestCandidate(manifest, 5, "/x");

        // Act & Assert
        await Assert.That(candidate1).IsEqualTo(candidate2);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // With expression
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task WithExpression_ShouldCreateNewInstance(CancellationToken ct = default) {
        // Arrange
        var manifest = new StaticWebAssetManifest();
        var original = new ScoredManifestCandidate(manifest, 10, "/path");

        // Act
        ScoredManifestCandidate modified = original with { Score = 20 };

        // Assert
        await Assert.That(modified.Score).IsEqualTo(20);
        await Assert.That(modified.ManifestPath).IsEqualTo("/path");
        await Assert.That(modified.Manifest).IsSameReferenceAs(manifest);
        await Assert.That(modified).IsNotEqualTo(original);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Property access
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Manifest_ShouldBeAccessible(CancellationToken ct = default) {
        // Arrange
        var manifest = new StaticWebAssetManifest { ContentRoots = ["/root"] };

        // Act
        var candidate = new ScoredManifestCandidate(manifest, 10, "/path");

        // Assert
        await Assert.That(candidate.Manifest).IsSameReferenceAs(manifest);
        await Assert.That(candidate.Manifest.ContentRoots!.Length).IsEqualTo(1);
    }

    [Test]
    public async Task Score_CanBeZero(CancellationToken ct = default) {
        // Arrange & Act
        var candidate = new ScoredManifestCandidate(new StaticWebAssetManifest(), 0, "/path");

        // Assert
        await Assert.That(candidate.Score).IsEqualTo(0);
    }

    [Test]
    public async Task Score_CanBeNegative(CancellationToken ct = default) {
        // Arrange & Act
        var candidate = new ScoredManifestCandidate(new StaticWebAssetManifest(), -5, "/path");

        // Assert
        await Assert.That(candidate.Score).IsEqualTo(-5);
    }
}
