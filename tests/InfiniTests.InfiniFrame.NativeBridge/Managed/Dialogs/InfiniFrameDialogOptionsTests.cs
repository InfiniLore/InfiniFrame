// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Dialogs;

namespace InfiniTests.InfiniFrame.NativeBridge.Managed.Dialogs;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameDialogOptionsTests {

    [Test]
    public async Task None_HasValueZero(CancellationToken ct = default) {
        // Arrange & Act
        var option = InfiniFrameDialogOptions.None;

        // Assert
        await Assert.That((byte)option).IsEqualTo((byte)0);
    }

    [Test]
    public async Task MultiSelect_HasValueOne(CancellationToken ct = default) {
        // Arrange & Act
        var option = InfiniFrameDialogOptions.MultiSelect;

        // Assert
        await Assert.That((byte)option).IsEqualTo((byte)0x1);
    }

    [Test]
    public async Task ForceOverwrite_HasValueTwo(CancellationToken ct = default) {
        // Arrange & Act
        var option = InfiniFrameDialogOptions.ForceOverwrite;

        // Assert
        await Assert.That((byte)option).IsEqualTo((byte)0x2);
    }

    [Test]
    public async Task DisableCreateFolder_HasValueFour(CancellationToken ct = default) {
        // Arrange & Act
        var option = InfiniFrameDialogOptions.DisableCreateFolder;

        // Assert
        await Assert.That((byte)option).IsEqualTo((byte)0x4);
    }

    [Test]
    public async Task FlagsAreDistinctBits_NoneShareBitPositions(CancellationToken ct = default) {
        // Arrange & Act
        byte multiSelect = (byte)InfiniFrameDialogOptions.MultiSelect;
        byte forceOverwrite = (byte)InfiniFrameDialogOptions.ForceOverwrite;
        byte disableCreateFolder = (byte)InfiniFrameDialogOptions.DisableCreateFolder;

        // Assert — no two flags share a bit
        await Assert.That(multiSelect & forceOverwrite).IsEqualTo(0);
        await Assert.That(multiSelect & disableCreateFolder).IsEqualTo(0);
        await Assert.That(forceOverwrite & disableCreateFolder).IsEqualTo(0);
    }

    [Test]
    public async Task CombinedFlags_MultiSelectAndForceOverwrite_HasExpectedValue(CancellationToken ct = default) {
        // Arrange
        const byte expected = 0x1 | 0x2;

        // Act
        InfiniFrameDialogOptions combined = InfiniFrameDialogOptions.MultiSelect | InfiniFrameDialogOptions.ForceOverwrite;

        // Assert
        await Assert.That((byte)combined).IsEqualTo(expected);
    }

    [Test]
    public async Task CombinedFlags_AllOptions_HasExpectedValue(CancellationToken ct = default) {
        // Arrange
        const byte expected = 0x1 | 0x2 | 0x4;

        // Act
        InfiniFrameDialogOptions combined = InfiniFrameDialogOptions.MultiSelect
            | InfiniFrameDialogOptions.ForceOverwrite
            | InfiniFrameDialogOptions.DisableCreateFolder;

        // Assert
        await Assert.That((byte)combined).IsEqualTo(expected);
    }

    [Test]
    public async Task HasFlag_MultiSelect_WhenCombined_ReturnsTrue(CancellationToken ct = default) {
        // Arrange
        InfiniFrameDialogOptions combined = InfiniFrameDialogOptions.MultiSelect | InfiniFrameDialogOptions.ForceOverwrite;

        // Act
        bool hasMultiSelect = combined.HasFlag(InfiniFrameDialogOptions.MultiSelect);

        // Assert
        await Assert.That(hasMultiSelect).IsTrue();
    }

    [Test]
    public async Task HasFlag_DisableCreateFolder_WhenNotSet_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        InfiniFrameDialogOptions options = InfiniFrameDialogOptions.MultiSelect | InfiniFrameDialogOptions.ForceOverwrite;

        // Act
        bool hasDisableCreateFolder = options.HasFlag(InfiniFrameDialogOptions.DisableCreateFolder);

        // Assert
        await Assert.That(hasDisableCreateFolder).IsFalse();
    }

    [Test]
    public async Task None_HasFlagNone_ReturnsTrue(CancellationToken ct = default) {
        // Arrange
        var options = InfiniFrameDialogOptions.None;

        // Act
        bool hasNone = options.HasFlag(InfiniFrameDialogOptions.None);

        // Assert
        await Assert.That(hasNone).IsTrue();
    }

    [Test]
    public async Task UnderlyingType_IsByte(CancellationToken ct = default) {
        // Arrange & Act
        Type underlyingType = Enum.GetUnderlyingType(typeof(InfiniFrameDialogOptions));

        // Assert
        await Assert.That(underlyingType).IsEqualTo(typeof(byte));
    }
}
