// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using System.Runtime.InteropServices;

namespace InfiniTests.InfiniFrame.NativeBridge.Managed;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class NativeRectTests {

    [Test]
    public async Task Default_HasZeroX(CancellationToken ct = default) {
        // Arrange & Act
        NativeRect rect = default;

        // Assert
        await Assert.That(rect.X).IsEqualTo(0);
    }

    [Test]
    public async Task Default_HasZeroY(CancellationToken ct = default) {
        // Arrange & Act
        NativeRect rect = default;

        // Assert
        await Assert.That(rect.Y).IsEqualTo(0);
    }

    [Test]
    public async Task Default_HasZeroWidth(CancellationToken ct = default) {
        // Arrange & Act
        NativeRect rect = default;

        // Assert
        await Assert.That(rect.Width).IsEqualTo(0);
    }

    [Test]
    public async Task Default_HasZeroHeight(CancellationToken ct = default) {
        // Arrange & Act
        NativeRect rect = default;

        // Assert
        await Assert.That(rect.Height).IsEqualTo(0);
    }

    [Test]
    public async Task X_SetAndGet_PreservesValue(CancellationToken ct = default) {
        // Arrange
        const int expectedX = 1920;

        // Act
        NativeRect rect = new() { X = expectedX };

        // Assert
        await Assert.That(rect.X).IsEqualTo(expectedX);
    }

    [Test]
    public async Task Y_SetAndGet_PreservesValue(CancellationToken ct = default) {
        // Arrange
        const int expectedY = 1080;

        // Act
        NativeRect rect = new() { Y = expectedY };

        // Assert
        await Assert.That(rect.Y).IsEqualTo(expectedY);
    }

    [Test]
    public async Task Width_SetAndGet_PreservesValue(CancellationToken ct = default) {
        // Arrange
        const int expectedWidth = 2560;

        // Act
        NativeRect rect = new() { Width = expectedWidth };

        // Assert
        await Assert.That(rect.Width).IsEqualTo(expectedWidth);
    }

    [Test]
    public async Task Height_SetAndGet_PreservesValue(CancellationToken ct = default) {
        // Arrange
        const int expectedHeight = 1440;

        // Act
        NativeRect rect = new() { Height = expectedHeight };

        // Assert
        await Assert.That(rect.Height).IsEqualTo(expectedHeight);
    }

    [Test]
    public async Task X_WithNegativeValue_PreservesValue(CancellationToken ct = default) {
        // Arrange — monitor to the left of the primary has a negative X origin
        const int expectedX = -1920;

        // Act
        NativeRect rect = new() { X = expectedX };

        // Assert
        await Assert.That(rect.X).IsEqualTo(expectedX);
    }

    [Test]
    public async Task Y_WithNegativeValue_PreservesValue(CancellationToken ct = default) {
        // Arrange — monitor above the primary has a negative Y origin
        const int expectedY = -1080;

        // Act
        NativeRect rect = new() { Y = expectedY };

        // Assert
        await Assert.That(rect.Y).IsEqualTo(expectedY);
    }

    [Test]
    public async Task Width_WithMaxIntValue_PreservesValue(CancellationToken ct = default) {
        // Arrange
        const int expectedWidth = int.MaxValue;

        // Act
        NativeRect rect = new() { Width = expectedWidth };

        // Assert
        await Assert.That(rect.Width).IsEqualTo(expectedWidth);
    }

    [Test]
    public async Task Height_WithMaxIntValue_PreservesValue(CancellationToken ct = default) {
        // Arrange
        const int expectedHeight = int.MaxValue;

        // Act
        NativeRect rect = new() { Height = expectedHeight };

        // Assert
        await Assert.That(rect.Height).IsEqualTo(expectedHeight);
    }

    [Test]
    public async Task IsValueType_Copy_ProducesIndependentInstance(CancellationToken ct = default) {
        // Arrange
        NativeRect original = new() { X = 0, Y = 0, Width = 1920, Height = 1080 };

        // Act
        NativeRect copy = original;
        copy.X = 500;

        // Assert — original is unchanged
        await Assert.That(original.X).IsEqualTo(0);
        await Assert.That(copy.X).IsEqualTo(500);
    }

    [Test]
    public async Task SequentialLayout_SizeMatchesExpected(CancellationToken ct = default) {
        // Arrange
        // Four int fields (4 bytes each) = 16 bytes
        const int expectedSize = 16;

        // Act
        int actualSize = Marshal.SizeOf<NativeRect>();

        // Assert
        await Assert.That(actualSize).IsEqualTo(expectedSize);
    }

    [Test]
    public async Task AllFields_WhenSetTogether_AllValuesArePreserved(CancellationToken ct = default) {
        // Arrange & Act
        NativeRect rect = new() { X = -800, Y = -600, Width = 3840, Height = 2160 };

        // Assert
        await Assert.That(rect.X).IsEqualTo(-800);
        await Assert.That(rect.Y).IsEqualTo(-600);
        await Assert.That(rect.Width).IsEqualTo(3840);
        await Assert.That(rect.Height).IsEqualTo(2160);
    }
}
