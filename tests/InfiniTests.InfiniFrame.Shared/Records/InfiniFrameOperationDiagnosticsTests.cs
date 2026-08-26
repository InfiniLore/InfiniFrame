// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Debugging;

namespace InfiniTests.InfiniFrame.Shared.Records;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameOperationDiagnosticsTests {

    [Test]
    public async Task Constructor_SetsRequiredProperties(CancellationToken ct = default) {
        // Arrange
        DateTimeOffset started = DateTimeOffset.UtcNow;

        // Act
        var diag = new InfiniFrameOperationDiagnostics {
            Name = "TestOp",
            Id = 42,
            StartedUtc = started,
            FinalState = "Completed"
        };

        // Assert
        await Assert.That(diag.Name).IsEqualTo("TestOp");
        await Assert.That(diag.Id).IsEqualTo((ulong)42);
        await Assert.That(diag.StartedUtc).IsEqualTo(started);
        await Assert.That(diag.FinalState).IsEqualTo("Completed");
    }

    [Test]
    public async Task OptionalProperties_DefaultToNull(CancellationToken ct = default) {
        // Arrange & Act
        var diag = new InfiniFrameOperationDiagnostics {
            Name = "Op",
            Id = 1,
            StartedUtc = DateTimeOffset.UtcNow,
            FinalState = "Running"
        };

        // Assert
        await Assert.That(diag.CompletedUtc).IsNull();
        await Assert.That(diag.NativeCode).IsNull();
        await Assert.That(diag.FailureReason).IsNull();
    }

    [Test]
    public async Task AllProperties_CanBeSet(CancellationToken ct = default) {
        // Arrange
        DateTimeOffset started = DateTimeOffset.UtcNow;
        DateTimeOffset completed = started.AddSeconds(5);

        // Act
        var diag = new InfiniFrameOperationDiagnostics {
            Name = "Navigate",
            Id = 100,
            StartedUtc = started,
            CompletedUtc = completed,
            FinalState = "Failed",
            NativeCode = 404,
            FailureReason = "Not found"
        };

        // Assert
        await Assert.That(diag.CompletedUtc).IsEqualTo(completed);
        await Assert.That(diag.NativeCode).IsEqualTo(404);
        await Assert.That(diag.FailureReason).IsEqualTo("Not found");
    }

    [Test]
    public async Task Equality_SameValues_ReturnsTrue(CancellationToken ct = default) {
        // Arrange
        DateTimeOffset time = DateTimeOffset.UtcNow;
        var d1 = new InfiniFrameOperationDiagnostics { Name = "X", Id = 1, StartedUtc = time, FinalState = "Done" };
        var d2 = new InfiniFrameOperationDiagnostics { Name = "X", Id = 1, StartedUtc = time, FinalState = "Done" };

        // Act & Assert
        await Assert.That(d1).IsEqualTo(d2);
    }
}
