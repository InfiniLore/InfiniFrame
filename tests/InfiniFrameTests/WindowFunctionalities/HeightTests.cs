// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge;
using InfiniFrameTests.Shared;

namespace InfiniFrameTests.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class HeightTests {
    private const int Height = 20;

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [DisplayName($"{nameof(HeightTests)}.{nameof(FullIntegration)}")]
    public async Task Builder(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetUseOsDefaultSize(true);
        builder.SetHeight(Height);

        // Assert
        await Assert.That(builder.Configuration.Height).IsEqualTo(Height);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToNativeParameters();
        await Assert.That(configParameters.Height).IsEqualTo(Height);
    }

    [Test]
    [DisplayName($"{nameof(HeightTests)}.{nameof(Builder_ShouldOverwriteOsDefaultSizeAndCentered)}")]
    public async Task Builder_ShouldOverwriteOsDefaultSizeAndCentered() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        InfiniFrameNativeParameters expectedConfigParameters = new InfiniFrameOptionsBuilder {
            Height = Height,
            UseOsDefaultSize = false,
            Centered = false
        }.ToNativeParameters();

        // Act
        builder.SetUseOsDefaultSize(true);
        builder.SetHeight(Height);

        // Assert
        await Assert.That(builder.Configuration.Height).IsEqualTo(Height);
        await Assert.That(builder.Configuration.UseOsDefaultSize).IsFalse();
        await Assert.That(builder.Configuration.Centered).IsFalse();

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToNativeParameters();
        await Assert.That(configParameters).IsEqualTo(expectedConfigParameters);
    }

    [Test]
    [DisplayName($"{nameof(HeightTests)}.{nameof(Window)}")]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task Window(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetHeight(500);

        // Assert
        await Assert.That(window.Height).IsEqualTo(500);
    }

    [Test]
    [DisplayName($"{nameof(HeightTests)}.{nameof(FullIntegration)}")]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task FullIntegration(CancellationToken ct = default) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder
                .SetChromeless(true)
                .SetHeight(500),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Height).IsEqualTo(500);
    }

    [Test]
    [DisplayName($"{nameof(HeightTests)}.{nameof(Window_WithChromelessToGetSmallestHeight)}")]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task Window_WithChromelessToGetSmallestHeight(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder.SetChromeless(true),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetHeight(Height);

        // Assert
        await Assert.That(window.Height).IsEqualTo(Height);
    }

    [Test]
    [DisplayName($"{nameof(HeightTests)}.{nameof(FullIntegration_WithChromelessToGetSmallestHeight)}")]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task FullIntegration_WithChromelessToGetSmallestHeight(CancellationToken ct = default) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder
                .SetChromeless(true)
                .SetHeight(Height),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Height).IsEqualTo(Height);
    }
}
