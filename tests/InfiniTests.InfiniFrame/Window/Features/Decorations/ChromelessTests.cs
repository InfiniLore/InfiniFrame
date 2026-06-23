// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Decorations;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ChromelessTests {
    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtBuilderStage_DirectAssignment(bool value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.Decorations.SetChromeless(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Decorations.IsChromeless).IsEqualTo(value);
        await Assert.That(initParameters.Chromeless).IsEqualTo(value);
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtBuilderStage_ExtensionAssignment(bool value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetChromeless(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Decorations.IsChromeless).IsEqualTo(value);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.Chromeless).IsEqualTo(value);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtWindowStage_ThroughBuilderAssignment(bool value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder => {
            builder.Features.Decorations.SetChromeless(value);
            if (!value) return;

            builder.Features.Size.SetSize(900, 600);
            builder.Features.Position.SetLocation(100, 100);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        IInfiniFrameWindowBuilder builder = windowUtility.BuilderSnapshot;

        // Act

        // Assert
        await Assert.That(builder.Features.Decorations.IsChromeless).IsEqualTo(value);
        await Assert.That(window.Features.Decorations.IsChromeless).IsEqualTo(value);
    }
}
