// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Enums;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class NavigationStatusTests {

    [Test]
    [Arguments(NavigationStatus.Succeeded)]
    [Arguments(NavigationStatus.Failed)]
    [Arguments(NavigationStatus.Superseded)]
    [Arguments(NavigationStatus.WindowClosed)]
    public async Task Value_CanBeAssigned(NavigationStatus value, CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(value).IsEqualTo(value);
    }

    [Test]
    public async Task AllValues_CanBeIterated(CancellationToken ct = default) {
        // Arrange
        NavigationStatus[] values = Enum.GetValues<NavigationStatus>();

        // Act
        int count = values.Length;

        // Assert
        await Assert.That(count).IsEqualTo(4);
    }
}
