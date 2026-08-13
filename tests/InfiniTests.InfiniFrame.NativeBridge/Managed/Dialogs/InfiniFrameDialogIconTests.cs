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
    public async Task Info_HasValueZero(CancellationToken ct = default) {
        // Arrange & Act
        var icon = InfiniFrameDialogIcon.Info;

        // Assert
        await Assert.That((int)icon).IsEqualTo(0);
    }

    [Test]
    public async Task Warning_HasValueOne(CancellationToken ct = default) {
        // Arrange & Act
        var icon = InfiniFrameDialogIcon.Warning;

        // Assert
        await Assert.That((int)icon).IsEqualTo(1);
    }

    [Test]
    public async Task Error_HasValueTwo(CancellationToken ct = default) {
        // Arrange & Act
        var icon = InfiniFrameDialogIcon.Error;

        // Assert
        await Assert.That((int)icon).IsEqualTo(2);
    }

    [Test]
    public async Task Question_HasValueThree(CancellationToken ct = default) {
        // Arrange & Act
        var icon = InfiniFrameDialogIcon.Question;

        // Assert
        await Assert.That((int)icon).IsEqualTo(3);
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
    public async Task Info_IsLessThan_Warning(CancellationToken ct = default) {
        // Arrange & Act
        int info = (int)InfiniFrameDialogIcon.Info;
        int warning = (int)InfiniFrameDialogIcon.Warning;

        // Assert, ordinal order must match the C++ enum
        await Assert.That(info).IsLessThan(warning);
    }

    [Test]
    public async Task Warning_IsLessThan_Error(CancellationToken ct = default) {
        // Arrange & Act
        int warning = (int)InfiniFrameDialogIcon.Warning;
        int error = (int)InfiniFrameDialogIcon.Error;

        // Assert
        await Assert.That(warning).IsLessThan(error);
    }

    [Test]
    public async Task Error_IsLessThan_Question(CancellationToken ct = default) {
        // Arrange & Act
        int error = (int)InfiniFrameDialogIcon.Error;
        int question = (int)InfiniFrameDialogIcon.Question;

        // Assert
        await Assert.That(error).IsLessThan(question);
    }
}
