// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;
using InfiniFrame;
using InfiniTests.Native;

namespace InfiniTests.InfiniFrame.Window.Features.Decorations;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// --------------------------------------------------------------------------------------------------------------------
public sealed class WindowsAppUserModelIdTests {
    [Test]
    public async Task DirectAssignment_PassesValueToNativeParameters() {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();
        const string value = "InfiniLore.InfiniFrame.Tests";

        // Act
        builder.Features.Decorations.SetWindowsAppUserModelId(value);

        // Assert — WindowsAppUserModelId is now an application-level setting, stored on the builder feature.
        await Assert.That(builder.Features.Decorations.WindowsAppUserModelId).IsEqualTo(value);
    }

    [Test]
    public async Task ExtensionAssignment_ReturnsSameBuilderAndPassesValueToNativeParameters() {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();
        const string value = "InfiniLore.InfiniFrame.Tests";

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetWindowsAppUserModelId(value);

        // Assert — WindowsAppUserModelId is now an application-level setting, no longer set on window parameters.
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
    }

    [Test]
    [OnlyRunOnWindowsX64]
    [NotInParallelInfiniTests]
    public async Task WindowCreation_AssignsExplicitProcessIdentity(CancellationToken ct) {
        const string value = "InfiniLore.InfiniFrame.Tests";

        // If another test already initialized the application, skip — we can't reinitialize.
        if (InfiniFrameApplication.Instance?.ApplicationHandle != IntPtr.Zero)
            return;

        // Initialize application with the specific config (WindowsAppUserModelId).
        InfiniFrameApplication app = InfiniFrameApplication.Initialize(config => {
            config.WindowsAppUserModelId = value;
        });

        int result = WindowsNative.GetCurrentProcessAppUserModelId(out IntPtr appUserModelId);
        try {
            await Assert.That(result).IsEqualTo(0);
            await Assert.That(Marshal.PtrToStringUni(appUserModelId)).IsEqualTo(value);
        }
        finally {
            Marshal.FreeCoTaskMem(appUserModelId);
        }
    }
}
