using InfiniLore.Photino.NET;

namespace Tests.InfiniLore.Photino.NET;
using Tests.InfiniLore.Photino.NET.TestUtilities;

public class WindowTests {
    // -----------------------------------------------------------------------------------------------------------------
    // Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
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
    [NotInParallel(ParallelControl.Photino)]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Maximize_IsDefined(bool state) {
        SkipUtilities.SkipOnLinux(state);
        
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;
        
        // Act
        window.SetMaximized(state);

        // Assert
        await Assert.That(window.Maximized).IsEqualTo(state);
    }
    
    [Test]
    [NotInParallel(ParallelControl.Photino)]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Minimize_IsDefined(bool state) {
        SkipUtilities.SkipOnLinux(state);
        
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;
        
        // Act
        window.SetMinimized(state);

        // Assert
        await Assert.That(window.Minimized).IsEqualTo(state);
    }
    
    
    [Test]
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
    
    [Test]
    [NotInParallel(ParallelControl.Photino)]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Fullscreen_IsDefined(bool state) {
        // SkipUtilities.SkipOnLinux(state);
        
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;
        
        // Act
        window.SetFullScreen(state);

        // Assert
        await Assert.That(window.FullScreen).IsEqualTo(state);
    }

    [Test]
    public async Task IconFilePath_IsDefined() {
        const string iconFilePath = "Assets/favicon.ico";
        
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;

        // Act
        window.SetIconFile(iconFilePath);

        // Assert
        await Assert.That(window.IconFilePath).IsEqualTo(iconFilePath);
    }
}
