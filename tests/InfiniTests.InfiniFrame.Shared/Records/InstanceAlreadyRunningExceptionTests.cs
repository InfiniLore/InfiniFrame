// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Records;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InstanceAlreadyRunningExceptionTests {

    [Test]
    public async Task Constructor_SetsMessage(CancellationToken ct = default) {
        // Arrange & Act
        var ex = new InstanceAlreadyRunningException();

        // Assert
        await Assert.That(ex.Message).IsEqualTo("Another instance of the application is already running.");
    }

    [Test]
    public async Task Constructor_CreatesValidException(CancellationToken ct = default) {
        // Arrange & Act
        var ex = new InstanceAlreadyRunningException();

        // Assert
        await Assert.That(ex).IsNotNull();
        await Assert.That(ex.Message).IsNotEmpty();
    }

    [Test]
    public async Task CanBeCaughtAsInvalidOperationException(CancellationToken ct = default) {
        // Arrange & Act
        InvalidOperationException? caught = null;
        try {
            throw new InstanceAlreadyRunningException();
        }
        catch (InvalidOperationException ex) {
            caught = ex;
        }

        // Assert
        await Assert.That(caught).IsNotNull();
        await Assert.That(caught.Message).Contains("already running");
    }
}
