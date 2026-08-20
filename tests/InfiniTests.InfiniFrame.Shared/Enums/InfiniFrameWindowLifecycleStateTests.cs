// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Enums;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowLifecycleStateTests {

    [Test]
    public async Task AllValues_AreDistinct(CancellationToken ct = default) {
        // Arrange
        var values = (InfiniFrameWindowLifecycleState[])Enum.GetValues(typeof(InfiniFrameWindowLifecycleState));

        // Act
        int distinctCount = values.Select(v => (int)v).Distinct().Count();

        // Assert
        await Assert.That(distinctCount).IsEqualTo(9);
    }

    [Test]
    public async Task Creating_EqualsInitializing(CancellationToken ct = default) {
        // Arrange
        var creating = InfiniFrameWindowLifecycleState.Creating;
        var initializing = InfiniFrameWindowLifecycleState.Initializing;

        // Act & Assert
        await Assert.That(creating).IsEqualTo(initializing);
    }

    [Test]
    public async Task Ready_EqualsRunning(CancellationToken ct = default) {
        // Arrange
        var ready = InfiniFrameWindowLifecycleState.Ready;
        var running = InfiniFrameWindowLifecycleState.Running;

        // Act & Assert
        await Assert.That(ready).IsEqualTo(running);
    }

    [Test]
    public async Task CloseRequested_EqualsClosingRequested(CancellationToken ct = default) {
        // Arrange
        var closeRequested = InfiniFrameWindowLifecycleState.CloseRequested;
        var closingRequested = InfiniFrameWindowLifecycleState.ClosingRequested;

        // Act & Assert
        await Assert.That(closeRequested).IsEqualTo(closingRequested);
    }

    [Test]
    public async Task States_IncreaseInOrder(CancellationToken ct = default) {
        // Arrange & Act
        int created = (int)InfiniFrameWindowLifecycleState.Created;
        int creating = (int)InfiniFrameWindowLifecycleState.Creating;
        int ready = (int)InfiniFrameWindowLifecycleState.Ready;
        int closeRequested = (int)InfiniFrameWindowLifecycleState.CloseRequested;
        int nativeClosed = (int)InfiniFrameWindowLifecycleState.NativeClosed;
        int teardownPending = (int)InfiniFrameWindowLifecycleState.TeardownPending;
        int teardownComplete = (int)InfiniFrameWindowLifecycleState.TeardownComplete;
        int nativeHandleReleased = (int)InfiniFrameWindowLifecycleState.NativeHandleReleased;
        int disposed = (int)InfiniFrameWindowLifecycleState.Disposed;

        // Assert
        await Assert.That(created).IsLessThan(creating);
        await Assert.That(creating).IsLessThan(ready);
        await Assert.That(ready).IsLessThan(closeRequested);
        await Assert.That(closeRequested).IsLessThan(nativeClosed);
        await Assert.That(nativeClosed).IsLessThan(teardownPending);
        await Assert.That(teardownPending).IsLessThan(teardownComplete);
        await Assert.That(teardownComplete).IsLessThan(nativeHandleReleased);
        await Assert.That(nativeHandleReleased).IsLessThan(disposed);
    }
}
