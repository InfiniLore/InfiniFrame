// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.State;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class FullScreenTests {

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtBuilderStage_DirectAssignment(bool value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        
        // Act
        builder.Features.State.SetFullScreen(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert   
        await Assert.That(builder.Features.State.StartFullScreen).IsEqualTo(value);
        await Assert.That(initParameters.FullScreen).IsEqualTo(value);
    }
    
    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtBuilderStage_ExtensionAssignment(bool value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        
        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetFullScreen(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert   
        await Assert.That(builder.Features.State.StartFullScreen).IsEqualTo(value);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.FullScreen).IsEqualTo(value);
    }
    
    [Test]
    [NotInParallelInfiniTests]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtWindowStage_DirectAssignment(bool value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.State.SetFullScreen(value);

        // Assert
        await Assert.That(window.Features.State.IsFullScreen).IsEqualTo(value);
    }
    
    [Test]
    [NotInParallelInfiniTests]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtWindowStage_ExtensionAssignment(bool value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        IInfiniFrameWindow returnedWindow = window.SetFullScreen(value);

        // Assert
        await Assert.That(window.Features.State.IsFullScreen).IsEqualTo(value);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }
    
    [Test]
    [NotInParallelInfiniTests]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtWindowStage_ThroughBuilderAssignment(bool value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder => {
            builder.Features.State.SetFullScreen(value);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        IInfiniFrameWindowBuilder builder = windowUtility.BuilderSnapshot;

        // Act

        // Assert
        await Assert.That(builder.Features.State.StartFullScreen).IsEqualTo(value);
        await Assert.That(window.Features.State.IsFullScreen).IsEqualTo(value);
    }
}
