// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniTests.Native;

namespace InfiniTests.InfiniFrame.Window.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ParentChildWindowTests {
    private const uint GwOwner = 4;

    [Test]
    [SkipOnWindowsArm]
    // Rider can schedule the net8 test host last while all target frameworks are cold-starting WebView2.
    // This integration test creates two native browser windows, so use a 30-second total budget.
    [DefaultInfiniTestsTimeout(20_000)]
    [NotInParallelInfiniTests]
    public async Task AtBuilderStage_AssignsParentWindowAndNativeParentHandle(CancellationToken ct = default) {
        // Arrange
        using var parentWindowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow parentWindow = parentWindowUtility.Window;

        // Act
        using var childWindowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            var configuration = (InfiniFrameWindowBuilderConfiguration)builder.Configuration;
            configuration.ParentWindow = parentWindow;
        }, ct);
        IInfiniFrameWindow childWindow = childWindowUtility.Window;

        // Assert
        await Assert.That(childWindow.Configuration.ParentWindow).IsEqualTo(parentWindow);
        // NativeParent is deliberately transient: it is supplied under a parent-handle lease
        // during construction and is not retained as a stale pointer in managed configuration.
        await Assert.That(childWindow.Configuration.StartupParameters.NativeParent).IsEqualTo(IntPtr.Zero);
    }

    [Test]
    [SkipOnWindowsArm]
    [DefaultInfiniTestsTimeout(6_000)]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_ClosingParent_ClosesChildWindow(CancellationToken ct = default) {
        // Arrange
        using var parentWindowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow parentWindow = parentWindowUtility.Window;
        using var childWindowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            var configuration = (InfiniFrameWindowBuilderConfiguration)builder.Configuration;
            configuration.ParentWindow = parentWindow;
        }, ct);
        IInfiniFrameWindow childWindow = childWindowUtility.Window;
        lock (parentWindow.Configuration.ChildWindows) {
            parentWindow.Configuration.ChildWindows.Add(childWindow);
        }

        // Act
        parentWindow.Close();
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (!childWindow.IsClosedOrClosing() && DateTime.UtcNow < timeoutAt) {
            await Task.Delay(50, ct);
        }

        // Assert
        await Assert.That(childWindow.IsClosedOrClosing()).IsTrue();
    }

    [Test]
    [OnlyRunOnMacOs]
    [NotInParallelInfiniTests]
    public async Task OnMacOs_ClosingChild_DetachesPooledHostFromParent(CancellationToken ct = default) {
        using var parentUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow parent = parentUtility.Window;
        IntPtr childHost;
        using (var childUtility = InfiniFrameTestWindow.Create(builder => {
            ((InfiniFrameWindowBuilderConfiguration)builder.Configuration).ParentWindow = parent;
        }, ct)) {
            childHost = childUtility.Window.WindowHandle;
            childUtility.Window.Close();
            childUtility.Window.WaitForClose();
        }

        parent.Close();
        parent.WaitForClose();
        using var replacement = InfiniFrameTestWindow.Create(ct);
        await Assert.That(replacement.Window.WindowHandle).IsEqualTo(childHost);
        await Assert.That(replacement.Window.IsClosedOrClosing()).IsFalse();
    }

    [Test]
    [OnlyRunOnWindowsX64]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_OnWindows_ChildWindowOwnerMatchesParentWindowHandle(CancellationToken ct = default) {
        // Arrange
        using var parentWindowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow parentWindow = parentWindowUtility.Window;
        using var childWindowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            var configuration = (InfiniFrameWindowBuilderConfiguration)builder.Configuration;
            configuration.ParentWindow = parentWindow;
        }, ct);
        IInfiniFrameWindow childWindow = childWindowUtility.Window;

        // Act
        IntPtr ownerWindow = IntPtr.Zero;
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (DateTime.UtcNow < timeoutAt) {
            ownerWindow = WindowsNative.GetRelatedWindow(childWindow.WindowHandle, GwOwner);
            if (ownerWindow == parentWindow.WindowHandle) {
                break;
            }

            await Task.Delay(50, ct);
        }

        // Assert
        await Assert.That(ownerWindow).IsEqualTo(parentWindow.WindowHandle);
    }
}