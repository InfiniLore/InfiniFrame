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
    [Arguments((ulong)1, NavigationStatus.Succeeded)]
    [Arguments((ulong)2, NavigationStatus.Failed)]
    [Arguments((ulong)3, NavigationStatus.Superseded)]
    [Arguments((ulong)4, NavigationStatus.WindowClosed)]
    public async Task Constructor_SetsStatus(ulong operationId, NavigationStatus status, CancellationToken ct = default) {
        // Arrange & Act
        var result = new NavigationResult(operationId, status);

        // Assert
        await Assert.That(result.OperationId).IsEqualTo(operationId);
        await Assert.That(result.Status).IsEqualTo(status);
    }

    [Test]
    [Arguments("")]
    [Arguments("Not found")]
    [Arguments("Connection timeout")]
    public async Task Constructor_WithFailureReason_SetsReason(string failureReason, CancellationToken ct = default) {
        // Arrange & Act
        var result = new NavigationResult(5, NavigationStatus.Failed, null, 404, failureReason);

        // Assert
        await Assert.That(result.FailureReason).IsEqualTo(failureReason);
    }

    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    [Arguments(404)]
    [Arguments(int.MaxValue)]
    public async Task Constructor_WithNativeErrorCode_SetsErrorCode(int errorCode, CancellationToken ct = default) {
        // Arrange & Act
        var result = new NavigationResult(5, NavigationStatus.Failed, null, errorCode, "error");

        // Assert
        await Assert.That(result.NativeErrorCode).IsEqualTo(errorCode);
    }

    [Test]
    public async Task Constructor_WithDefaults_OptionalPropertiesAreNull(CancellationToken ct = default) {
        // Arrange & Act
        var result = new NavigationResult(42, NavigationStatus.Failed);

        // Assert
        await Assert.That(result.Uri).IsNull();
        await Assert.That(result.FailureReason).IsNull();
    }

    [Test]
    public async Task Equality_SameValues_ReturnsTrue(CancellationToken ct = default) {
        // Arrange
        var uri = new Uri("https://example.com");
        var r1 = new NavigationResult(1, NavigationStatus.Succeeded, uri);
        var r2 = new NavigationResult(1, NavigationStatus.Succeeded, uri);

        // Assert
        await Assert.That(r1).IsEqualTo(r2);
    }

    [Test]
    public async Task Equality_DifferentValues_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        var r1 = new NavigationResult(1, NavigationStatus.Succeeded);
        var r2 = new NavigationResult(2, NavigationStatus.Succeeded);

        // Assert
        await Assert.That(r1).IsNotEqualTo(r2);
    }

    [Test]
    public async Task WithExpression_CreatesNewInstance(CancellationToken ct = default) {
        // Arrange
        var original = new NavigationResult(1, NavigationStatus.Succeeded, new Uri("https://example.com"));

        // Act
        NavigationResult modified = original with { Status = NavigationStatus.Failed };

        // Assert
        await Assert.That(modified.Status).IsEqualTo(NavigationStatus.Failed);
        await Assert.That(modified.OperationId).IsEqualTo((ulong)1);
        await Assert.That(original.Status).IsEqualTo(NavigationStatus.Succeeded);
    }
}
