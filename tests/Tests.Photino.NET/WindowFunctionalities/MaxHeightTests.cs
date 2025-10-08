// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Photino.NET;
using Tests.Photino.NET.TestUtilities;

namespace Tests.Photino.NET.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MaxHeightTests {
    private const int MaxHeight = 20;

    [Test]
    public async Task Builder() {
        // Arrange
        var builder = PhotinoWindowBuilder.Create();

        // Act
        builder.SetMaxHeight(MaxHeight);

        // Assert
        await Assert.That(builder.Configuration.MaxHeight).IsEqualTo(MaxHeight);

        PhotinoNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.MaxHeight).IsEqualTo(MaxHeight);
    }
    
    [Test]
    [SkipUtility.OnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    public async Task Window() {
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;

        // Act
        window.SetMaxHeight(500);

        // Assert
        await Assert.That(window.MaxHeight).IsEqualTo(500);
    }

    [Test]
    [SkipUtility.OnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    public async Task FullIntegration() {
        // Arrange

        // Act
        using var windowUtility = WindowTestUtility.Create(
            builder => builder
                .SetMaxHeight(500)
        );
        IPhotinoWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.MaxHeight).IsEqualTo(500);
    }
}
