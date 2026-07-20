// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using System.Runtime.InteropServices;

namespace InfiniTests.InfiniFrame.NativeBridge.Managed.Windows;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ColorSchemeChangeTests {
    [Test]
    [OnlyRunOnWindows]
    [Arguments(0)]
    [Arguments(1)]
    public async Task IsColorSchemeChange_InvalidPointer_DoesNotCrashAndReturnsFalse(int input, CancellationToken ct = default) {
        // Arrange
        IntPtr inputPtr = input;

        // Act
        InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.IsColorSchemeChange(inputPtr, out bool result);

        // Assert
        await Assert.That(result).IsFalse();
        await Assert.That(status).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
    }

    [Test]
    [OnlyRunOnWindows]
    public async Task IsColorSchemeChange_ImmersiveColorSetPointer_ReturnsTrue(CancellationToken ct = default) {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

        IntPtr pointer = IntPtr.Zero;
        try {
            pointer = Marshal.StringToHGlobalUni("ImmersiveColorSet");

            InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.IsColorSchemeChange(pointer, out bool result);

            await Assert.That(result).IsTrue();
            await Assert.That(status).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
        }
        finally {
            if (pointer != IntPtr.Zero) Marshal.FreeHGlobal(pointer);
        }
    }
}
