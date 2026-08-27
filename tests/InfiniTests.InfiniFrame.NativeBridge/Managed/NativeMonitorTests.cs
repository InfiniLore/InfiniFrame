// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;
using InfiniFrame.NativeBridge;

namespace InfiniTests.InfiniFrame.NativeBridge.Managed;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class NativeMonitorTests {

    [Test]
    public async Task Default_HasZeroedMonitorRect(CancellationToken ct = default) {
        // Arrange & Act
        NativeMonitor monitor = default;

        // Assert
        await Assert.That(monitor.Monitor.X).IsEqualTo(0);
        await Assert.That(monitor.Monitor.Y).IsEqualTo(0);
        await Assert.That(monitor.Monitor.Width).IsEqualTo(0);
        await Assert.That(monitor.Monitor.Height).IsEqualTo(0);
    }

    [Test]
    public async Task Default_HasZeroedWorkRect(CancellationToken ct = default) {
        // Arrange & Act
        NativeMonitor monitor = default;

        // Assert
        await Assert.That(monitor.Work.X).IsEqualTo(0);
        await Assert.That(monitor.Work.Y).IsEqualTo(0);
        await Assert.That(monitor.Work.Width).IsEqualTo(0);
        await Assert.That(monitor.Work.Height).IsEqualTo(0);
    }

    [Test]
    public async Task Default_HasZeroScale(CancellationToken ct = default) {
        // Arrange & Act
        NativeMonitor monitor = default;

        // Assert
        await Assert.That(monitor.Scale).IsEqualTo(0.0);
    }

    [Test]
    public async Task Monitor_SetAndGet_PreservesAllCoordinates(CancellationToken ct = default) {
        // Arrange
        NativeRect rect = new() { X = 1920, Y = 0, Width = 2560, Height = 1440 };

        // Act
        NativeMonitor monitor = new() { Monitor = rect };

        // Assert
        await Assert.That(monitor.Monitor.X).IsEqualTo(1920);
        await Assert.That(monitor.Monitor.Y).IsEqualTo(0);
        await Assert.That(monitor.Monitor.Width).IsEqualTo(2560);
        await Assert.That(monitor.Monitor.Height).IsEqualTo(1440);
    }

    [Test]
    public async Task Work_SetAndGet_PreservesAllCoordinates(CancellationToken ct = default) {
        // Arrange
        NativeRect workRect = new() { X = 0, Y = 40, Width = 1920, Height = 1040 };

        // Act
        NativeMonitor monitor = new() { Work = workRect };

        // Assert
        await Assert.That(monitor.Work.X).IsEqualTo(0);
        await Assert.That(monitor.Work.Y).IsEqualTo(40);
        await Assert.That(monitor.Work.Width).IsEqualTo(1920);
        await Assert.That(monitor.Work.Height).IsEqualTo(1040);
    }

    [Test]
    public async Task Scale_SetAndGet_PreservesValue(CancellationToken ct = default) {
        // Arrange
        const double expectedScale = 1.25;

        // Act
        NativeMonitor monitor = new() { Scale = expectedScale };

        // Assert
        await Assert.That(monitor.Scale).IsEqualTo(expectedScale);
    }

    [Test]
    public async Task Scale_WithHighDpiValue_PreservesValue(CancellationToken ct = default) {
        // Arrange
        const double expectedScale = 2.0;

        // Act
        NativeMonitor monitor = new() { Scale = expectedScale };

        // Assert
        await Assert.That(monitor.Scale).IsEqualTo(expectedScale);
    }

    [Test]
    public async Task Monitor_WithNegativeOrigin_PreservesCoordinates(CancellationToken ct = default) {
        // Arrange, secondary monitor to the left of the primary
        NativeRect rect = new() { X = -1920, Y = 0, Width = 1920, Height = 1080 };

        // Act
        NativeMonitor monitor = new() { Monitor = rect };

        // Assert
        await Assert.That(monitor.Monitor.X).IsEqualTo(-1920);
        await Assert.That(monitor.Monitor.Y).IsEqualTo(0);
        await Assert.That(monitor.Monitor.Width).IsEqualTo(1920);
        await Assert.That(monitor.Monitor.Height).IsEqualTo(1080);
    }

    [Test]
    public async Task IsValueType_Copy_ProducesIndependentInstance(CancellationToken ct = default) {
        // Arrange
        NativeMonitor original = new() {
            Monitor = new NativeRect { X = 0, Y = 0, Width = 1920, Height = 1080 },
            Work = new NativeRect { X = 0, Y = 40, Width = 1920, Height = 1040 },
            Scale = 1.0
        };

        // Act
        NativeMonitor copy = original;
        copy.Scale = 2.0;

        // Assert, original is unchanged
        await Assert.That(original.Scale).IsEqualTo(1.0);
        await Assert.That(copy.Scale).IsEqualTo(2.0);
    }

    [Test]
    public async Task SequentialLayout_SizeMatchesExpected(CancellationToken ct = default) {
        // Arrange
        // Two NativeRect fields (4 × int = 16 bytes each) + one double (8 bytes) = 40 bytes
        const int expectedSize = 40;

        // Act
        int actualSize = Marshal.SizeOf<NativeMonitor>();

        // Assert
        await Assert.That(actualSize).IsEqualTo(expectedSize);
    }

    [Test]
    public async Task AllFields_WhenSetTogether_AllValuesArePreserved(CancellationToken ct = default) {
        // Arrange & Act
        NativeMonitor monitor = new() {
            Monitor = new NativeRect { X = -3840, Y = -1080, Width = 3840, Height = 2160 },
            Work = new NativeRect { X = -3840, Y = -1040, Width = 3840, Height = 2120 },
            Scale = 1.5
        };

        // Assert
        await Assert.That(monitor.Monitor.X).IsEqualTo(-3840);
        await Assert.That(monitor.Monitor.Y).IsEqualTo(-1080);
        await Assert.That(monitor.Monitor.Width).IsEqualTo(3840);
        await Assert.That(monitor.Monitor.Height).IsEqualTo(2160);
        await Assert.That(monitor.Work.X).IsEqualTo(-3840);
        await Assert.That(monitor.Work.Y).IsEqualTo(-1040);
        await Assert.That(monitor.Work.Width).IsEqualTo(3840);
        await Assert.That(monitor.Work.Height).IsEqualTo(2120);
        await Assert.That(monitor.Scale).IsEqualTo(1.5);
    }

    [Test]
    public async Task Monitor_WorkAreaSmallerThanMonitorArea_BothFieldsCoexist(CancellationToken ct = default) {
        // Arrange, typical setup: taskbar consumes 40px at the bottom
        NativeRect monitorRect = new() { X = 0, Y = 0, Width = 1920, Height = 1080 };
        NativeRect workRect = new() { X = 0, Y = 0, Width = 1920, Height = 1040 };

        // Act
        NativeMonitor monitor = new() { Monitor = monitorRect, Work = workRect };

        // Assert
        await Assert.That(monitor.Monitor.Height).IsEqualTo(1080);
        await Assert.That(monitor.Work.Height).IsEqualTo(1040);
        await Assert.That(monitor.Monitor.Height).IsGreaterThan(monitor.Work.Height);
    }
}
