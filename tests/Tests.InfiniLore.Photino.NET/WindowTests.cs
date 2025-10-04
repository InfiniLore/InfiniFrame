using InfiniLore.Photino.NET;

namespace Tests.InfiniLore.Photino.NET;

public class WindowTests {
    public IPhotinoWindow Window { get; set; } = null!;

    [Before(Test)]
    public void Setup() {
        var builder = PhotinoWindowBuilder.Create();

        builder.SetStartUrl("https://localhost/");
        
        Window = builder.Build();
    }

    private void InitializeWindow() {
        _ = Task.Run(Window.WaitForClose);
        
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task InstanceHandle_IsDefined() {
        // Arrange
        InitializeWindow();
        
        // Act

        // Assert
        await Assert.That(Window.InstanceHandle).IsNotDefault();
    }
    
    [Test]
    public async Task NativeType_IsDefined() {
        // Arrange
        InitializeWindow();
        
        // Act

        // Assert
        await Assert.That(Window.NativeType).IsNotDefault();
    }
    
    [Test]
    public async Task Maximize_IsDefined() {
        // Arrange
        InitializeWindow();
        
        // Act
        Window.SetMaximized(true);

        // Assert
        await Assert.That(Window.Maximized).IsTrue();
    }
}
