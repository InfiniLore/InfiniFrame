// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;

namespace InfiniTests.InfiniFrame.Shared.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InteropGetMessageErrorResponseTests {

    [Test]
    public async Task Constructor_DefaultValues(CancellationToken ct = default) {
        // Arrange & Act
        var response = new InteropGetMessageErrorResponse();

        // Assert
        await Assert.That(response.RequestId).IsNull();
        await Assert.That(response.Success).IsFalse();
        await Assert.That(response.Error).IsNull();
    }

    [Test]
    public async Task Properties_CanBeSet(CancellationToken ct = default) {
        // Arrange & Act
        var response = new InteropGetMessageErrorResponse {
            RequestId = "req-456",
            Success = false,
            Error = "Something went wrong"
        };

        // Assert
        await Assert.That(response.RequestId).IsEqualTo("req-456");
        await Assert.That(response.Success).IsFalse();
        await Assert.That(response.Error).IsEqualTo("Something went wrong");
    }
}
