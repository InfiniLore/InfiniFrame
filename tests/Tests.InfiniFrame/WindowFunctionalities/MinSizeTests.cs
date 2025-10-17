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
public class MinSizeTests {
    private const int Width = 10;
    private const int Height = 20;

    [Test]
    public async Task Builder() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetMinSize(Width, Height);

        // Assert
        await Assert.That(builder.Configuration.MinWidth).IsEqualTo(Width);
        await Assert.That(builder.Configuration.MinHeight).IsEqualTo(Height);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.MinWidth).IsEqualTo(Width);
        await Assert.That(configParameters.MinHeight).IsEqualTo(Height);
    }

    [Test, SkipUtility.SkipOnMacOs, NotInParallel(ParallelControl.InfiniFrame)]
    public async Task Window() {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetMinSize(400, 500);

        // Assert
        await Assert.That(window.MinSize).IsEqualTo(new Size(400, 500));
    }

    [Test, SkipUtility.SkipOnMacOs, NotInParallel(ParallelControl.InfiniFrame)]
    public async Task Window_AsSize() {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetMinSize(new Size(400, 500));

        // Assert
        await Assert.That(window.MinSize).IsEqualTo(new Size(400, 500));
    }

    [Test, SkipUtility.SkipOnMacOs, NotInParallel(ParallelControl.InfiniFrame)]
    public async Task FullIntegration() {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder
                .SetChromeless(true)
                .SetMinSize(400, 500)
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.MinSize).IsEqualTo(new Size(400, 500));
    }
}
