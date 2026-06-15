// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.InfiniFrameWindowFeaturePosition;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SetTopTests {
    [Test]
    [Arguments(220)]
    [Arguments(420)]
    public async Task AtBuilderStage_DirectAssignment(int value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.Position.SetTop(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Position.Top).IsEqualTo(value);
        await Assert.That(builder.Features.Position.StartAtOsDefaultLocation).IsFalse();
        await Assert.That(initParameters.Top).IsEqualTo(value);
        await Assert.That(initParameters.UseOsDefaultLocation).IsFalse();
    }

    [Test]
    [Arguments(240)]
    [Arguments(440)]
    public async Task AtBuilderStage_ExtensionAssignment(int value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetTop(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Position.Top).IsEqualTo(value);
        await Assert.That(builder.Features.Position.StartAtOsDefaultLocation).IsFalse();
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.Top).IsEqualTo(value);
        await Assert.That(initParameters.UseOsDefaultLocation).IsFalse();
    }

    [Test]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_DirectAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int initialTop = window.Features.Position.Top;
        int targetTop = initialTop + 40;

        // Act
        window.Features.Position.SetTop(targetTop);

        // Assert
        await Assert.That(window.Features.Position.Top).IsNotEqualTo(initialTop);
    }

    [Test]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_ExtensionAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int initialTop = window.Features.Position.Top;
        int targetTop = initialTop + 50;

        // Act
        IInfiniFrameWindow returnedWindow = window.SetTop(targetTop);

        // Assert
        await Assert.That(window.Features.Position.Top).IsNotEqualTo(initialTop);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }
}
