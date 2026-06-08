// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class LeftTests {
    private const int Left = 20;

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [DisplayName($"{nameof(LeftTests)}.{nameof(Builder)}")]
    public async Task Builder(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetLeft(Left);

        // Assert
        await Assert.That(builder.Configuration.Left).IsEqualTo(Left);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToNativeParameters();
        await Assert.That(configParameters.Left).IsEqualTo(Left);
    }

    [Test]
    [DisplayName($"{nameof(LeftTests)}.{nameof(Builder_ShouldOverwriteOsDefaultLocationAndCentered)}")]
    public async Task Builder_ShouldOverwriteOsDefaultLocationAndCentered(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        InfiniFrameNativeParameters expectedConfigParameters = new InfiniFrameOptionsBuilder {
            Left = Left,
            UseOsDefaultLocation = false,
            Centered = false
        }.ToNativeParameters();

        // Act
        builder.Center();
        builder.SetUseOsDefaultLocation(true);
        builder.SetLeft(Left);

        // Assert
        await Assert.That(builder.Configuration.Left).IsEqualTo(Left);
        await Assert.That(builder.Configuration.UseOsDefaultLocation).IsFalse();
        await Assert.That(builder.Configuration.Centered).IsFalse();

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToNativeParameters();
        await Assert.That(configParameters).IsEqualTo(expectedConfigParameters, InfiniFrameNativeParametersEqualityComparer.Instance);
    }

    [Test]
    [DisplayName($"{nameof(LeftTests)}.{nameof(Window)}")]
    [SkipOnMacOs]
    [SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallelInfiniTests]
    public async Task Window(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetLeft(Left);

        // Assert
        await Assert.That(window.Left).IsEqualTo(Left);
    }

    [Test]
    [DisplayName($"{nameof(LeftTests)}.{nameof(FullIntegration)}")]
    [SkipOnMacOs]
    [SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallelInfiniTests]
    public async Task FullIntegration(CancellationToken ct = default) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameTestWindow.Create(
            builder: builder => builder.SetLeft(Left),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Left).IsEqualTo(Left);
    }
}
