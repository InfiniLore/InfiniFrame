using InfiniLore.Photino.NET;

namespace Tests.InfiniLore.Photino.NET;

public class WindowTests {
    // -----------------------------------------------------------------------------------------------------------------
    // Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [NotInParallel(ParallellControl.Photino)]
    public async Task InstanceHandle_IsDefined() {
        // Arrange
        using var windowUtility = WindowStateUtility.Create();
        IPhotinoWindow window = windowUtility.Window;
        
        // Act

        // Assert
        await Assert.That(window.InstanceHandle).IsNotDefault();
    }
    
    [Test]
    [NotInParallel(ParallellControl.Photino)]
    public async Task NativeType_IsDefined() {
        // Arrange
        using var windowUtility = WindowStateUtility.Create();
        IPhotinoWindow window = windowUtility.Window;
        
        // Act

        // Assert
        await Assert.That(window.NativeType).IsNotDefault();
    }
    
    [Test]
    [NotInParallel(ParallellControl.Photino)]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Maximize_IsDefined(bool state) {
        // Arrange
        using var windowUtility = WindowStateUtility.Create();
        IPhotinoWindow window = windowUtility.Window;
        
        // Act
        window.SetMaximized(state);

        // Assert
        await Assert.That(window.Maximized).IsEqualTo(state);
    }
    
    [Test]
    [NotInParallel(ParallellControl.Photino)]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Minimize_IsDefined(bool state) {
        // Arrange
        using var windowUtility = WindowStateUtility.Create();
        IPhotinoWindow window = windowUtility.Window;
        
        // Act
        window.SetMinimized(state);

        // Assert
        await Assert.That(window.Minimized).IsEqualTo(state);
    }
}
