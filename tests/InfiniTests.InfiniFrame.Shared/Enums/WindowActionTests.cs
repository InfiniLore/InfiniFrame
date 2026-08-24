// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Blazor;

namespace InfiniTests.InfiniFrame.Shared.Enums;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowActionTests {

    [Test]
    [Arguments(WindowAction.Minimize)]
    [Arguments(WindowAction.Maximize)]
    [Arguments(WindowAction.Close)]
    public async Task Value_CanBeAssigned(WindowAction value, CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(value).IsEqualTo(value);
    }

    [Test]
    public async Task AllValues_CanBeIterated(CancellationToken ct = default) {
        // Arrange
        WindowAction[] values = Enum.GetValues<WindowAction>();

        // Act
        int count = values.Length;

        // Assert
        await Assert.That(count).IsEqualTo(3);
    }
}
