// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace InfiniTests.InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowTests {
    // -----------------------------------------------------------------------------------------------------------------
    // Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task InstanceHandle_IsDefined(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act

        // Assert
        await Assert.That(window.InstanceHandle).IsNotDefault();
    }

    [Test]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task WindowHandle_IsDefined(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        IntPtr handle = window.WindowHandle;

        // Assert
        if (OperatingSystem.IsWindows()) await Assert.That(handle).IsNotDefault();
        else await Assert.That(handle).IsEqualTo(IntPtr.Zero);
    }

    [Test]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task Monitors_IsNotEmpty(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        ImmutableArray<InfiniMonitor> monitors = window.Monitors;

        // Assert
        await Assert.That(monitors).IsNotEmpty();
    }

    [Test]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task NativeType_IsDefined(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act

        // Assert
        await Assert.That(window.NativeType).IsNotDefault();
    }

    [Test]
    [Retry(5)]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(1_000)]
    [SuppressMessage("ReSharper", "MethodSupportsCancellation")]
    // Sometimes fails on CI due to timing issues
    public async Task Close_IsDefined(CancellationToken ct = default) {
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
    [DefaultInfiniTestsTimeout(6_000)]
    [NotInParallelInfiniTests]
    public async Task IsClosed_TracksWindowState(CancellationToken ct = default) {
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        await Assert.That(window.IsClosed).IsFalse();

        window.Close();
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (!window.IsClosed && DateTime.UtcNow < timeoutAt) {
            await Task.Delay(50, ct);
        }

        await Assert.That(window.IsClosed).IsTrue();
    }
}
