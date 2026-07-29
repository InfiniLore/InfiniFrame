// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;
using InfiniTests.Native;
using System.Runtime.InteropServices;

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
        builder.Features.Decorations.SetWindowsAppUserModelId(value);
        InfiniFrameNativeParameters parameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Decorations.WindowsAppUserModelId).IsEqualTo(value);
        await Assert.That(parameters.WindowsAppUserModelId).IsEqualTo(value);
    }

    [Test]
    public async Task ExtensionAssignment_ReturnsSameBuilderAndPassesValueToNativeParameters() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        const string value = "InfiniLore.InfiniFrame.Tests";

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetWindowsAppUserModelId(value);
        InfiniFrameNativeParameters parameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(parameters.WindowsAppUserModelId).IsEqualTo(value);
    }

    [Test]
    [OnlyRunOnWindowsX64]
    [NotInParallelInfiniTests]
    public async Task WindowCreation_AssignsExplicitProcessIdentity(CancellationToken ct) {
        const string value = "InfiniLore.InfiniFrame.Tests";

        using var window = InfiniFrameTestWindow.Create(builder: builder => builder.SetWindowsAppUserModelId(value), ct);

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
