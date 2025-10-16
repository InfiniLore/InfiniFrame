// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.InfiniFrame.NET;

namespace Tests.Photino.NET.WindowFunctionalities;
using InfiniLore.Photino.Native;
using Tests.Shared.Photino;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MinimizeTests {

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Builder(bool state) {
        // Arrange
        var builder = PhotinoWindowBuilder.Create();

        // Act
        builder.SetMinimized(state);

        // Assert
        await Assert.That(builder.Configuration.Minimized).IsEqualTo(state);

        PhotinoNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.Minimized).IsEqualTo(state);
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
        window.SetMinimized(state);

        // Assert
        await Assert.That(window.Minimized).IsEqualTo(state);
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
                .SetMinimized(state)
        );
        IPhotinoWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Minimized).IsEqualTo(state);
    }
    
}
