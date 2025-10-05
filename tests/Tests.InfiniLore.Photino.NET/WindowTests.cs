using InfiniLore.Photino.NET;

namespace Tests.InfiniLore.Photino.NET;

public class WindowTests {
    // -----------------------------------------------------------------------------------------------------------------
    // Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task InstanceHandle_IsDefined() {
        // Arrange
        using var windowUtility = WindowStateUtility.Create();
        IPhotinoWindow window = windowUtility.Window;
        
        // Act

        // Assert
        await Assert.That(window.InstanceHandle).IsNotDefault();
    }
    
    [Test]
    public async Task NativeType_IsDefined() {
        // Arrange
        using var windowUtility = WindowStateUtility.Create();
        IPhotinoWindow window = windowUtility.Window;
        
        // Act

        // Assert
        await Assert.That(window.NativeType).IsNotDefault();
    }
    
    [Test]
    public async Task Maximize_IsDefined() {
        // Arrange
        using var windowUtility = WindowStateUtility.Create();
        IPhotinoWindow window = windowUtility.Window;
        
        // Act & Assert
        window.SetMaximized(true);
        await Assert.That(window.Maximized).IsTrue();
        
        window.SetMaximized(false);
        await Assert.That(window.Maximized).IsFalse();
    }
    
    [Test]
    public async Task Minimize_IsDefined() {
        // Arrange
        using var windowUtility = WindowStateUtility.Create();
        IPhotinoWindow window = windowUtility.Window;
        
        // Act & Assert
        window.SetMinimized(true);
        await Assert.That(window.Minimized).IsTrue();
        
        window.SetMinimized(false);
        await Assert.That(window.Minimized).IsFalse();
    }
}
