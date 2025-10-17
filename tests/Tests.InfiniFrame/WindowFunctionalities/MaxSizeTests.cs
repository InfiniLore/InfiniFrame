// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.InfiniFrame.Native;
using InfiniLore.InfiniFrame;
using System.Drawing;

namespace Tests.InfiniFrame.WindowFunctionalities;
using Tests.Shared;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MaxSizeTests {
    private const int Width = 10;
    private const int Height = 20;

    [Test]
    public async Task Builder() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetMaxSize(Width, Height);

        // Assert
        await Assert.That(builder.Configuration.MaxWidth).IsEqualTo(Width);
        await Assert.That(builder.Configuration.MaxHeight).IsEqualTo(Height);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.MaxWidth).IsEqualTo(Width);
        await Assert.That(configParameters.MaxHeight).IsEqualTo(Height);
    }

    [Test, SkipUtility.SkipOnMacOs, NotInParallel(ParallelControl.InfiniFrame)]
    public async Task Window() {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetMaxSize(400, 500);

        // Assert
        await Assert.That(window.MaxSize).IsEqualTo(new Size(400, 500));
    }

    [Test, SkipUtility.SkipOnMacOs, NotInParallel(ParallelControl.InfiniFrame)]
    public async Task Window_AsSize() {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetMaxSize(new Size(400, 500));

        // Assert
        await Assert.That(window.MaxSize).IsEqualTo(new Size(400, 500));
    }

    [Test, SkipUtility.SkipOnMacOs, NotInParallel(ParallelControl.InfiniFrame)]
    public async Task FullIntegration() {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder
                .SetChromeless(true)
                .SetMaxSize(400, 500)
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.MaxSize).IsEqualTo(new Size(400, 500));
    }
}
