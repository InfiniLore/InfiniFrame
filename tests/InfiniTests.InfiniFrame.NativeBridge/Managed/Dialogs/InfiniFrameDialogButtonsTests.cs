// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Dialogs;

namespace InfiniTests.InfiniFrame.NativeBridge.Managed.Dialogs;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameDialogButtonsTests {

    [Test]
    [Arguments(InfiniFrameDialogButtons.Ok, 0)]
    [Arguments(InfiniFrameDialogButtons.OkCancel, 1)]
    [Arguments(InfiniFrameDialogButtons.YesNo, 2)]
    [Arguments(InfiniFrameDialogButtons.YesNoCancel, 3)]
    [Arguments(InfiniFrameDialogButtons.RetryCancel, 4)]
    [Arguments(InfiniFrameDialogButtons.AbortRetryIgnore, 5)]
    public async Task HasExpectedIntValue(InfiniFrameDialogButtons buttons, int expected, CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That((int)buttons).IsEqualTo(expected);
    }

    [Test]
    public async Task AllValues_AreDistinct(CancellationToken ct = default) {
        // Arrange
        var values = (InfiniFrameDialogButtons[])Enum.GetValues(typeof(InfiniFrameDialogButtons));

        // Act
        int distinctCount = values.Select(v => (int)v).Distinct().Count();

        // Assert
        await Assert.That(distinctCount).IsEqualTo(values.Length);
    }

    [Test]
    public async Task Values_AreSequentialFromZero(CancellationToken ct = default) {
        // Arrange
        var values = (InfiniFrameDialogButtons[])Enum.GetValues(typeof(InfiniFrameDialogButtons));

        // Assert
        for (int i = 0; i < values.Length; i++) {
            await Assert.That((int)values[i]).IsEqualTo(i);
        }
    }
}
