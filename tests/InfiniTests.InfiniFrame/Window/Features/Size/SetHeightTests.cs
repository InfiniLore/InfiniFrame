// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Size;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SetHeightTests {
    [Test]
    [Arguments(360)]
    [Arguments(620)]
    public async Task AtBuilderStage_DirectAssignment(int value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.Size.SetHeight(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Size.Height).IsEqualTo(value);
        await Assert.That(builder.Features.Size.StartWithOsDefaultSize).IsFalse();
        await Assert.That(initParameters.Height).IsEqualTo(value);
        await Assert.That(initParameters.UseOsDefaultSize).IsFalse();
    }

    [Test]
    [Arguments(380)]
    [Arguments(640)]
    public async Task AtBuilderStage_ExtensionAssignment(int value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetHeight(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Size.Height).IsEqualTo(value);
        await Assert.That(builder.Features.Size.StartWithOsDefaultSize).IsFalse();
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.Height).IsEqualTo(value);
        await Assert.That(initParameters.UseOsDefaultSize).IsFalse();
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(420)]
    public async Task AtWindowStage_DirectAssignment(int value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.Size.SetHeight(value);

        // Assert
        await Assert.That(window.Features.Size.Height).IsEqualTo(value);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(460)]
    public async Task AtWindowStage_ExtensionAssignment(int value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        IInfiniFrameWindow returnedWindow = window.SetHeight(value);

        // Assert
        await Assert.That(window.Features.Size.Height).IsEqualTo(value);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }
}
