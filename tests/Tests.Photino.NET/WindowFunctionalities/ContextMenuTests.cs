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
public class ContextMenuTests {

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Builder(bool state) {
        // Arrange
        var builder = PhotinoWindowBuilder.Create();

        // Act
        builder.SetContextMenuEnabled(state);

        // Assert
        await Assert.That(builder.Configuration.ContextMenuEnabled).IsEqualTo(state);

        PhotinoNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.ContextMenuEnabled).IsEqualTo(state);
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
        window.SetContextMenuEnabled(state);

        // Assert
        bool foundState = window.ContextMenuEnabled;
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
                .SetContextMenuEnabled(state)
        );
        IPhotinoWindow window = windowUtility.Window;
        
        // Assert
        bool foundState = window.ContextMenuEnabled;
        await Assert.That(foundState).IsEqualTo(state);
    }
    
}
