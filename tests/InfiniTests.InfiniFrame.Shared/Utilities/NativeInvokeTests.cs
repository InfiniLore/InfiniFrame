// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using InfiniFrame.NativeBridge.Handles;
using Microsoft.Extensions.Logging.Abstractions;
using System.Runtime.InteropServices;

namespace InfiniTests.InfiniFrame.Shared.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class NativeInvokeTests {

    // -----------------------------------------------------------------------------------------------------------------
    // InvokeWithValidation<T>(window, FuncWithOut<T>)
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task InvokeWithValidation_FuncWithOut_ReturnsValueSetViaOutParameter(CancellationToken ct = default) {
        // Arrange
        var owner = new TestHandleOwner(123456);

        // Act
        string? result = NativeInvoke.InvokeSyncWithValidation<string>(
            NullLogger.Instance,
            owner,
            Environment.CurrentManagedThreadId,
            callback: (_, out value) => {
                value = "out-value";
                return InfiniFrameNativeInteropStatus.Success;
            });

        // Assert
        await Assert.That(result).IsEqualTo("out-value");
    }

    [Test]
    public async Task InvokeWithValidation_FuncWithOut_PassesLeasedHandleToCallback(CancellationToken ct = default) {
        // Arrange
        IntPtr expectedHandle = new(99999);
        var owner = new TestHandleOwner(expectedHandle);
        IntPtr received = IntPtr.Zero;

        // Act
        NativeInvoke.InvokeSyncWithValidation<int>(
            NullLogger.Instance,
            owner,
            Environment.CurrentManagedThreadId,
            callback: (h, out v) => {
                received = h;
                v = 0;
                return InfiniFrameNativeInteropStatus.Success;
            });

        // Assert
        await Assert.That(received).IsEqualTo(expectedHandle);
    }

    [Test]
    public async Task InvokeWithValidation_Success_IgnoresAndClearsStaleLastError(CancellationToken ct = default) {
        var owner = new TestHandleOwner(123456);

        NativeInvoke.InvokeSyncWithValidation(
            NullLogger.Instance,
            owner,
            Environment.CurrentManagedThreadId,
            callback: () => {
                Marshal.SetLastPInvokeError(203);
                return InfiniFrameNativeInteropStatus.Success;
            });

        await Assert.That(Marshal.GetLastPInvokeError()).IsEqualTo(0);
    }

    private sealed class TestHandleOwner(IntPtr value) : INativeWindowHandleOwner {
        private readonly NativeWindowHandle _handle = new(value, false);

        public NativeHandleLease AcquireNativeHandle(NativeHandleAccess access = NativeHandleAccess.Feature) => new(_handle);
    }
}