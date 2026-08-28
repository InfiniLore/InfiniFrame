// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BlazorWebView.FileProviders;

namespace InfiniTests.InfiniFrame.Shared.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ManifestCandidateTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Constructor
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Constructor_ShouldSetRequiredProperties(CancellationToken ct = default) {
        // Arrange & Act
        var candidate = new ManifestCandidate("/manifest.json", 10);

        // Assert
        await Assert.That(candidate.ManifestPath).IsEqualTo("/manifest.json");
        await Assert.That(candidate.BaseScore).IsEqualTo(10);
        await Assert.That(candidate.ResourceStream).IsNull();
    }

    [Test]
    public async Task Constructor_WithOptionalStream_ShouldSetAllProperties(CancellationToken ct = default) {
        // Arrange
        using var stream = new MemoryStream();

        // Act
        var candidate = new ManifestCandidate("/path", 5, stream);

        // Assert
        await Assert.That(candidate.ManifestPath).IsEqualTo("/path");
        await Assert.That(candidate.BaseScore).IsEqualTo(5);
        await Assert.That(candidate.ResourceStream).IsSameReferenceAs(stream);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Record equality
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Equality_SamePathAndScore_ShouldBeEqual(CancellationToken ct = default) {
        // Arrange
        var candidate1 = new ManifestCandidate("/path", 10);
        var candidate2 = new ManifestCandidate("/path", 10);

        // Act & Assert
        await Assert.That(candidate1).IsEqualTo(candidate2);
    }

    [Test]
    public async Task Equality_DifferentPaths_ShouldNotBeEqual(CancellationToken ct = default) {
        // Arrange
        var candidate1 = new ManifestCandidate("/path1", 10);
        var candidate2 = new ManifestCandidate("/path2", 10);

        // Act & Assert
        await Assert.That(candidate1).IsNotEqualTo(candidate2);
    }

    [Test]
    public async Task Equality_DifferentScores_ShouldNotBeEqual(CancellationToken ct = default) {
        // Arrange
        var candidate1 = new ManifestCandidate("/path", 10);
        var candidate2 = new ManifestCandidate("/path", 20);

        // Act & Assert
        await Assert.That(candidate1).IsNotEqualTo(candidate2);
    }

    [Test]
    public async Task Equality_DifferentStreams_ShouldNotBeEqual(CancellationToken ct = default) {
        // Arrange
        using var stream1 = new MemoryStream();
        using var stream2 = new MemoryStream();
        var candidate1 = new ManifestCandidate("/path", 10, stream1);
        var candidate2 = new ManifestCandidate("/path", 10, stream2);

        // Act & Assert
        await Assert.That(candidate1).IsNotEqualTo(candidate2);
    }

    [Test]
    public async Task Equality_BothStreamsNull_ShouldBeEqual(CancellationToken ct = default) {
        // Arrange
        var candidate1 = new ManifestCandidate("/path", 10);
        var candidate2 = new ManifestCandidate("/path", 10);

        // Act & Assert
        await Assert.That(candidate1).IsEqualTo(candidate2);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // With expression
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task WithExpression_ShouldCreateNewInstance(CancellationToken ct = default) {
        // Arrange
        var original = new ManifestCandidate("/path", 10);

        // Act
        ManifestCandidate modified = original with { BaseScore = 20 };

        // Assert
        await Assert.That(modified.BaseScore).IsEqualTo(20);
        await Assert.That(modified.ManifestPath).IsEqualTo("/path");
        await Assert.That(modified.ResourceStream).IsNull();
        await Assert.That(modified).IsNotEqualTo(original);
    }

    [Test]
    public async Task WithExpression_ChangePath_ShouldCreateNewInstance(CancellationToken ct = default) {
        // Arrange
        var original = new ManifestCandidate("/old", 5);

        // Act
        ManifestCandidate modified = original with { ManifestPath = "/new" };

        // Assert
        await Assert.That(modified.ManifestPath).IsEqualTo("/new");
        await Assert.That(modified.BaseScore).IsEqualTo(5);
    }

    [Test]
    public async Task WithExpression_WithStream_ShouldPreserveStream(CancellationToken ct = default) {
        // Arrange
        using var stream = new MemoryStream();
        var original = new ManifestCandidate("/path", 10, stream);

        // Act
        ManifestCandidate modified = original with { BaseScore = 99 };

        // Assert
        await Assert.That(modified.ResourceStream).IsSameReferenceAs(stream);
        await Assert.That(modified.BaseScore).IsEqualTo(99);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ResourceStream
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ResourceStream_DefaultValue_ShouldBeNull(CancellationToken ct = default) {
        // Arrange & Act
        var candidate = new ManifestCandidate("/path", 10);

        // Assert
        await Assert.That(candidate.ResourceStream).IsNull();
    }

    [Test]
    public async Task ResourceStream_CanAcceptNull(CancellationToken ct = default) {
        // Arrange & Act
        var candidate = new ManifestCandidate("/path", 10);

        // Assert
        await Assert.That(candidate.ResourceStream).IsNull();
    }

    [Test]
    public async Task ResourceStream_CanAcceptMemoryStream(CancellationToken ct = default) {
        // Arrange
        using var stream = new MemoryStream([1, 2, 3]);

        // Act
        var candidate = new ManifestCandidate("/path", 10, stream);

        // Assert
        await Assert.That(candidate.ResourceStream).IsNotNull();
        await Assert.That(candidate.ResourceStream!.Length).IsEqualTo(3);
    }

    [Test]
    public async Task ResourceStream_CanAcceptFileStream(CancellationToken ct = default) {
        // Arrange
        var tempFile = Path.GetTempFileName();
        try {
            await File.WriteAllTextAsync(tempFile, "test content");
            using var stream = File.OpenRead(tempFile);

            // Act
            var candidate = new ManifestCandidate("/path", 10, stream);

            // Assert
            await Assert.That(candidate.ResourceStream).IsNotNull();
            await Assert.That(candidate.ResourceStream!.CanRead).IsTrue();
        } finally {
            File.Delete(tempFile);
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Property access
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task BaseScore_CanBeZero(CancellationToken ct = default) {
        // Arrange & Act
        var candidate = new ManifestCandidate("/path", 0);

        // Assert
        await Assert.That(candidate.BaseScore).IsEqualTo(0);
    }

    [Test]
    public async Task BaseScore_CanBeNegative(CancellationToken ct = default) {
        // Arrange & Act
        var candidate = new ManifestCandidate("/path", -1);

        // Assert
        await Assert.That(candidate.BaseScore).IsEqualTo(-1);
    }

    [Test]
    public async Task ManifestPath_CanBeEmpty(CancellationToken ct = default) {
        // Arrange & Act
        var candidate = new ManifestCandidate("", 10);

        // Assert
        await Assert.That(candidate.ManifestPath).IsEqualTo(string.Empty);
    }
}
