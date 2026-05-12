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
    [Retry(5)]
    [TimeoutUtility.WithDefaultTimeout(6_000)]
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
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (!childWindow.IsClosed && DateTime.UtcNow < timeoutAt) {
            await Task.Delay(50, ct);
        }

        await Assert.That(childWindow.IsClosed).IsTrue();
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

        IntPtr ownerWindow = IntPtr.Zero;
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (DateTime.UtcNow < timeoutAt) {
            ownerWindow = GetWindow(childWindow.WindowHandle, GwOwner);
            if (ownerWindow == parentWindow.WindowHandle)
                break;

            await Task.Delay(50, ct);
        }

        await Assert.That(ownerWindow).IsEqualTo(parentWindow.WindowHandle);
    }
}
