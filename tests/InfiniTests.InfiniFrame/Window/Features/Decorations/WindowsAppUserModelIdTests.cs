// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;
using System.Runtime.InteropServices;

namespace InfiniTests.InfiniFrame.Window.Features.Decorations;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed partial class WindowsAppUserModelIdTests {
    // ReSharper disable once InconsistentNaming
    [LibraryImport("shell32.dll")]
    private static partial int GetCurrentProcessExplicitAppUserModelID(out IntPtr appUserModelId);

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

        int result = GetCurrentProcessExplicitAppUserModelID(out IntPtr appUserModelId);
        try {
            await Assert.That(result).IsEqualTo(0);
            await Assert.That(Marshal.PtrToStringUni(appUserModelId)).IsEqualTo(value);
        }
        finally {
            Marshal.FreeCoTaskMem(appUserModelId);
        }
    }
}
