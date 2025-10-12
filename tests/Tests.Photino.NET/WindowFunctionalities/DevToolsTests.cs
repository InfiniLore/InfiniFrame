// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Photino.NET;

namespace Tests.Photino.NET.WindowFunctionalities;
using Tests.Shared.Photino;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class DevToolsTests {

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Builder(bool state) {
        // Arrange
        var builder = PhotinoWindowBuilder.Create();

        // Act
        builder.SetDevToolsEnabled(state);

        // Assert
        await Assert.That(builder.Configuration.DevToolsEnabled).IsEqualTo(state);

        PhotinoNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.DevToolsEnabled).IsEqualTo(state);
    }
    
    [Test]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnWindows("For some reason it keeps tripping up the transport connection")]
    [NotInParallel(ParallelControl.Photino)]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Window(bool state) {
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;

        // Act
        window.SetDevToolsEnabled(state);

        // Assert
        bool foundState = window.DevToolsEnabled;
        await Assert.That(foundState).IsEqualTo(state);
    }

    [Test]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnWindows("For some reason it keeps tripping up the transport connection")]
    [NotInParallel(ParallelControl.Photino)]
    [Arguments(true)]
    [Arguments(false)]
    public async Task FullIntegration(bool state) {
        // Arrange

        // Act
        var windowUtility = WindowTestUtility.Create(
            builder => builder
                .SetDevToolsEnabled(state)
        );
        IPhotinoWindow window = windowUtility.Window;
        
        // Assert
        bool foundState = window.DevToolsEnabled;
        await Assert.That(foundState).IsEqualTo(state);
    }
    
}
