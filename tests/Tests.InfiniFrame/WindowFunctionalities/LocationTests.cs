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
public class LocationTests {
    private const int Left = 10;
    private const int Top = 20;

    [Test]
    public async Task Builder() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetUseOsDefaultLocation(true);
        builder.SetLocation(Left, Top);

        // Assert
        await Assert.That(builder.Configuration.Left).IsEqualTo(Left);
        await Assert.That(builder.Configuration.Top).IsEqualTo(Top);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.Left).IsEqualTo(Left);
        await Assert.That(configParameters.Top).IsEqualTo(Top);
    }

    [Test]
    public async Task Builder_ShouldOverwriteOsDefaultLocationAndCentered() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        InfiniFrameNativeParameters expectedConfigParameters = new InfiniFrameWindowConfiguration {
            Left = Left,
            Top = Top,
            UseOsDefaultLocation = false,
            Centered = false
        }.ToParameters();

        // Act
        builder.SetUseOsDefaultLocation(true);
        builder.SetLocation(Left, Top);

        // Assert
        await Assert.That(builder.Configuration.Left).IsEqualTo(Left);
        await Assert.That(builder.Configuration.Top).IsEqualTo(Top);
        await Assert.That(builder.Configuration.UseOsDefaultLocation).IsEqualTo(false);
        await Assert.That(builder.Configuration.Centered).IsEqualTo(false);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters).IsEqualTo(expectedConfigParameters);
    }

    [Test, SkipUtility.SkipOnMacOs, SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement), NotInParallel(ParallelControl.InfiniFrame)]
    public async Task Window() {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetLocation(Left, Top);

        // Assert
        await Assert.That(window.Location).IsEqualTo(new Point(Left, Top));
    }

    [Test, SkipUtility.SkipOnMacOs, SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement), NotInParallel(ParallelControl.InfiniFrame)]
    public async Task Window_AsPoint() {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetLocation(new Point(Left, Top));

        // Assert
        await Assert.That(window.Location).IsEqualTo(new Point(Left, Top));
    }

    [Test, SkipUtility.SkipOnMacOs]
    public async Task FullIntegration() {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder.SetLocation(Left, Top)
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Location).IsEqualTo(new Point(Left, Top));
    }
}
