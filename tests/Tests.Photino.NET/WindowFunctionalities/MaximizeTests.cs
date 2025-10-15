// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Photino.NET;

namespace Tests.Photino.NET.WindowFunctionalities;
using InfiniLore.Photino.Native;
using Tests.Shared.Photino;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MaximizeTests {

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Builder(bool state) {
        // Arrange
        var builder = PhotinoWindowBuilder.Create();

        // Act
        builder.SetMaximized(state);

        // Assert
        await Assert.That(builder.Configuration.Maximized).IsEqualTo(state);

        PhotinoNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.Maximized).IsEqualTo(state);
    }
    
    [Test]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Window(bool state) {
        SkipUtility.SkipOnLinux(state);
        
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;

        // Act
        window.SetMaximized(state);

        // Assert
        await Assert.That(window.Maximized).IsEqualTo(state);
    }
    
    [Test]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux]
    [NotInParallel(ParallelControl.Photino)]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Window_Toggle(bool state) {
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;

        // Act
        window.SetMaximized(state);
        window.ToggleMaximized();

        // Assert
        await Assert.That(window.Maximized).IsEqualTo(!state);
    }

    [Test]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    [Arguments(true)]
    [Arguments(false)]
    public async Task FullIntegration(bool state) {
        SkipUtility.SkipOnLinux(state);
        
        // Arrange

        // Act
        using var windowUtility = WindowTestUtility.Create(
            builder => builder
                .SetMaximized(state)
        );
        IPhotinoWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Maximized).IsEqualTo(state);
    }
    
}
