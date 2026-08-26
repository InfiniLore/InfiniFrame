// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Dialogs;

namespace InfiniTests.InfiniFrame.NativeBridge.Managed.Dialogs;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameDialogIconTests {

    [Test]
    [Arguments(InfiniFrameDialogIcon.Info, 0)]
    [Arguments(InfiniFrameDialogIcon.Warning, 1)]
    [Arguments(InfiniFrameDialogIcon.Error, 2)]
    [Arguments(InfiniFrameDialogIcon.Question, 3)]
    public async Task HasExpectedIntValue(InfiniFrameDialogIcon icon, int expected, CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That((int)icon).IsEqualTo(expected);
    }

    [Test]
    public async Task AllValues_AreDistinct(CancellationToken ct = default) {
        // Arrange
        var values = (InfiniFrameDialogIcon[])Enum.GetValues(typeof(InfiniFrameDialogIcon));

        // Act
        int distinctCount = values.Select(v => (int)v).Distinct().Count();

        // Assert
        await Assert.That(distinctCount).IsEqualTo(values.Length);
    }

    [Test]
    [Arguments(InfiniFrameDialogIcon.Info, InfiniFrameDialogIcon.Warning)]
    [Arguments(InfiniFrameDialogIcon.Warning, InfiniFrameDialogIcon.Error)]
    [Arguments(InfiniFrameDialogIcon.Error, InfiniFrameDialogIcon.Question)]
    public async Task IsLessThan_NextValue(InfiniFrameDialogIcon smaller, InfiniFrameDialogIcon larger, CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That((int)smaller).IsLessThan((int)larger);
    }
}
