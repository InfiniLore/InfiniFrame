// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Native;
using InfiniFrameTests.Shared;

namespace InfiniFrameTests.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MaxHeightTests {
    private const int MaxHeight = 20;

    [Test]
    [DisplayName($"{nameof(MaxHeightTests)}.{nameof(Builder)}")]
    public async Task Builder() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetMaxHeight(MaxHeight);

        // Assert
        await Assert.That(builder.Configuration.MaxHeight).IsEqualTo(MaxHeight);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.MaxHeight).IsEqualTo(MaxHeight);
    }

    [Test]
    [DisplayName($"{nameof(MaxHeightTests)}.{nameof(Window)}")]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task Window() {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetMaxHeight(500);

        // Assert
        await Assert.That(window.MaxHeight).IsEqualTo(500);
    }

    [Test]
    [DisplayName($"{nameof(MaxHeightTests)}.{nameof(FullIntegration)}")] 
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task FullIntegration() {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder
                .SetMaxHeight(500)
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.MaxHeight).IsEqualTo(500);
    }
}
