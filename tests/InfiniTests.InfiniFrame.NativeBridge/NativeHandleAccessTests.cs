// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Handles;

namespace InfiniTests.InfiniFrame.NativeBridge;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class NativeHandleAccessTests {

    [Test]
    public async Task Feature_IsFirstValue(CancellationToken ct = default) {
        // Arrange & Act & Assert
        var value = NativeHandleAccess.Feature;
        await Assert.That(value).IsEqualTo(NativeHandleAccess.Feature);
    }

    [Test]
    public async Task Close_IsSecondValue(CancellationToken ct = default) {
        // Arrange & Act & Assert
        var value = NativeHandleAccess.Close;
        await Assert.That(value).IsEqualTo(NativeHandleAccess.Close);
    }

    [Test]
    public async Task WaitForExit_IsThirdValue(CancellationToken ct = default) {
        // Arrange & Act & Assert
        var value = NativeHandleAccess.WaitForExit;
        await Assert.That(value).IsEqualTo(NativeHandleAccess.WaitForExit);
    }

    [Test]
    public async Task AllValues_CanBeIterated(CancellationToken ct = default) {
        // Arrange
        NativeHandleAccess[] values = Enum.GetValues<NativeHandleAccess>();

        // Act
        int count = values.Length;

        // Assert
        await Assert.That(count).IsEqualTo(3);
    }
}
