// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using System.Diagnostics.CodeAnalysis;

namespace InfiniTests.InfiniFrame.Window.Features.InfiniFrameWindowFeatureLifecycle;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CloseTests {
    [Test]
    [Retry(5)]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(1_000)]
    [SuppressMessage("ReSharper", "MethodSupportsCancellation")]
    // Sometimes fails on CI due to timing issues
    public async Task AtWindowStage_ExtensionAssignment(CancellationToken ct = default) {
        // Arrange
        var windowClosingTcs = new TaskCompletionSource<bool>();
        using var windowUtility = InfiniFrameTestWindow.Create(
            builder: builder => builder.EventsStore.WindowClosingRequested.Add(_ => {
                windowClosingTcs.TrySetResult(true);
            }),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Close();
        await Task.Delay(1_000, ct);

        // Assert
        bool windowClosing = await windowClosingTcs.Task.WaitAsync(TimeSpan.FromSeconds(1));
        await Assert.That(windowClosing).IsTrue();
    }

    [Test]
    [Retry(5)]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(2_000)]
    public async Task AtWindowStage_DirectAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        await window.Features.Lifecycle.CloseAsync(ct);

        // Assert
        await Assert.That(window.Features.Lifecycle.IsClosedOrClosing()).IsTrue();
    }
}
