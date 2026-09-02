// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.State;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ZoomFactorTests {

    [Test]
    [Arguments(50)]
    [Arguments(100)]
    [Arguments(150)]
    [Arguments(200)]
    public async Task AtBuilderStage_DirectAssignment(int value, CancellationToken ct) {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();

        // Act
        builder.Features.State.SetZoomFactor(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert   
        await Assert.That(builder.Features.State.ZoomFactor).IsEqualTo(value);
        await Assert.That(initParameters.Zoom).IsEqualTo(value);
    }

    [Test]
    [Arguments(50)]
    [Arguments(100)]
    [Arguments(150)]
    [Arguments(200)]
    public async Task AtBuilderStage_ExtensionAssignment(int value, CancellationToken ct) {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetZoomFactor(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert   
        await Assert.That(builder.Features.State.ZoomFactor).IsEqualTo(value);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.Zoom).IsEqualTo(value);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(50)]
    [Arguments(100)]
    [Arguments(150)]
    [Arguments(200)]
    public async Task AtWindowStage_DirectAssignment(int value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.State.SetZoomFactor(value);

        // Assert
        await Assert.That(window.Features.State.ZoomFactor).IsEqualTo(value);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(50)]
    [Arguments(100)]
    [Arguments(150)]
    [Arguments(200)]
    public async Task AtWindowStage_ExtensionAssignment(int value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        IInfiniFrameWindow returnedWindow = window.SetZoomFactor(value);

        // Assert
        await Assert.That(window.Features.State.ZoomFactor).IsEqualTo(value);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(50)]
    [Arguments(100)]
    [Arguments(150)]
    [Arguments(200)]
    public async Task AtWindowStage_ThroughBuilderAssignment(int value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.State.SetZoomFactor(value);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        IInfiniFrameWindowBuilder builder = windowUtility.BuilderSnapshot;

        // Act

        // Assert
        await Assert.That(builder.Features.State.ZoomFactor).IsEqualTo(value);
        await Assert.That(window.Features.State.ZoomFactor).IsEqualTo(value);
    }
}
