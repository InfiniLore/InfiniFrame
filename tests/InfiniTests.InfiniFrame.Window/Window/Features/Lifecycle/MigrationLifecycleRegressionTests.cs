// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Lifecycle;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Focused regression tests for GTK4/WebKitGTK 6.0 migration-specific lifecycle behavior
///     (Phase 09): teardown completion exactly-once, window churn survival, and SetSize
///     reliability when it races the first map (see 07-lifecycle-teardown-report.md).
/// </summary>
public class MigrationLifecycleRegressionTests {
    private static async Task<int> WaitUntilEqualAsync(
        Func<int> getValue,
        int target,
        TimeSpan timeout,
        CancellationToken ct
        ) {
        DateTime deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline) {
            ct.ThrowIfCancellationRequested();
            int current = getValue();
            if (current == target) {
                return current;
            }
            await Task.Delay(25, ct);
        }
        return getValue();
    }

    [Test]
    [NotInParallelInfiniTests]
    [DefaultTimeout(120_000)]
    public async Task RepeatedCreateCloseCycles_KeepSubsequentWindowsHealthy(CancellationToken ct = default) {
        // Arrange: rapid create/close churn is the exact pattern that surfaced the
        // deferred-teardown use-after-free and the UI-thread restart SIGABRT (Phase 07).
        const int cycles = 10;

        // Act
        for (int i = 0; i < cycles; i++) {
            ct.ThrowIfCancellationRequested();
            using var windowUtility = InfiniFrameTestWindow.Create(ct);
            windowUtility.Window.Close();
            windowUtility.Window.WaitForClose();
        }

        // Assert: a window created after the churn is fully functional (its size
        // round-trips through the native layer).
        using var finalUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = finalUtility.Window;
        int originalWidth = window.Features.Size.Width;
        int targetWidth = originalWidth == 640 ? 660 : 640;
        window.Features.Size.SetSize(targetWidth, 480);
        int updatedWidth = await PollUtility.WaitForChangeAsync(
            getValue: () => window.Features.Size.Width, originalWidth, TimeSpan.FromSeconds(10), ct);
        await Assert.That(updatedWidth).IsEqualTo(targetWidth);
    }

    [Test]
    [NotInParallelInfiniTests]
    [DefaultTimeout(120_000)]
    public async Task SetSize_IssuedInRapidSuccession_FinalTargetLands(CancellationToken ct = default) {
        // Arrange: SetSize calls that race the first map/size negotiation used to be
        // silently clobbered (GTK4). Three attempts amplify the race window.
        const int attempts = 3;

        for (int attempt = 0; attempt < attempts; attempt++) {
            ct.ThrowIfCancellationRequested();
            using var windowUtility = InfiniFrameTestWindow.Create(ct);
            IInfiniFrameWindow window = windowUtility.Window;

            // Act: immediate back-to-back resizes; the final target must win.
            window.Features.Size.SetSize(620, 440);
            window.Features.Size.SetSize(720, 460);
            window.Features.Size.SetSize(820, 480);

            // Assert
            int finalWidth = await WaitUntilEqualAsync(
                () => window.Features.Size.Width, 820, TimeSpan.FromSeconds(10), ct);
            int finalHeight = await WaitUntilEqualAsync(
                () => window.Features.Size.Height, 480, TimeSpan.FromSeconds(10), ct);
            await Assert.That(finalWidth).IsEqualTo(820);
            await Assert.That(finalHeight).IsEqualTo(480);
        }
    }

    [Test]
    [NotInParallelInfiniTests]
    [DefaultTimeout(120_000)]
    public async Task RapidClose_MultipleWindows_RaisesClosedExactlyOnceEach(CancellationToken ct = default) {
        // Arrange: the deferred-teardown completion must run exactly once per window
        // regardless of how closes interleave (Phase 07 TeardownGuard contract).
        const int windowCount = 4;
        int closedCount = 0;
        var utilities = new List<InfiniFrameTestWindow>();
        try {
            for (int i = 0; i < windowCount; i++) {
                utilities.Add(InfiniFrameTestWindow.Create(
                    builder: builder => builder.RegisterWindowClosedHandler(
                        // ReSharper disable once AccessToModifiedClosure
                        _ => Interlocked.Increment(ref closedCount)), ct));
            }

            // Act: close all windows back-to-back without waiting in between.
            foreach (InfiniFrameTestWindow utility in utilities) {
                utility.Window.Close();
            }
            foreach (InfiniFrameTestWindow utility in utilities) {
                utility.Window.WaitForClose();
            }

            // Assert: exactly one closed notification per window — a late duplicate
            // notification would exceed the count after the settle delay.
            int observed = await WaitUntilEqualAsync(() => closedCount, windowCount, TimeSpan.FromSeconds(10), ct);
            await Assert.That(observed).IsEqualTo(windowCount);
            await Task.Delay(500, ct);
            await Assert.That(closedCount).IsEqualTo(windowCount);
        } finally {
            foreach (InfiniFrameTestWindow utility in utilities) {
                utility.Dispose();
            }
        }
    }
}
