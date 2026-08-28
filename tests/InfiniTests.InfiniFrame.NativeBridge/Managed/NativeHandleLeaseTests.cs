// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Handles;

namespace InfiniTests.InfiniFrame.NativeBridge.Managed;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class NativeHandleLeaseTests {

    [Test]
    public async Task Handle_ReturnsCorrectPointer(CancellationToken ct = default) {
        // Arrange
        IntPtr expected = new(0x123456);
        var handle = new NativeWindowHandle(expected, false);
        var lease = new NativeHandleLease(handle);

        // Act
        IntPtr result = lease.Handle;

        // Assert
        await Assert.That(result).IsEqualTo(expected);
        lease.Dispose();
    }

    [Test]
    public async Task Handle_ReturnsExpectedValue_AfterMultipleLeases(CancellationToken ct = default) {
        // Arrange
        IntPtr expected = new(0xABCDEF);
        var handle = new NativeWindowHandle(expected, false);

        // Act
        using NativeHandleLease lease1 = new NativeHandleLease(handle);
        using NativeHandleLease lease2 = new NativeHandleLease(handle);

        // Assert
        await Assert.That(lease1.Handle).IsEqualTo(expected);
        await Assert.That(lease2.Handle).IsEqualTo(expected);
    }

    [Test]
    public async Task Dispose_MultipleCalls_DoesNotThrow(CancellationToken ct = default) {
        // Arrange
        var handle = new NativeWindowHandle(new IntPtr(42), false);
        var lease = new NativeHandleLease(handle);

        // Act & Assert
        lease.Dispose();
        lease.Dispose();
        lease.Dispose();

        await Task.CompletedTask;
    }

    [Test]
    public async Task Dispose_ReleasesHandleReference(CancellationToken ct = default) {
        // Arrange
        var handle = new NativeWindowHandle(new IntPtr(100), false);
        var lease = new NativeHandleLease(handle);

        // Act - disposing the lease releases the ref count
        lease.Dispose();

        // Assert - the SafeHandle itself isn't closed yet (ref count released, not disposed),
        // but a subsequent SafeHandle.Dispose will close it
        handle.Dispose();
        await Assert.That(handle.IsClosed).IsTrue();
    }

    [Test]
    public async Task Constructor_WithInvalidHandle_ThrowsObjectDisposedException(CancellationToken ct = default) {
        // Arrange
        var handle = new NativeWindowHandle(IntPtr.Zero, false);
        handle.Dispose();

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(() => {
            _ = new NativeHandleLease(handle);
            return Task.CompletedTask;
        });
    }

    [Test]
    public async Task Constructor_WithClosedHandle_ThrowsObjectDisposedException(CancellationToken ct = default) {
        // Arrange
        var handle = new NativeWindowHandle(new IntPtr(999), false);
        handle.Close();

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(() => {
            _ = new NativeHandleLease(handle);
            return Task.CompletedTask;
        });
    }

    [Test]
    public async Task Dispose_DoesNotAffectOtherLeases(CancellationToken ct = default) {
        // Arrange
        IntPtr value = new(0x555555);
        var handle = new NativeWindowHandle(value, false);
        var lease1 = new NativeHandleLease(handle);
        var lease2 = new NativeHandleLease(handle);

        // Act
        lease1.Dispose();

        // Assert - lease2 still holds a valid handle
        await Assert.That(lease2.Handle).IsEqualTo(value);
        lease2.Dispose();
    }

    [Test]
    public async Task Handle_RemainsConstantAcrossMultipleAccesses(CancellationToken ct = default) {
        // Arrange
        IntPtr expected = new(0xDEAD);
        var handle = new NativeWindowHandle(expected, false);
        var lease = new NativeHandleLease(handle);

        // Act & Assert
        await Assert.That(lease.Handle).IsEqualTo(expected);
        await Assert.That(lease.Handle).IsEqualTo(expected);
        await Assert.That(lease.Handle).IsEqualTo(expected);

        lease.Dispose();
    }

    [Test]
    public async Task ImplementsIDisposable(CancellationToken ct = default) {
        // Arrange
        var handle = new NativeWindowHandle(new IntPtr(1), false);

        // Act
        var lease = new NativeHandleLease(handle);

        // Assert
        await Assert.That(lease).IsAssignableTo<IDisposable>();
        lease.Dispose();
    }
}
