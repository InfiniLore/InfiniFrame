// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;

namespace InfiniTests.InfiniFrame.NativeBridge;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameNativeInteropStatusTests {

    [Test]
    public async Task Success_IsZero(CancellationToken ct = default) {
        // Arrange & Act & Assert
        var value = InfiniFrameNativeInteropStatus.Success;
        await Assert.That(value).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
    }

    [Test]
    public async Task InvalidArgument_IsValue(CancellationToken ct = default) {
        // Arrange & Act & Assert
        var value = InfiniFrameNativeInteropStatus.InvalidArgument;
        await Assert.That(value).IsEqualTo(InfiniFrameNativeInteropStatus.InvalidArgument);
    }

    [Test]
    public async Task OutParameterSetToInvalidNull_IsValue(CancellationToken ct = default) {
        // Arrange & Act & Assert
        var value = InfiniFrameNativeInteropStatus.OutParameterSetToInvalidNull;
        await Assert.That(value).IsEqualTo(InfiniFrameNativeInteropStatus.OutParameterSetToInvalidNull);
    }

    [Test]
    public async Task OperationFailed_IsValue(CancellationToken ct = default) {
        // Arrange & Act & Assert
        var value = InfiniFrameNativeInteropStatus.OperationFailed;
        await Assert.That(value).IsEqualTo(InfiniFrameNativeInteropStatus.OperationFailed);
    }

    [Test]
    public async Task AllValues_CanBeIterated(CancellationToken ct = default) {
        // Arrange
        InfiniFrameNativeInteropStatus[] values = Enum.GetValues<InfiniFrameNativeInteropStatus>();

        // Act
        int count = values.Length;

        // Assert
        await Assert.That(count).IsEqualTo(4);
    }
}
