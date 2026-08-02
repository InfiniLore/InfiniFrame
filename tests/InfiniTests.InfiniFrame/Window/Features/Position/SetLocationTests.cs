// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;
using System.Drawing;

namespace InfiniTests.InfiniFrame.Window.Features.Position;
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
    [SkipOnLinux]
    [NotInParallelInfiniTests]
    [Arguments(140, 260)]
    public async Task AtWindowStage_DirectAssignment(int left, int top, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int originalLeft = window.Features.Position.Left;
        int originalTop = window.Features.Position.Top;
        int targetLeft = left == originalLeft ? left + 20 : left;
        int targetTop = top == originalTop ? top + 20 : top;

        // Act
        window.Features.Position.SetLocation(targetLeft, targetTop);

        // Assert
        int updatedLeft = await PollUtility.WaitForChangeAsync(getValue: () => window.Features.Position.Left, originalLeft, TimeSpan.FromSeconds(5), ct);
        int updatedTop = await PollUtility.WaitForChangeAsync(getValue: () => window.Features.Position.Top, originalTop, TimeSpan.FromSeconds(5), ct);
        await Assert.That(updatedLeft).IsEqualTo(targetLeft);
        await Assert.That(updatedTop).IsEqualTo(targetTop);
    }

    [Test]
    [SkipOnLinux]
    [NotInParallelInfiniTests]
    [Arguments(160, 280)]
    public async Task AtWindowStage_ExtensionAssignment(int left, int top, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int originalLeft = window.Features.Position.Left;
        int originalTop = window.Features.Position.Top;
        int targetLeft = left == originalLeft ? left + 20 : left;
        int targetTop = top == originalTop ? top + 20 : top;

        // Act
        IInfiniFrameWindow returnedWindow = window.SetLocation(targetLeft, targetTop);

        // Assert
        int updatedLeft = await PollUtility.WaitForChangeAsync(getValue: () => window.Features.Position.Left, originalLeft, TimeSpan.FromSeconds(5), ct);
        int updatedTop = await PollUtility.WaitForChangeAsync(getValue: () => window.Features.Position.Top, originalTop, TimeSpan.FromSeconds(5), ct);
        await Assert.That(updatedLeft).IsEqualTo(targetLeft);
        await Assert.That(updatedTop).IsEqualTo(targetTop);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }
}