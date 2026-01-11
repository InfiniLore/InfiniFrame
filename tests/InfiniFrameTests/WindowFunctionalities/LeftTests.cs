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
public class LeftTests {
    private const int Left = 20;

    [Test]
    [DisplayName($"{nameof(LeftTests)}.{nameof(Builder)}")]
    public async Task Builder() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetLeft(Left);

        // Assert
        await Assert.That(builder.Configuration.Left).IsEqualTo(Left);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.Left).IsEqualTo(Left);
    }

    [Test]
    [DisplayName($"{nameof(LeftTests)}.{nameof(Builder_ShouldOverwriteOsDefaultLocationAndCentered)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task Builder_ShouldOverwriteOsDefaultLocationAndCentered() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        InfiniFrameNativeParameters expectedConfigParameters = new InfiniFrameWindowConfiguration {
            Left = Left,
            UseOsDefaultLocation = false,
            Centered = false
        }.ToParameters();

        // Act
        builder.Center();
        builder.SetUseOsDefaultLocation(true);
        builder.SetLeft(Left);

        // Assert
        await Assert.That(builder.Configuration.Left).IsEqualTo(Left);
        await Assert.That(builder.Configuration.UseOsDefaultLocation).IsEqualTo(false);
        await Assert.That(builder.Configuration.Centered).IsEqualTo(false);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters).IsEqualTo(expectedConfigParameters);
    }

    [Test]
    [DisplayName($"{nameof(LeftTests)}.{nameof(Window)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    public async Task Window(CancellationToken timeoutToken) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetLeft(Left);

        // Assert
        await Assert.That(window.Left).IsEqualTo(Left);
    }

    [Test]
    [DisplayName($"{nameof(LeftTests)}.{nameof(FullIntegration)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    public async Task FullIntegration(CancellationToken timeoutToken) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder
                .SetLeft(Left)
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Left).IsEqualTo(Left);
    }

}
