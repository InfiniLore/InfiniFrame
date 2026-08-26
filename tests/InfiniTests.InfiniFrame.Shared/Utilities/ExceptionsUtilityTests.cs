// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Utilities;

namespace InfiniTests.InfiniFrame.Shared.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ExceptionsUtilityTests {

    [Test]
    [Arguments(typeof(InvalidOperationException))]
    [Arguments(typeof(ArgumentException))]
    [Arguments(typeof(ArgumentNullException))]
    [Arguments(typeof(NullReferenceException))]
    [Arguments(typeof(IOException))]
    [Arguments(typeof(OperationCanceledException))]
    [Arguments(typeof(NotImplementedException))]
    [Arguments(typeof(NotSupportedException))]
    [Arguments(typeof(TimeoutException))]
    [Arguments(typeof(ObjectDisposedException))]
    public async Task IsNonFatalException_NonFatalTypes_ReturnsTrue(Type exceptionType, CancellationToken ct = default) {
        // Arrange
        var exception = (Exception)Activator.CreateInstance(exceptionType, "test")!;

        // Act
        bool result = ExceptionsUtility.IsNonFatalException(exception);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    [Arguments(typeof(OutOfMemoryException))]
    [Arguments(typeof(AccessViolationException))]
    public async Task IsNonFatalException_FatalTypes_ReturnsFalse(Type exceptionType, CancellationToken ct = default) {
        // Arrange
        var exception = (Exception)Activator.CreateInstance(exceptionType)!;

        // Act
        bool result = ExceptionsUtility.IsNonFatalException(exception);

        // Assert
        await Assert.That(result).IsFalse();
    }
}
