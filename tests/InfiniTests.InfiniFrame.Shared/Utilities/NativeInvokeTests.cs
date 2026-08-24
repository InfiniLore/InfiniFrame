// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;
using InfiniFrame.NativeBridge;
using InfiniFrame.NativeBridge.Handles;
using Microsoft.Extensions.Logging.Abstractions;

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
            Callback);

        // Assert
        await Assert.That(result).IsEqualTo("out-value");
    }
    private static InfiniFrameNativeInteropStatus Callback(IntPtr _, out string value) {
        value = "out-value";
        return InfiniFrameNativeInteropStatus.Success;
    }

    [Test]
    public async Task InvokeWithValidation_FuncWithOut_PassesLeasedHandleToCallback(CancellationToken ct = default) {
        // Arrange
        IntPtr expectedHandle = new(99999);
        var owner = new TestHandleOwner(expectedHandle);
        IntPtr received = IntPtr.Zero;

        // Act
        InfiniFrameNativeInteropStatus FuncWithOut(IntPtr h, out int v) {
            received = h;
            v = 0;
            return InfiniFrameNativeInteropStatus.Success;
        }

        NativeInvoke.InvokeSyncWithValidation<int>(
            NullLogger.Instance,
            owner,
            Environment.CurrentManagedThreadId,
            FuncWithOut);

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
