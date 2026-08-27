// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using InfiniFrame.NativeBridge.Handles;

namespace InfiniTests.InfiniFrame.NativeBridge.Managed;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public class NativeWindowHandleTests {
    // Keep the stress deterministic across Linux runners with very different CPU counts.
    private const int ConcurrentWorkerCount = 4;

    [Test]
    [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
    public async Task ConcurrentAcquireAndShutdown_NeverReturnsAStaleHandle(CancellationToken ct = default) {
        // Arrange
        IntPtr value = new(0x123456);
        using var owner = new TestOwner(value);
        int successfulAcquisitions = 0;

        Task[] workers = [
            .. Enumerable.Range(0, ConcurrentWorkerCount)
                .Select(_ => Task.Run(action: () => {
                    for (int i = 0; i < 2_000; i++) {
                        try {
                            using NativeHandleLease lease = owner.AcquireNativeHandle();
                            if (lease.Handle != value) throw new InvalidOperationException("Stale handle acquired.");

                            Interlocked.Increment(ref successfulAcquisitions);
                            Thread.Yield();
                        }
                        catch (ObjectDisposedException) {
                            return;
                        }
                    }
                }, ct))
        ];

        // Act
        await Task.Yield();
        // ReSharper disable once DisposeOnUsingVariable
        owner.Dispose();
        await Task.WhenAll(workers);

        // Assert
        await Assert.That(successfulAcquisitions).IsGreaterThanOrEqualTo(0);
        await Assert.That(() => owner.AcquireNativeHandle()).Throws<ObjectDisposedException>();
    }

    [Test]
    public async Task DisposingSafeHandle_WaitsForOutstandingLease(CancellationToken ct = default) {
        // Arrange
        IntPtr value = new(0x654321);
        var handle = new NativeWindowHandle(value, false);
        var lease = new NativeHandleLease(handle);

        // Act
        handle.Dispose();

        // Assert
        await Assert.That(lease.Handle).IsEqualTo(value);

        // Act
        lease.Dispose();

        // Assert
        await Assert.That(handle.IsClosed).IsTrue();
    }

    private sealed class TestOwner(IntPtr value) : INativeWindowHandleOwner, IDisposable {
        private readonly NativeWindowHandle _handle = new(value, false);
        private int _closed;

        public void Dispose() {
            Volatile.Write(ref _closed, 1);
            _handle.Dispose();
        }

        public NativeHandleLease AcquireNativeHandle(NativeHandleAccess access = NativeHandleAccess.Feature) {
            ObjectDisposedException.ThrowIf(Volatile.Read(ref _closed) != 0, nameof(TestOwner));
            return new NativeHandleLease(_handle);
        }
    }
}
