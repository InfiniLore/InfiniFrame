// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Size;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SetWidthTests {
    [Test]
    [Arguments(640)]
    [Arguments(980)]
    public async Task AtBuilderStage_DirectAssignment(int value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.Size.SetWidth(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Size.Width).IsEqualTo(value);
        await Assert.That(builder.Features.Size.StartWithOsDefaultSize).IsFalse();
        await Assert.That(initParameters.Width).IsEqualTo(value);
        await Assert.That(initParameters.UseOsDefaultSize).IsFalse();
    }

    [Test]
    [Arguments(660)]
    [Arguments(1000)]
    public async Task AtBuilderStage_ExtensionAssignment(int value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetWidth(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Size.Width).IsEqualTo(value);
        await Assert.That(builder.Features.Size.StartWithOsDefaultSize).IsFalse();
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.Width).IsEqualTo(value);
        await Assert.That(initParameters.UseOsDefaultSize).IsFalse();
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(700)]
    public async Task AtWindowStage_DirectAssignment(int value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.Size.SetWidth(value);

        // Assert
        await Assert.That(window.Features.Size.Width).IsEqualTo(value);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(740)]
    public async Task AtWindowStage_ExtensionAssignment(int value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        IInfiniFrameWindow returnedWindow = window.SetWidth(value);

        // Assert
        await Assert.That(window.Features.Size.Width).IsEqualTo(value);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }
}
