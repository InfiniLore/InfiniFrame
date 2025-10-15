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
public class MaxWidthTests {
    private const int MaxWidth = 20;

    [Test]
    public async Task Builder() {
        // Arrange
        var builder = PhotinoWindowBuilder.Create();

        // Act
        builder.SetMaxWidth(MaxWidth);

        // Assert
        await Assert.That(builder.Configuration.MaxWidth).IsEqualTo(MaxWidth);

        PhotinoNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.MaxWidth).IsEqualTo(MaxWidth);
    }
    
    [Test]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    public async Task Window() {
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;

        // Act
        window.SetMaxWidth(500);

        // Assert
        await Assert.That(window.MaxWidth).IsEqualTo(500);
    }

    [Test]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    public async Task FullIntegration() {
        // Arrange

        // Act
        using var windowUtility = WindowTestUtility.Create(
            builder => builder
                .SetMaxWidth(500)
        );
        IPhotinoWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.MaxWidth).IsEqualTo(500);
    }
}
