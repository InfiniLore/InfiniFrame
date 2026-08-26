// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Records;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class JavaScriptEvaluationExceptionTests {

    [Test]
    public async Task Constructor_SetsMessage(CancellationToken ct = default) {
        // Arrange & Act
        var ex = new JavaScriptEvaluationException("JS error occurred");

        // Assert
        await Assert.That(ex.Message).IsEqualTo("JS error occurred");
    }

    [Test]
    public async Task Constructor_CreatesValidException(CancellationToken ct = default) {
        // Arrange & Act
        var ex = new JavaScriptEvaluationException("error");

        // Assert
        await Assert.That(ex).IsNotNull();
        await Assert.That(ex.Message).IsNotEmpty();
    }

    [Test]
    public async Task CanBeCaughtAsException(CancellationToken ct = default) {
        // Arrange & Act
        Exception? caught;
        try {
            throw new JavaScriptEvaluationException("eval failed");
        }
        catch (Exception ex) {
            caught = ex;
        }

        // Assert
        await Assert.That(caught).IsNotNull();
        await Assert.That(caught.Message).Contains("eval failed");
    }
}
