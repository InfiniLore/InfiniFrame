// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Decorations;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class LimitLinuxWindowTitleLengthTests {
    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtBuilderStage_DirectAssignment(bool value, CancellationToken ct) {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();

        // Act
        builder.Features.Decorations.SetLimitLinuxWindowTitleLength(value);

        // Assert
        await Assert.That(builder.Features.Decorations.LimitLinuxWindowTitleLength).IsEqualTo(value);
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtBuilderStage_ExtensionAssignment(bool value, CancellationToken ct) {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetLimitLinuxWindowTitleLength(value);

        // Assert
        await Assert.That(builder.Features.Decorations.LimitLinuxWindowTitleLength).IsEqualTo(value);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtWindowStage_ThroughBuilderAssignment(bool value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Decorations.SetLimitLinuxWindowTitleLength(value);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        IInfiniFrameWindowBuilder builder = windowUtility.BuilderSnapshot;

        // Act

        // Assert
        await Assert.That(builder.Features.Decorations.LimitLinuxWindowTitleLength).IsEqualTo(value);
        await Assert.That(window.Features.Decorations.LimitLinuxWindowTitleLength).IsEqualTo(value);
    }
}
