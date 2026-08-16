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
        // Arrange & Act & Assert
        await Assert.That(ExitCodes.Success).IsEqualTo(0);
    }

    [Test]
    public async Task GenericFailure_IsNonZero(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(ExitCodes.GenericFailure).IsNotEqualTo(0);
    }

    [Test]
    public async Task NativeDependencyMissing_IsNonZero(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(ExitCodes.NativeDependencyMissing).IsNotEqualTo(0);
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
