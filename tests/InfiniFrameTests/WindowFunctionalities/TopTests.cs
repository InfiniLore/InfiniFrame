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
public class TopTests {
    private const int Top = 20;

    [Test]
    [DisplayName($"{nameof(TopTests)}.{nameof(Builder)}")]
    public async Task Builder() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetTop(Top);

        // Assert
        await Assert.That(builder.Configuration.Top).IsEqualTo(Top);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.Top).IsEqualTo(Top);
    }

    [Test]
    [DisplayName($"{nameof(TopTests)}.{nameof(Builder_ShouldOverwriteOsDefaultLocationAndCentered)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task Builder_ShouldOverwriteOsDefaultLocationAndCentered() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        InfiniFrameNativeParameters expectedConfigParameters = new InfiniFrameWindowConfiguration {
            Top = Top,
            UseOsDefaultLocation = false,
            Centered = false
        }.ToParameters();

        // Act
        builder.Center();
        builder.SetUseOsDefaultLocation(true);
        builder.SetTop(Top);

        // Assert
        await Assert.That(builder.Configuration.Top).IsEqualTo(Top);
        await Assert.That(builder.Configuration.UseOsDefaultLocation).IsEqualTo(false);
        await Assert.That(builder.Configuration.Centered).IsEqualTo(false);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters).IsEqualTo(expectedConfigParameters);
    }

    [Test]
    [DisplayName($"{nameof(TopTests)}.{nameof(Window)}")]   
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    public async Task Window(CancellationToken timeoutToken) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetTop(Top);

        // Assert
        await Assert.That(window.Top).IsEqualTo(Top);
    }

    [Test]
    [DisplayName($"{nameof(TopTests)}.{nameof(FullIntegration)}")] 
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    public async Task FullIntegration(CancellationToken timeoutToken) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder
                .SetTop(Top)
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Top).IsEqualTo(Top);
    }

}
