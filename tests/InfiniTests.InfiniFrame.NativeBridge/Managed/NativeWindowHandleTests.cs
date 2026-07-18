// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Handles;

namespace InfiniTests.InfiniFrame.NativeBridge.Managed;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public class NativeWindowHandleTests {
    private sealed class TestOwner(IntPtr value) : INativeWindowHandleOwner, IDisposable {
        private readonly NativeWindowHandle _handle = new(value, ownsHandle: false);
        private int _closed;

        public NativeHandleLease AcquireNativeHandle(NativeHandleAccess access = NativeHandleAccess.Feature) {
            ObjectDisposedException.ThrowIf(Volatile.Read(ref _closed) != 0, nameof(TestOwner));
            return new NativeHandleLease(_handle);
        }

        public void Dispose() {
            Volatile.Write(ref _closed, 1);
            NativeWindowHandleRegistry.Unregister(value, this);
            _handle.Dispose();
        }
    }

    [Test]
    public async Task ConcurrentAcquireAndShutdown_NeverReturnsAStaleHandle(CancellationToken ct = default) {
        IntPtr value = new(0x123456);
        using var owner = new TestOwner(value);
        NativeWindowHandleRegistry.Register(value, owner);
        int successfulAcquisitions = 0;

        Task[] workers = Enumerable.Range(0, Math.Max(4, Environment.ProcessorCount))
            .Select(_ => Task.Run(() => {
                for (int i = 0; i < 2_000; i++) {
                    try {
                        using NativeHandleLease lease = NativeWindowHandleRegistry.Acquire(value);
                        if (lease.Handle != value) throw new InvalidOperationException("Stale handle acquired.");
                        Interlocked.Increment(ref successfulAcquisitions);
                        Thread.Yield();
                    }
                    catch (ObjectDisposedException) {
                        return;
                    }
                }
            }, ct)).ToArray();

        await Task.Yield();
        // ReSharper disable once DisposeOnUsingVariable
        owner.Dispose();
        await Task.WhenAll(workers);

        await Assert.That(successfulAcquisitions).IsGreaterThanOrEqualTo(0);
        await Assert.That(() => NativeWindowHandleRegistry.Acquire(value)).Throws<ObjectDisposedException>();
    }

    [Test]
    public async Task DisposingSafeHandle_WaitsForOutstandingLease(CancellationToken ct = default) {
        IntPtr value = new(0x654321);
        var handle = new NativeWindowHandle(value, ownsHandle: false);
        var lease = new NativeHandleLease(handle);

        handle.Dispose();
        await Assert.That(lease.Handle).IsEqualTo(value);

        lease.Dispose();
        await Assert.That(handle.IsClosed).IsTrue();
    }
}
