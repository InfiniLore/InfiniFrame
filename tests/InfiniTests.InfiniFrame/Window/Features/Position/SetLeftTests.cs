// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Position;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SetLeftTests {
    [Test]
    [Arguments(320)]
    [Arguments(520)]
    public async Task AtBuilderStage_DirectAssignment(int value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.Position.SetLeft(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Position.Left).IsEqualTo(value);
        await Assert.That(builder.Features.Position.StartAtOsDefaultLocation).IsFalse();
        await Assert.That(initParameters.Left).IsEqualTo(value);
        await Assert.That(initParameters.UseOsDefaultLocation).IsFalse();
    }

    [Test]
    [Arguments(340)]
    [Arguments(540)]
    public async Task AtBuilderStage_ExtensionAssignment(int value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetLeft(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Position.Left).IsEqualTo(value);
        await Assert.That(builder.Features.Position.StartAtOsDefaultLocation).IsFalse();
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.Left).IsEqualTo(value);
        await Assert.That(initParameters.UseOsDefaultLocation).IsFalse();
    }

    [Test]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    [Arguments(360)]
    public async Task AtWindowStage_DirectAssignment(int value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int originalLocation = window.Features.Position.Left;

        // Act
        window.Features.Position.SetLeft(value);

        // Assert
        int newValue = await PollUtility.WaitForChangeAsync(
            () => window.Features.Position.Left, 
            originalLocation, 
            TimeSpan.FromSeconds(5),
            ct
        );
        await Assert.That(newValue).IsEqualTo(value);
    }

    [Test]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    [Arguments(380)]
    public async Task AtWindowStage_ExtensionAssignment(int value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int originalLocation = window.Features.Position.Left;

        // Act
        IInfiniFrameWindow returnedWindow = window.SetLeft(value);

        // Assert
        int newValue = await PollUtility.WaitForChangeAsync(
            () => window.Features.Position.Left, 
            originalLocation, 
            TimeSpan.FromSeconds(5),
            ct
        );
        await Assert.That(newValue).IsEqualTo(value);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }
}
