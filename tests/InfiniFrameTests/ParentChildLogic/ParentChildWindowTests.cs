// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Shared;
using System.Runtime.InteropServices;

namespace InfiniFrameTests.ParentChildLogic;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ParentChildWindowTests {
    private const uint GwOwner = 4;

    [DllImport("user32.dll", EntryPoint = "GetWindow", SetLastError = true)]
    private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
    
    [Test]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task TestParentChildWindow(CancellationToken ct = default) {
        // Arrange
        using var parentWindowUtility = InfiniFrameWindowTestUtility.Create(ct);
        IInfiniFrameWindow parentWindow = parentWindowUtility.Window;

        // Act
        using var childWindowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder.SetParentWindow(parentWindow),
            ct
        );
        IInfiniFrameWindow childWindow = childWindowUtility.Window;

        // Assert
        await Assert.That(childWindow.Configuration.ParentWindow).IsEqualTo(parentWindow);
        await Assert.That(childWindow.Configuration.StartupParameters.NativeParent).IsEqualTo(parentWindow.InstanceHandle);
    }

    [Test]
    [SkipUtility.SkipOnMacOs]
    [TimeoutUtility.WithDefaultTimeout(1_000)]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task ClosingParent_ShouldCloseChildWindow(CancellationToken ct = default) {
        using var parentWindowUtility = InfiniFrameWindowTestUtility.Create(ct);
        IInfiniFrameWindow parentWindow = parentWindowUtility.Window;

        using var childWindowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder.SetParentWindow(parentWindow),
            ct
        );
        IInfiniFrameWindow childWindow = childWindowUtility.Window;

        parentWindow.Close();
        await Task.Delay(1_000, ct);

        await Assert.That(childWindow.InstanceHandle).IsEqualTo(IntPtr.Zero);
    }

    [Test]
    [SkipUtility.OnlyRunOnWindows]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task ChildWindow_ShouldHaveNativeOwnerWindow_OnWindows(CancellationToken ct = default) {
        using var parentWindowUtility = InfiniFrameWindowTestUtility.Create(ct);
        IInfiniFrameWindow parentWindow = parentWindowUtility.Window;

        using var childWindowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder.SetParentWindow(parentWindow),
            ct
        );
        IInfiniFrameWindow childWindow = childWindowUtility.Window;

        IntPtr ownerWindow = GetWindow(childWindow.WindowHandle, GwOwner);

        await Assert.That(ownerWindow).IsEqualTo(parentWindow.WindowHandle);
    }
}
