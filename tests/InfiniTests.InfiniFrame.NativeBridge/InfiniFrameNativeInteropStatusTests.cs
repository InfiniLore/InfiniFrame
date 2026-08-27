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

    [Test]
    public async Task Success_HasValueZero(CancellationToken ct = default) {
        // Arrange & Act
        int value = (int)InfiniFrameNativeInteropStatus.Success;

        // Assert
        await Assert.That(value).IsEqualTo(0);
    }

    [Test]
    public async Task InvalidArgument_HasValue22(CancellationToken ct = default) {
        // Arrange & Act
        int value = (int)InfiniFrameNativeInteropStatus.InvalidArgument;

        // Assert
        await Assert.That(value).IsEqualTo(22);
    }

    [Test]
    public async Task OutParameterSetToInvalidNull_HasValue2001(CancellationToken ct = default) {
        // Arrange & Act
        int value = (int)InfiniFrameNativeInteropStatus.OutParameterSetToInvalidNull;

        // Assert
        await Assert.That(value).IsEqualTo(2001);
    }

    [Test]
    public async Task OperationFailed_HasValue14(CancellationToken ct = default) {
        // Arrange & Act
        int value = (int)InfiniFrameNativeInteropStatus.OperationFailed;

        // Assert
        await Assert.That(value).IsEqualTo(14);
    }

    [Test]
    public async Task AllValues_AreDistinct(CancellationToken ct = default) {
        // Arrange
        InfiniFrameNativeInteropStatus[] values = Enum.GetValues<InfiniFrameNativeInteropStatus>();

        // Act
        int[] intValues = [.. values.Select(v => (int)v)];

        // Assert
        await Assert.That(intValues.Distinct().Count()).IsEqualTo(values.Length);
    }

    [Test]
    public async Task AllValues_CanBeCastFromInt(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(Enum.IsDefined((InfiniFrameNativeInteropStatus)0)).IsTrue();
        await Assert.That(Enum.IsDefined((InfiniFrameNativeInteropStatus)22)).IsTrue();
        await Assert.That(Enum.IsDefined((InfiniFrameNativeInteropStatus)2001)).IsTrue();
        await Assert.That(Enum.IsDefined((InfiniFrameNativeInteropStatus)14)).IsTrue();
        await Assert.That(Enum.IsDefined((InfiniFrameNativeInteropStatus)999)).IsFalse();
    }
}
