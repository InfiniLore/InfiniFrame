using InfiniLore.InfiniFrame;
using System.Collections.Immutable;
using Tests.Shared;
using Monitor=InfiniLore.InfiniFrame.Monitor;

namespace Tests.InfiniFrame;

public class WindowTests {
    // -----------------------------------------------------------------------------------------------------------------
    // Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task InstanceHandle_IsDefined() {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;
        
        // Act

        // Assert
        await Assert.That(window.InstanceHandle).IsNotDefault();
    }

    [Test]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task WindowHandle_IsDefined() {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;
        
        // Act
        IntPtr handle = window.WindowHandle;

        // Assert
        if (OperatingSystem.IsWindows()) await Assert.That(handle).IsNotDefault();
        else await Assert.That(handle).IsEqualTo(IntPtr.Zero);
    }

    [Test]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task Monitors_IsNotEmpty() {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;
        
        // Act
        ImmutableArray<Monitor> monitors = window.Monitors;

        // Assert
        await Assert.That(monitors).IsNotEmpty();
    }
    
    [Test]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task NativeType_IsDefined() {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;
        
        // Act

        // Assert
        await Assert.That(window.NativeType).IsNotDefault();
    }
    
    [Test]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task Close_IsDefined() {
        // Arrange
        var windowClosingTcs = new TaskCompletionSource<bool>();
        var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder.Events.WindowClosingRequested += (_, _) => {
                windowClosingTcs.SetResult(true);
            }
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
