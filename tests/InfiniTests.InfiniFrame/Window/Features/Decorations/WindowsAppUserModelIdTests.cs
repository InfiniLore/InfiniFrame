// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;
using InfiniTests.Native;

namespace InfiniTests.InfiniFrame.Window.Features.Decorations;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class WindowsAppUserModelIdTests {
    [Test]
    public async Task DirectAssignment_PassesValueToNativeParameters() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        const string value = "InfiniLore.InfiniFrame.Tests";

        // Act
#pragma warning disable CS0618 // Type or member is obsolete
        builder.Features.Decorations.SetWindowsAppUserModelId(value);
#pragma warning restore CS0618
        InfiniFrameNativeParameters parameters = builder.CollectNativeParameters();

        // Assert — WindowsAppUserModelId is now an application-level setting, no longer set on window parameters.
        await Assert.That(builder.Features.Decorations.WindowsAppUserModelId).IsEqualTo(value);
    }

    [Test]
    public async Task ExtensionAssignment_ReturnsSameBuilderAndPassesValueToNativeParameters() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        const string value = "InfiniLore.InfiniFrame.Tests";

        // Act
#pragma warning disable CS0618 // Type or member is obsolete
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetWindowsAppUserModelId(value);
#pragma warning restore CS0618
        InfiniFrameNativeParameters parameters = builder.CollectNativeParameters();

        // Assert — WindowsAppUserModelId is now an application-level setting, no longer set on window parameters.
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(parameters.WindowsAppUserModelId).IsNull();
    }

    [Test]
    [OnlyRunOnWindowsX64]
    [NotInParallelInfiniTests]
    public async Task WindowCreation_AssignsExplicitProcessIdentity(CancellationToken ct) {
        const string value = "InfiniLore.InfiniFrame.Tests";

        using var window = InfiniFrameTestWindow.Create(builder: builder => {
#pragma warning disable CS0618 // Type or member is obsolete
            builder.SetWindowsAppUserModelId(value);
#pragma warning restore CS0618
        }, ct);

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
