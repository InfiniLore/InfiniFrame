// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Photino.NET;
using Tests.Photino.NET.TestUtilities;

namespace Tests.Photino.NET.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TopMostTests {

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Builder(bool state) {
        // Arrange
        var builder = PhotinoWindowBuilder.Create();

        // Act
        builder.SetTopMost(state);

        // Assert
        await Assert.That(builder.Configuration.TopMost).IsEqualTo(state);

        PhotinoNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.Topmost).IsEqualTo(state);
    }
    
    [Test]
    [SkipUtility.OnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Window(bool state) {
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;

        // Act
        window.SetTopMost(state);

        // Assert
        await Assert.That(window.TopMost).IsEqualTo(state);
    }

    [Test]
    [SkipUtility.OnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    [Arguments(true)]
    [Arguments(false)]
    public async Task FullIntegration(bool state) {
        // Arrange

        // Act
        using var windowUtility = WindowTestUtility.Create(
            builder => builder
                .SetTopMost(state)
        );
        IPhotinoWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.TopMost).IsEqualTo(state);
    }
    
}
