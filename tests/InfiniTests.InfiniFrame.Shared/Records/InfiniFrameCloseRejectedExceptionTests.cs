// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Records;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameCloseRejectedExceptionTests {

    [Test]
    public async Task Constructor_SetsMessage(CancellationToken ct = default) {
        // Arrange & Act
        var ex = new InfiniFrameCloseRejectedException();

        // Assert
        await Assert.That(ex.Message).IsEqualTo("The window close request was rejected by a window-closing handler.");
    }

    [Test]
    public async Task Constructor_CreatesValidException(CancellationToken ct = default) {
        // Arrange & Act
        var ex = new InfiniFrameCloseRejectedException();

        // Assert
        await Assert.That(ex).IsNotNull();
        await Assert.That(ex.Message).IsNotEmpty();
    }

    [Test]
    public async Task CanBeCaughtAsInvalidOperationException(CancellationToken ct = default) {
        // Arrange & Act
        InvalidOperationException? caught;
        try {
            throw new InfiniFrameCloseRejectedException();
        }
        catch (InvalidOperationException ex) {
            caught = ex;
        }

        // Assert
        await Assert.That(caught).IsNotNull();
        await Assert.That(caught.Message).Contains("rejected");
    }
}
