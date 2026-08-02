// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Dialogs;

namespace InfiniTests.InfiniFrame.NativeBridge.Managed.Dialogs;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameDialogResultTests {

    [Test]
    public async Task Cancel_HasValueMinusOne(CancellationToken ct = default) {
        // Arrange & Act
        var result = InfiniFrameDialogResult.Cancel;

        // Assert
        await Assert.That((int)result).IsEqualTo(-1);
    }

    [Test]
    public async Task Ok_HasValueZero(CancellationToken ct = default) {
        // Arrange & Act
        var result = InfiniFrameDialogResult.Ok;

        // Assert
        await Assert.That((int)result).IsEqualTo(0);
    }

    [Test]
    public async Task Yes_HasValueOne(CancellationToken ct = default) {
        // Arrange & Act
        var result = InfiniFrameDialogResult.Yes;

        // Assert
        await Assert.That((int)result).IsEqualTo(1);
    }

    [Test]
    public async Task No_HasValueTwo(CancellationToken ct = default) {
        // Arrange & Act
        var result = InfiniFrameDialogResult.No;

        // Assert
        await Assert.That((int)result).IsEqualTo(2);
    }

    [Test]
    public async Task Abort_HasValueThree(CancellationToken ct = default) {
        // Arrange & Act
        var result = InfiniFrameDialogResult.Abort;

        // Assert
        await Assert.That((int)result).IsEqualTo(3);
    }

    [Test]
    public async Task Retry_HasValueFour(CancellationToken ct = default) {
        // Arrange & Act
        var result = InfiniFrameDialogResult.Retry;

        // Assert
        await Assert.That((int)result).IsEqualTo(4);
    }

    [Test]
    public async Task Ignore_HasValueFive(CancellationToken ct = default) {
        // Arrange & Act
        var result = InfiniFrameDialogResult.Ignore;

        // Assert
        await Assert.That((int)result).IsEqualTo(5);
    }

    [Test]
    public async Task AllValues_AreDistinct(CancellationToken ct = default) {
        // Arrange
        var values = (InfiniFrameDialogResult[])Enum.GetValues(typeof(InfiniFrameDialogResult));

        // Act
        int distinctCount = values.Select(v => (int)v).Distinct().Count();

        // Assert
        await Assert.That(distinctCount).IsEqualTo(values.Length);
    }

    [Test]
    public async Task Cancel_IsLessThan_Ok(CancellationToken ct = default) {
        // Arrange & Act
        int cancel = (int)InfiniFrameDialogResult.Cancel;
        int ok = (int)InfiniFrameDialogResult.Ok;

        // Assert — Cancel is -1, negative sentinal value
        await Assert.That(cancel).IsLessThan(ok);
    }
}