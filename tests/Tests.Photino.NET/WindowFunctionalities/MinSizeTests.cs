// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.InfiniFrame.NET;
using System.Drawing;

namespace Tests.Photino.NET.WindowFunctionalities;
using InfiniLore.Photino.Native;
using Tests.Shared.Photino;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MinSizeTests {
    private const int Width = 10;
    private const int Height = 20;

    [Test]
    public async Task Builder() {
        // Arrange
        var builder = PhotinoWindowBuilder.Create();

        // Act
        builder.SetMinSize(Width, Height);

        // Assert
        await Assert.That(builder.Configuration.MinWidth).IsEqualTo(Width);
        await Assert.That(builder.Configuration.MinHeight).IsEqualTo(Height);

        PhotinoNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.MinWidth).IsEqualTo(Width);
        await Assert.That(configParameters.MinHeight).IsEqualTo(Height);
    }
    
    [Test]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    public async Task Window() {
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;

        // Act
        window.SetMinSize(400, 500);

        // Assert
        await Assert.That(window.MinSize).IsEqualTo(new Size(400, 500));
    }
    
    [Test]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    public async Task Window_AsSize() {
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;

        // Act
        window.SetMinSize(new Size(400, 500));

        // Assert
        await Assert.That(window.MinSize).IsEqualTo(new Size(400, 500));
    }

    [Test]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    public async Task FullIntegration() {
        // Arrange

        // Act
        using var windowUtility = WindowTestUtility.Create(
            builder => builder
                .SetChromeless(true)
                .SetMinSize(400, 500)
        );
        IPhotinoWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.MinSize).IsEqualTo(new Size(400, 500));
    }
}
