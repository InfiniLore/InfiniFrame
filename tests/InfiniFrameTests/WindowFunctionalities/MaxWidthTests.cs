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
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(TimeoutUtlitity.DefaultTimeout)]
    public async Task Window(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetMaxWidth(500);

        // Assert
        await Assert.That(window.MaxWidth).IsEqualTo(500);
    }

    [Test]
    [DisplayName($"{nameof(MaxWidthTests)}.{nameof(FullIntegration)}")]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(TimeoutUtlitity.DefaultTimeout)]
    public async Task FullIntegration(CancellationToken ct) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder.SetMaxWidth(500),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.MaxWidth).IsEqualTo(500);
    }
}
