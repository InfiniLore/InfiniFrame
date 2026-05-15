// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge;
using InfiniFrameTests.Shared;
using System.Drawing;

namespace InfiniFrameTests.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class LocationTests {
    private const int Left = 10;
    private const int Top = 20;

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [DisplayName($"{nameof(LocationTests)}.{nameof(Builder)}")]
    public async Task Builder(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetUseOsDefaultLocation(true);
        builder.SetLocation(Left, Top);

        // Assert
        await Assert.That(builder.Configuration.Left).IsEqualTo(Left);
        await Assert.That(builder.Configuration.Top).IsEqualTo(Top);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToNativeParameters();
        await Assert.That(configParameters.Left).IsEqualTo(Left);
        await Assert.That(configParameters.Top).IsEqualTo(Top);
    }

    [Test]
    [DisplayName($"{nameof(LocationTests)}.{nameof(Builder_ShouldOverwriteOsDefaultLocationAndCentered)}")]
    public async Task Builder_ShouldOverwriteOsDefaultLocationAndCentered(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        InfiniFrameNativeParameters expectedConfigParameters = new InfiniFrameOptionsBuilder {
            Left = Left,
            Top = Top,
            UseOsDefaultLocation = false,
            Centered = false
        }.ToNativeParameters();

        // Act
        builder.SetUseOsDefaultLocation(true);
        builder.SetLocation(Left, Top);

        // Assert
        await Assert.That(builder.Configuration.Left).IsEqualTo(Left);
        await Assert.That(builder.Configuration.Top).IsEqualTo(Top);
        await Assert.That(builder.Configuration.UseOsDefaultLocation).IsFalse();
        await Assert.That(builder.Configuration.Centered).IsFalse();

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToNativeParameters();
        await Assert.That(configParameters).IsEqualTo(expectedConfigParameters);
    }

    [Test]
    [DisplayName($"{nameof(LocationTests)}.{nameof(Window)}")]   
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallel(ParallelControl.InfiniFrame)]   
    public async Task Window(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetLocation(Left, Top);

        // Assert
        await Assert.That(window.Location).IsEqualTo(new Point(Left, Top));
    }

    [Test]
    [DisplayName($"{nameof(LocationTests)}.{nameof(Window_AsPoint)}")]  
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task Window_AsPoint(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetLocation(new Point(Left, Top));

        // Assert
        await Assert.That(window.Location).IsEqualTo(new Point(Left, Top));
    }

    [Test]
    [DisplayName($"{nameof(LocationTests)}.{nameof(FullIntegration)}")] 
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task FullIntegration(CancellationToken ct = default) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder.SetLocation(Left, Top),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Location).IsEqualTo(new Point(Left, Top));
    }
}
