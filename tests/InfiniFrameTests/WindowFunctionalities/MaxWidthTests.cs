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
public class MaxWidthTests {
    private const int MaxWidth = 20;

    [Test]
    [DisplayName($"{nameof(MaxWidthTests)}.{nameof(Builder)}")]
    public async Task Builder() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetMaxWidth(MaxWidth);

        // Assert
        await Assert.That(builder.Configuration.MaxWidth).IsEqualTo(MaxWidth);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.MaxWidth).IsEqualTo(MaxWidth);
    }

    [Test]
    [DisplayName($"{nameof(MaxWidthTests)}.{nameof(Window)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    public async Task Window(CancellationToken timeoutToken) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetMaxWidth(500);

        // Assert
        await Assert.That(window.MaxWidth).IsEqualTo(500);
    }

    [Test]
    [DisplayName($"{nameof(MaxWidthTests)}.{nameof(FullIntegration)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    public async Task FullIntegration(CancellationToken timeoutToken) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder
                .SetMaxWidth(500)
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.MaxWidth).IsEqualTo(500);
    }
}
