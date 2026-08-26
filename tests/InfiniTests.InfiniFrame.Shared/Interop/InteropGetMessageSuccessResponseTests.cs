// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;

namespace InfiniTests.InfiniFrame.Shared.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InteropGetMessageSuccessResponseTests {

    [Test]
    public async Task Constructor_DefaultValues(CancellationToken ct = default) {
        // Arrange & Act
        var response = new InteropGetMessageSuccessResponse();

        // Assert
        await Assert.That(response.RequestId).IsNull();
        await Assert.That(response.Success).IsFalse();
        await Assert.That(response.Data).IsNull();
    }

    [Test]
    public async Task Properties_CanBeSet(CancellationToken ct = default) {
        // Arrange & Act
        var response = new InteropGetMessageSuccessResponse {
            RequestId = "req-123",
            Success = true,
            Data = "{\"key\":\"value\"}"
        };

        // Assert
        await Assert.That(response.RequestId).IsEqualTo("req-123");
        await Assert.That(response.Success).IsTrue();
        await Assert.That(response.Data).IsEqualTo("{\"key\":\"value\"}");
    }
}
