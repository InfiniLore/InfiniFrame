// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Native;
using System.Runtime.InteropServices;

namespace InfiniFrameTests;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameNativeExportGuardTests {
    private const int InvalidArgument = 22;

    [Test]
    public async Task NullWindowHandle_ReturnsSafeDefaultsAndSetsLastError() {
        // Act
        InfiniFrameNative.GetSize(IntPtr.Zero, out int width, out int height);
        int lastError = Marshal.GetLastPInvokeError();
        IntPtr title = InfiniFrameNative.GetTitle(IntPtr.Zero);
        int titleLastError = Marshal.GetLastPInvokeError();

        // Assert
        await Assert.That(width).IsEqualTo(0);
        await Assert.That(height).IsEqualTo(0);
        await Assert.That(lastError).IsEqualTo(InvalidArgument);
        await Assert.That(title).IsEqualTo(IntPtr.Zero);
        await Assert.That(titleLastError).IsEqualTo(InvalidArgument);
    }

    [Test]
    public async Task PtrToNativeStringAndFree_ReturnsNullForZeroPointer() {
        // Act
        string? value = InfiniFrameNative.PtrToNativeStringAndFree(IntPtr.Zero);

        // Assert
        await Assert.That(value).IsNull();
    }

    [Test]
    public async Task SuccessfulNoOpExport_ClearsPreviousLastError() {
        // Arrange
        _ = InfiniFrameNative.GetTitle(IntPtr.Zero);
        await Assert.That(Marshal.GetLastPInvokeError()).IsEqualTo(InvalidArgument);

        // Act
        InfiniFrameNativeStatusCode status = InfiniFrameNative.FreeString(IntPtr.Zero);

        // Assert
        await Assert.That(status).IsEqualTo(InfiniFrameNativeStatusCode.Success);
        await Assert.That(Marshal.GetLastPInvokeError()).IsEqualTo(0);
    }

    [Test]
    public async Task StatusExport_WithNullWindow_ReturnsInvalidArgumentAndSetsLastError() {
        // Act
        InfiniFrameNativeStatusCode status = InfiniFrameNative.Center(IntPtr.Zero);

        // Assert
        await Assert.That(status).IsEqualTo(InfiniFrameNativeStatusCode.InvalidArgument);
        await Assert.That(Marshal.GetLastPInvokeError()).IsEqualTo(InvalidArgument);
    }

    [Test]
    public async Task Constructor_WithInvalidInitParameterSize_ReturnsNullAndSetsInvalidArgument() {
        // Arrange
        var parameters = new InfiniFrameNativeParameters {
            StartString = "<html></html>",
            Size = 1
        };

        // Act
        IntPtr instance = InfiniFrameNative.Constructor(in parameters);

        // Assert
        await Assert.That(instance).IsEqualTo(IntPtr.Zero);
        await Assert.That(Marshal.GetLastPInvokeError()).IsEqualTo(InvalidArgument);
    }

    [Test]
    public async Task Constructor_WithInvalidInitParameterSize_PreservesNativeErrorMessage() {
        // Arrange
        var parameters = new InfiniFrameNativeParameters {
            StartString = "<html></html>",
            Size = 1
        };

        // Act
        _ = InfiniFrameNative.Constructor(in parameters);
        string? message = InfiniFrameNative.GetLastErrorMessageAndFree();

        // Assert
        await Assert.That(message).Contains("Initial parameters passed are 1 bytes");
    }
}
