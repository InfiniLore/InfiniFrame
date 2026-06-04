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
        bool result = InfiniFrameNativeTesting.IsColorSchemeChange(inputPtr);

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    [OnlyRunOnWindows]
    public async Task IsColorSchemeChange_ImmersiveColorSetPointer_ReturnsTrue(CancellationToken ct = default) {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

        IntPtr pointer = IntPtr.Zero;
        try {
            pointer = Marshal.StringToHGlobalUni("ImmersiveColorSet");

            bool result = InfiniFrameNativeTesting.IsColorSchemeChange(pointer);

            await Assert.That(result).IsTrue();
        }
        finally {
            if (pointer != IntPtr.Zero) Marshal.FreeHGlobal(pointer);
        }
    }
}
