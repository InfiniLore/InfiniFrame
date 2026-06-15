// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Browser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WebSecurityTests {

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtBuilderStage_DirectAssignment(bool value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        
        // Act
        builder.Features.Browser.EnableWebSecurity(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert   
        await Assert.That(builder.Features.Browser.IsWebSecurityEnabled).IsEqualTo(value);
        await Assert.That(initParameters.WebSecurityEnabled).IsEqualTo(value);
    }
    
    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtBuilderStage_ExtensionAssignment(bool value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        
        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.EnableWebSecurity(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert   
        await Assert.That(builder.Features.Browser.IsWebSecurityEnabled).IsEqualTo(value);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.WebSecurityEnabled).IsEqualTo(value);
    }
    
    // [Test]
    // [NotInParallelInfiniTests]
    // [Arguments(true)]
    // [Arguments(false)]
    // public async Task AtWindowStage_DirectAssignment(bool value, CancellationToken ct) {
    //     // Arrange
    //     using var windowUtility = InfiniFrameTestWindow.Create(ct);
    //     IInfiniFrameWindow window = windowUtility.Window;
    //
    //     // Act
    //     window.Features.Browser.EnableWebSecurity(value);
    //
    //     // Assert
    //     await Assert.That(window.Features.Browser.IsWebSecurityEnabled).IsEqualTo(value);
    // }
    //
    // [Test]
    // [NotInParallelInfiniTests]
    // [Arguments(true)]
    // [Arguments(false)]
    // public async Task AtWindowStage_ExtensionAssignment(bool value, CancellationToken ct) {
    //     // Arrange
    //     using var windowUtility = InfiniFrameTestWindow.Create(ct);
    //     IInfiniFrameWindow window = windowUtility.Window;
    //
    //     // Act
    //     IInfiniFrameWindow returnedWindow = window.EnableWebSecurity(value);
    //
    //     // Assert
    //     await Assert.That(window.Features.Browser.IsWebSecurityEnabled).IsEqualTo(value);
    //     await Assert.That(returnedWindow).IsSameReferenceAs(window);
    // }
    
    [Test]
    [NotInParallelInfiniTests]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtWindowStage_ThroughBuilderAssignment(bool value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder => {
            builder.Features.Browser.EnableWebSecurity(value);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        IInfiniFrameWindowBuilder builder = windowUtility.BuilderSnapshot;

        // Act

        // Assert
        await Assert.That(builder.Features.Browser.IsWebSecurityEnabled).IsEqualTo(value);
        await Assert.That(window.Features.Browser.IsWebSecurityEnabled).IsEqualTo(value);
    }
}
