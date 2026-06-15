// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;
using System.Drawing;

namespace InfiniTests.InfiniFrame.Window.Features.InfiniFrameWindowFeaturePosition;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SetLocationTests {
    [Test]
    [Arguments(120, 240)]
    [Arguments(300, 400)]
    public async Task AtBuilderStage_DirectAssignment(int left, int top, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.Position.SetLocation(left, top);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Position.Left).IsEqualTo(left);
        await Assert.That(builder.Features.Position.Top).IsEqualTo(top);
        await Assert.That(builder.Features.Position.StartAtOsDefaultLocation).IsFalse();
        await Assert.That(initParameters.Left).IsEqualTo(left);
        await Assert.That(initParameters.Top).IsEqualTo(top);
        await Assert.That(initParameters.UseOsDefaultLocation).IsFalse();
    }

    [Test]
    [Arguments(500, 600)]
    [Arguments(700, 800)]
    public async Task AtBuilderStage_ExtensionAssignment(int left, int top, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        Point value = new(left, top);

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetLocation(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Position.Left).IsEqualTo(left);
        await Assert.That(builder.Features.Position.Top).IsEqualTo(top);
        await Assert.That(builder.Features.Position.StartAtOsDefaultLocation).IsFalse();
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.Left).IsEqualTo(left);
        await Assert.That(initParameters.Top).IsEqualTo(top);
        await Assert.That(initParameters.UseOsDefaultLocation).IsFalse();
    }

    [Test]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    [Arguments(140, 260)]
    public async Task AtWindowStage_DirectAssignment(int left, int top, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.Position.SetLocation(left, top);

        // Assert
        await Assert.That(window.Features.Position.Left).IsEqualTo(left);
        await Assert.That(window.Features.Position.Top).IsEqualTo(top);
    }

    [Test]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    [Arguments(160, 280)]
    public async Task AtWindowStage_ExtensionAssignment(int left, int top, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        IInfiniFrameWindow returnedWindow = window.SetLocation(left, top);

        // Assert
        await Assert.That(window.Features.Position.Left).IsEqualTo(left);
        await Assert.That(window.Features.Position.Top).IsEqualTo(top);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }
}
