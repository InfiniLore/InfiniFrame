// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Decorations;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TitleTests {
    [Test]
    [Arguments("InfiniFrame Title A")]
    [Arguments("InfiniFrame Title B")]
    public async Task AtBuilderStage_DirectAssignment(string value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.Decorations.SetTitle(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Decorations.Title).IsEqualTo(value);
        await Assert.That(initParameters.Title).IsEqualTo(value);
    }

    [Test]
    [Arguments("InfiniFrame Title C")]
    [Arguments("InfiniFrame Title D")]
    public async Task AtBuilderStage_ExtensionAssignment(string value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetTitle(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Decorations.Title).IsEqualTo(value);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.Title).IsEqualTo(value);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments("InfiniFrame Title Through Builder")]
    public async Task AtWindowStage_ThroughBuilderAssignment(string value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Decorations.SetTitle(value);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        IInfiniFrameWindowBuilder builder = windowUtility.BuilderSnapshot;

        // Act

        // Assert
        await Assert.That(builder.Features.Decorations.Title).IsEqualTo(value);
        await Assert.That(window.Features.Decorations.Title).IsEqualTo(value);
    }
}
