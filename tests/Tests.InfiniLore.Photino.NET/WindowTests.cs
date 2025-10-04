using InfiniLore.Photino.NET;

namespace Tests.InfiniLore.Photino.NET;

public class WindowTests {
    public IPhotinoWindow Window { get; set; } = null!;

    [Before(Test)]
    public void Setup() {
        var builder = PhotinoWindowBuilder.Create();

        builder.SetStartUrl("https://localhost/");
        builder.SetSize(10, 10);
        
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
    [Arguments(true)]
    [Arguments(false)]
    public async Task Maximize_IsDefined(bool state) {
        // Arrange
        InitializeWindow();
        
        // Act
        Window.SetMaximized(state);

        // Assert
        await Assert.That(Window.Maximized).IsEqualTo(state);
    }
    
    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Minimize_IsDefined(bool state) {
        // Arrange
        InitializeWindow();
        
        // Act
        Window.SetMinimized(state);

        // Assert
        await Assert.That(Window.Minimized).IsEqualTo(state);
    }
}
