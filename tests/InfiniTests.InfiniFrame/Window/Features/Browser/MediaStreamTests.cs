// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Browser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MediaStreamTests {

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtBuilderStage_DirectAssignment(bool value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        
        // Act
        builder.Features.Browser.EnableMediaStream(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert   
        await Assert.That(builder.Features.Browser.IsMediaStreamEnabled).IsEqualTo(value);
        await Assert.That(initParameters.MediaStreamEnabled).IsEqualTo(value);
    }
    
    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtBuilderStage_ExtensionAssignment(bool value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        
        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.EnableMediaStream(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert   
        await Assert.That(builder.Features.Browser.IsMediaStreamEnabled).IsEqualTo(value);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.MediaStreamEnabled).IsEqualTo(value);
    }
    
    // [Test]
    // [Arguments(true)]
    // [Arguments(false)]
    // public async Task AtWindowStage_DirectAssignment(bool value, CancellationToken ct) {
    //     // Arrange
    //     using var windowUtility = InfiniFrameTestWindow.Create(ct);
    //     IInfiniFrameWindow window = windowUtility.Window;
    //
    //     // Act
    //     window.Features.Browser.EnableMediaStream(value);
    //
    //     // Assert
    //     await Assert.That(window.Features.Browser.IsMediaStreamEnabled).IsEqualTo(value);
    // }
    //
    // [Test]
    // [Arguments(true)]
    // [Arguments(false)]
    // public async Task AtWindowStage_ExtensionAssignment(bool value, CancellationToken ct) {
    //     // Arrange
    //     using var windowUtility = InfiniFrameTestWindow.Create(ct);
    //     IInfiniFrameWindow window = windowUtility.Window;
    //
    //     // Act
    //     IInfiniFrameWindow returnedWindow = window.EnableMediaStream(value);
    //
    //     // Assert
    //     await Assert.That(window.Features.Browser.IsMediaStreamEnabled).IsEqualTo(value);
    //     await Assert.That(returnedWindow).IsSameReferenceAs(window);
    // }
    
    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtWindowStage_ThroughBuilderAssignment(bool value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder => {
            builder.Features.Browser.EnableMediaStream(value);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        IInfiniFrameWindowBuilder builder = windowUtility.BuilderSnapshot;

        // Act

        // Assert
        await Assert.That(builder.Features.Browser.IsMediaStreamEnabled).IsEqualTo(value);
        await Assert.That(window.Features.Browser.IsMediaStreamEnabled).IsEqualTo(value);
    }
}
