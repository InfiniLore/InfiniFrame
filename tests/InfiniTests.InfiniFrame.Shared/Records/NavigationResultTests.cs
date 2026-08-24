// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Records;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class NavigationResultTests {

    [Test]
    public async Task Constructor_SetsProperties(CancellationToken ct = default) {
        // Arrange & Act
        var result = new NavigationResult((ulong)1, NavigationStatus.Succeeded, new Uri("https://example.com"));

        // Assert
        await Assert.That(result.OperationId).IsEqualTo((ulong)1);
        await Assert.That(result.Status).IsEqualTo(NavigationStatus.Succeeded);
        await Assert.That(result.Uri).IsEqualTo(new Uri("https://example.com"));
        await Assert.That(result.NativeErrorCode).IsEqualTo(0);
        await Assert.That(result.FailureReason).IsNull();
    }

    [Test]
    public async Task Constructor_WithDefaults_OptionalPropertiesAreNull(CancellationToken ct = default) {
        // Arrange & Act
        var result = new NavigationResult((ulong)42, NavigationStatus.Failed);

        // Assert
        await Assert.That(result.OperationId).IsEqualTo((ulong)42);
        await Assert.That(result.Status).IsEqualTo(NavigationStatus.Failed);
        await Assert.That(result.Uri).IsNull();
        await Assert.That(result.NativeErrorCode).IsEqualTo(0);
        await Assert.That(result.FailureReason).IsNull();
    }

    [Test]
    public async Task Constructor_WithFailureReason_SetsReason(CancellationToken ct = default) {
        // Arrange & Act
        var result = new NavigationResult((ulong)5, NavigationStatus.Failed, null, 404, "Not found");

        // Assert
        await Assert.That(result.FailureReason).IsEqualTo("Not found");
        await Assert.That(result.NativeErrorCode).IsEqualTo(404);
    }

    [Test]
    public async Task Equality_SameValues_ReturnsTrue(CancellationToken ct = default) {
        // Arrange
        var uri = new Uri("https://example.com");
        var r1 = new NavigationResult((ulong)1, NavigationStatus.Succeeded, uri);
        var r2 = new NavigationResult((ulong)1, NavigationStatus.Succeeded, uri);

        // Act & Assert
        await Assert.That(r1).IsEqualTo(r2);
    }

    [Test]
    public async Task Equality_DifferentValues_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        var r1 = new NavigationResult((ulong)1, NavigationStatus.Succeeded);
        var r2 = new NavigationResult((ulong)2, NavigationStatus.Succeeded);

        // Act & Assert
        await Assert.That(r1).IsNotEqualTo(r2);
    }

    [Test]
    public async Task WithExpression_CreatesNewInstance(CancellationToken ct = default) {
        // Arrange
        var original = new NavigationResult((ulong)1, NavigationStatus.Succeeded, new Uri("https://example.com"));

        // Act
        NavigationResult modified = original with { Status = NavigationStatus.Failed };

        // Assert
        await Assert.That(modified.Status).IsEqualTo(NavigationStatus.Failed);
        await Assert.That(modified.OperationId).IsEqualTo((ulong)1);
        await Assert.That(original.Status).IsEqualTo(NavigationStatus.Succeeded);
    }

    [Test]
    public async Task WindowClosed_Status_HasCorrectValue(CancellationToken ct = default) {
        // Arrange & Act
        var result = new NavigationResult((ulong)1, NavigationStatus.WindowClosed);

        // Assert
        await Assert.That(result.Status).IsEqualTo(NavigationStatus.WindowClosed);
    }

    [Test]
    public async Task Superseded_Status_HasCorrectValue(CancellationToken ct = default) {
        // Arrange & Act
        var result = new NavigationResult((ulong)1, NavigationStatus.Superseded);

        // Assert
        await Assert.That(result.Status).IsEqualTo(NavigationStatus.Superseded);
    }
}
