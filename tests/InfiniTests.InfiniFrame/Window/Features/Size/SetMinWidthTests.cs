// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Size;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SetMinWidthTests {
    [Test]
    [Arguments(380)]
    [Arguments(520)]
    public async Task AtBuilderStage_DirectAssignment(int value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.Size.SetMinWidth(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Size.MinWidth).IsEqualTo(value);
        await Assert.That(initParameters.MinWidth).IsEqualTo(value);
    }

    [Test]
    [Arguments(400)]
    [Arguments(540)]
    public async Task AtBuilderStage_ExtensionAssignment(int value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetMinWidth(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Size.MinWidth).IsEqualTo(value);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.MinWidth).IsEqualTo(value);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(420)]
    public async Task AtWindowStage_DirectAssignment(int value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.Size.SetMinWidth(value);

        // Assert
        await Assert.That(window.Features.Size.MinWidth).IsEqualTo(value);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(440)]
    public async Task AtWindowStage_ExtensionAssignment(int value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        IInfiniFrameWindow returnedWindow = window.SetMinWidth(value);

        // Assert
        await Assert.That(window.Features.Size.MinWidth).IsEqualTo(value);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }
}
