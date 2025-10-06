using InfiniLore.Photino.NET;
using Tests.Photino.NET.TestUtilities;

namespace Tests.Photino.NET;

public class WindowTests {
    // -----------------------------------------------------------------------------------------------------------------
    // Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [SkipUtility.OnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    public async Task InstanceHandle_IsDefined() {
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;
        
        // Act

        // Assert
        await Assert.That(window.InstanceHandle).IsNotDefault();
    }
    
    [Test]
    [SkipUtility.OnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    public async Task NativeType_IsDefined() {
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;
        
        // Act

        // Assert
        await Assert.That(window.NativeType).IsNotDefault();
    }
    
    [Test]
    [SkipUtility.OnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    public async Task Close_IsDefined() {
        // SkipUtilities.SkipOnLinux();
        
        // Arrange
        var windowClosingTcs = new TaskCompletionSource<bool>();
        var windowUtility = WindowTestUtility.Create(
            builder => builder.Events.WindowClosingRequested += (_, _) => {
                windowClosingTcs.SetResult(true);
            }
        );
        IPhotinoWindow window = windowUtility.Window;
        
        // Act
        window.Close();
        await Task.Delay(100);

        // Assert
        bool windowClosing = await windowClosingTcs.Task.WaitAsync(TimeSpan.FromSeconds(1));
        await Assert.That(windowClosing).IsTrue();
    }
}
