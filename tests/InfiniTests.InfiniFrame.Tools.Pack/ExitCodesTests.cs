// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack;

namespace InfiniTests.InfiniFrame.Tools.Pack;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ExitCodesTests {

    [Test]
    public async Task Success_IsZero(CancellationToken ct = default) {
        // Arrange
        int value = ExitCodes.Success;

        // Act & Assert
        await Assert.That(value).IsEqualTo(0);
    }

    [Test]
    public async Task GenericFailure_IsNonZero(CancellationToken ct = default) {
        // Arrange
        int value = ExitCodes.GenericFailure;

        // Act & Assert
        await Assert.That(value).IsNotEqualTo(0);
    }

    [Test]
    public async Task NativeDependencyMissing_IsNonZero(CancellationToken ct = default) {
        // Arrange
        int value = ExitCodes.NativeDependencyMissing;

        // Act & Assert
        await Assert.That(value).IsNotEqualTo(0);
    }

    [Test]
    public async Task AllExitCodes_AreDistinct(CancellationToken ct = default) {
        // Arrange
        int[] codes = [
            ExitCodes.Success,
            ExitCodes.GenericFailure,
            ExitCodes.NativeDependencyMissing
        ];

        // Act
        int distinctCount = codes.Distinct().Count();

        // Assert
        await Assert.That(distinctCount).IsEqualTo(codes.Length);
    }
}
