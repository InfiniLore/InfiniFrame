// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.State;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Regression tests for spurious initial window-state events (migration requirement):
///     a newly created window must not report itself minimized from the initial
///     unmapped/undecorated state, and must not raise a focus-out before the first
///     focus-in (the GTK4 initial active-state notification used to fabricate one).
/// </summary>
public class InitialStateEventRegressionTests {

    [Test]
    [NotInParallelInfiniTests]
    [DefaultTimeout(60_000)]
    public async Task Startup_RaisesNoFalseMinimizedAndNoLeadingFocusOut(CancellationToken ct = default) {
        // Arrange: track every state/focus notification from native startup.
        if (!OperatingSystem.IsLinux()) return; // Linux migration regression tests
        int focusInCount = 0;
        int focusOutCount = 0;
        int focusOutBeforeFirstIn = 0;
        int minimizedCount = 0;

        // Act
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => builder
            .RegisterFocusInHandler(_ => Interlocked.Increment(ref focusInCount))
            .RegisterFocusOutHandler(_ => {
                if (Volatile.Read(ref focusInCount) == 0) Interlocked.Increment(ref focusOutBeforeFirstIn);
                Interlocked.Increment(ref focusOutCount);
            })
            .RegisterMinimizedHandler(_ => Interlocked.Increment(ref minimizedCount)), ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Let the first map / activation notification window elapse.
        await Task.Delay(1500, ct);

        // Assert: no fabricated minimized state and no focus-out preceding the first
        // focus-in. (Whether the environment ever activates the window at all is
        // compositor policy - only the ordering and the false-minimized contract are
        // asserted.)
        await Assert.That(Volatile.Read(ref minimizedCount)).IsEqualTo(0);
        await Assert.That(Volatile.Read(ref focusOutBeforeFirstIn)).IsEqualTo(0);
        await Assert.That(window.Features.State.IsMinimized).IsFalse();
        await Assert.That(window.IsClosedOrClosing()).IsFalse();
    }

    [Test]
    [NotInParallelInfiniTests]
    [DefaultTimeout(60_000)]
    public async Task Startup_MinimizedFlagFalse_AcrossRepeatedCreation(CancellationToken ct = default) {
        // Arrange + Act: the false-minimized regression is timing sensitive; repeat
        // window creation to cover map-race variations.
        if (!OperatingSystem.IsLinux()) return;
        const int cycles = 5;

        for (int i = 0; i < cycles; i++) {
            ct.ThrowIfCancellationRequested();
            int minimizedCount = 0;
            using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => builder
                .RegisterMinimizedHandler(_ => Interlocked.Increment(ref minimizedCount)), ct);
            await Task.Delay(300, ct);
            await Assert.That(Volatile.Read(ref minimizedCount)).IsEqualTo(0);
            await Assert.That(windowUtility.Window.Features.State.IsMinimized).IsFalse();
        }
    }
}
