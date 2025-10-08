// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Photino.NET;
using System.Drawing;
using Tests.Photino.NET.TestUtilities;

namespace Tests.Photino.NET.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MaxSizeTests {
    private const int Width = 10;
    private const int Height = 20;

    [Test]
    public async Task Builder() {
        // Arrange
        var builder = PhotinoWindowBuilder.Create();

        // Act
        builder.SetMaxSize(Width, Height);

        // Assert
        await Assert.That(builder.Configuration.MaxWidth).IsEqualTo(Width);
        await Assert.That(builder.Configuration.MaxHeight).IsEqualTo(Height);

        PhotinoNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.MaxWidth).IsEqualTo(Width);
        await Assert.That(configParameters.MaxHeight).IsEqualTo(Height);
    }
    
    [Test]
    [SkipUtility.OnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    public async Task Window() {
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;

        // Act
        window.SetMaxSize(400, 500);

        // Assert
        await Assert.That(window.MaxSize).IsEqualTo(new Size(400, 500));
    }
    
    [Test]
    [SkipUtility.OnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    public async Task Window_AsSize() {
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;

        // Act
        window.SetMaxSize(new Size(400, 500));

        // Assert
        await Assert.That(window.MaxSize).IsEqualTo(new Size(400, 500));
    }

    [Test]
    [SkipUtility.OnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    public async Task FullIntegration() {
        // Arrange

        // Act
        using var windowUtility = WindowTestUtility.Create(
            builder => builder
                .SetChromeless(true)
                .SetMaxSize(400, 500)
        );
        IPhotinoWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.MaxSize).IsEqualTo(new Size(400, 500));
    }
}
