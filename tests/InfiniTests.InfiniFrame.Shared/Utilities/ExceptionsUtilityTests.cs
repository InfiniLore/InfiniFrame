// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Utilities;

namespace InfiniTests.InfiniFrame.Shared.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ExceptionsUtilityTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Non-fatal exceptions — should return true
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task IsNonFatalException_InvalidOperationException_ReturnsTrue(CancellationToken ct = default) {
        // Arrange
        var exception = new InvalidOperationException("test");

        // Act
        bool result = ExceptionsUtility.IsNonFatalException(exception);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task IsNonFatalException_ArgumentException_ReturnsTrue(CancellationToken ct = default) {
        // Arrange
        var exception = new ArgumentException("test");

        // Act
        bool result = ExceptionsUtility.IsNonFatalException(exception);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task IsNonFatalException_NullReferenceException_ReturnsTrue(CancellationToken ct = default) {
        // Arrange
        var exception = new NullReferenceException();

        // Act
        bool result = ExceptionsUtility.IsNonFatalException(exception);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task IsNonFatalException_IOException_ReturnsTrue(CancellationToken ct = default) {
        // Arrange
        var exception = new IOException("disk error");

        // Act
        bool result = ExceptionsUtility.IsNonFatalException(exception);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task IsNonFatalException_OperationCanceledException_ReturnsTrue(CancellationToken ct = default) {
        // Arrange — OperationCanceledException is not in the fatal list
        var exception = new OperationCanceledException();

        // Act
        bool result = ExceptionsUtility.IsNonFatalException(exception);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task IsNonFatalException_NotImplementedException_ReturnsTrue(CancellationToken ct = default) {
        // Arrange
        var exception = new NotImplementedException();

        // Act
        bool result = ExceptionsUtility.IsNonFatalException(exception);

        // Assert
        await Assert.That(result).IsTrue();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Fatal exceptions — should return false
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task IsNonFatalException_OutOfMemoryException_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        var exception = new OutOfMemoryException();

        // Act
        bool result = ExceptionsUtility.IsNonFatalException(exception);

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task IsNonFatalException_AccessViolationException_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        var exception = new AccessViolationException();

        // Act
        bool result = ExceptionsUtility.IsNonFatalException(exception);

        // Assert
        await Assert.That(result).IsFalse();
    }
}
