// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Shared;
using System.Collections.Immutable;

namespace InfiniFrameTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowTests {
    // -----------------------------------------------------------------------------------------------------------------
    // Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [DisplayName($"{nameof(WindowTests)}.{nameof(InstanceHandle_IsDefined)}")]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(TimeoutUtlitity.DefaultTimeout)]
    public async Task InstanceHandle_IsDefined(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act

        // Assert
        await Assert.That(window.InstanceHandle).IsNotDefault();
    }

    [Test]
    [DisplayName($"{nameof(WindowTests)}.{nameof(WindowHandle_IsDefined)}")]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(TimeoutUtlitity.DefaultTimeout)]
    public async Task WindowHandle_IsDefined(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        IntPtr handle = window.WindowHandle;

        // Assert
        if (OperatingSystem.IsWindows()) await Assert.That(handle).IsNotDefault();
        else await Assert.That(handle).IsEqualTo(IntPtr.Zero);
    }

    [Test]
    [DisplayName($"{nameof(WindowTests)}.{nameof(Monitors_IsNotEmpty)}")]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(TimeoutUtlitity.DefaultTimeout)]
    public async Task Monitors_IsNotEmpty(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        ImmutableArray<InfiniMonitor> monitors = window.Monitors;

        // Assert
        await Assert.That(monitors).IsNotEmpty();
    }

    [Test]
    [DisplayName($"{nameof(WindowTests)}.{nameof(NativeType_IsDefined)}")]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(TimeoutUtlitity.DefaultTimeout)]
    public async Task NativeType_IsDefined(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act

        // Assert
        await Assert.That(window.NativeType).IsNotDefault();
    }

    [Test]
    [DisplayName($"{nameof(WindowTests)}.{nameof(Close_IsDefined)}")]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(TimeoutUtlitity.DefaultTimeout)]
    public async Task Close_IsDefined(CancellationToken ct) {
        // Arrange
        var windowClosingTcs = new TaskCompletionSource<bool>();
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder.Events.WindowClosingRequested.Add(_ => {
                windowClosingTcs.SetResult(true);
            }),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Close();
        await Task.Delay(100);

        // Assert
        bool windowClosing = await windowClosingTcs.Task.WaitAsync(TimeSpan.FromSeconds(1));
        await Assert.That(windowClosing).IsTrue();
    }
}
