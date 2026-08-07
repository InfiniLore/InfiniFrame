// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniTests.InfiniFrame.Window.Features.InstanceArbitration;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InstanceArbitrationTests {

    [Test]
    [NotInParallelInfiniTests]
    public async Task TryAcquirePrimaryInstance_FirstCall_ReturnsTrue(CancellationToken ct) {
        // Arrange
        string mutexName = $"InfiniFrame.Test.{Guid.NewGuid():N}";

        // Act
        bool result = global::InfiniFrame.InstanceArbitration.TryAcquirePrimaryInstance(mutexName);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task TryAcquirePrimaryInstance_SecondCall_ReturnsFalse(CancellationToken ct) {
        // Arrange
        string mutexName = $"InfiniFrame.Test.{Guid.NewGuid():N}";

        // Act
        bool first = global::InfiniFrame.InstanceArbitration.TryAcquirePrimaryInstance(mutexName);
        bool second = global::InfiniFrame.InstanceArbitration.TryAcquirePrimaryInstance(mutexName);

        // Assert
        await Assert.That(first).IsTrue();
        await Assert.That(second).IsFalse();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task TryAcquirePrimaryInstance_NullMutexName_UsesDefault(CancellationToken ct) {
        // Arrange - acquire the default mutex (or discover another process already holds it)
        bool first = global::InfiniFrame.InstanceArbitration.TryAcquirePrimaryInstance(null);

        // Act - call again with null; same default name means this must fail within the same process
        bool second = global::InfiniFrame.InstanceArbitration.TryAcquirePrimaryInstance(null);

        // Assert - verify the method uses a consistent name (not a new GUID each time)
        // If the first call succeeded, the second must fail (same mutex name).
        // If another TFM runner holds the mutex, both return false — still correct.
        await Assert.That(first || !second).IsTrue();
    }

    [Test]
    public async Task IsProcessElevated_ReturnsTrueOrFalse(CancellationToken ct) {
        // Act
        bool result = global::InfiniFrame.InstanceArbitration.IsProcessElevated();

        // Assert - verify it returns a valid bool without throwing
        await Assert.That(result || !result).IsTrue();
    }
}
