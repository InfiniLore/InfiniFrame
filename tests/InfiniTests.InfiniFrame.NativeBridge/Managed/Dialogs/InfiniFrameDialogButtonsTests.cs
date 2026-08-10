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
    public async Task Ok_HasValueZero(CancellationToken ct = default) {
        // Arrange & Act
        var buttons = InfiniFrameDialogButtons.Ok;

        // Assert
        await Assert.That((int)buttons).IsEqualTo(0);
    }

    [Test]
    public async Task OkCancel_HasValueOne(CancellationToken ct = default) {
        // Arrange & Act
        var buttons = InfiniFrameDialogButtons.OkCancel;

        // Assert
        await Assert.That((int)buttons).IsEqualTo(1);
    }

    [Test]
    public async Task YesNo_HasValueTwo(CancellationToken ct = default) {
        // Arrange & Act
        var buttons = InfiniFrameDialogButtons.YesNo;

        // Assert
        await Assert.That((int)buttons).IsEqualTo(2);
    }

    [Test]
    public async Task YesNoCancel_HasValueThree(CancellationToken ct = default) {
        // Arrange & Act
        var buttons = InfiniFrameDialogButtons.YesNoCancel;

        // Assert
        await Assert.That((int)buttons).IsEqualTo(3);
    }

    [Test]
    public async Task RetryCancel_HasValueFour(CancellationToken ct = default) {
        // Arrange & Act
        var buttons = InfiniFrameDialogButtons.RetryCancel;

        // Assert
        await Assert.That((int)buttons).IsEqualTo(4);
    }

    [Test]
    public async Task AbortRetryIgnore_HasValueFive(CancellationToken ct = default) {
        // Arrange & Act
        var buttons = InfiniFrameDialogButtons.AbortRetryIgnore;

        // Assert
        await Assert.That((int)buttons).IsEqualTo(5);
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

        // Act & Assert, each value matches its ordinal index, important for native interop
        for (int i = 0; i < values.Length; i++) {
            await Assert.That((int)values[i]).IsEqualTo(i);
        }
    }
}
