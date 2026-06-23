// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Browser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class UserAgentTests {

    [Test]
    [Arguments("userAgentName", "userAgentName")]
    [Arguments(null, "")]
    [Arguments("", "")]
    [Arguments(" ", "")]
    public async Task AtBuilderStage_DirectAssignment(string? value, string? expected, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.Browser.SetUserAgent(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert   
        await Assert.That(builder.Features.Browser.UserAgent).IsEqualTo(expected);
        await Assert.That(initParameters.UserAgent).IsEqualTo(expected);
    }

    [Test]
    [Arguments("userAgentName", "userAgentName")]
    [Arguments(null, "")]
    [Arguments("", "")]
    [Arguments(" ", "")]
    public async Task AtBuilderStage_ExtensionAssignment(string? value, string? expected, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetUserAgent(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert   
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        
        await Assert.That(builder.Features.Browser.UserAgent).IsEqualTo(expected);
        await Assert.That(initParameters.UserAgent).IsEqualTo(expected);
    }

    [Test]
    [Arguments("userAgentName", "userAgentName")]
    [Arguments(null, "")]
    [Arguments("", "")]
    [Arguments(" ", "")]
    public async Task AtWindowStage_DirectAssignment(string? value, string? expected, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.Browser.SetUserAgent(value);

        // Assert
        await Assert.That(window.Features.Browser.UserAgent).IsEqualTo(expected);
    }

    [Test]
    [Arguments("userAgentName", "userAgentName")]
    [Arguments(null, "")]
    [Arguments("", "")]
    [Arguments(" ", "")]
    public async Task AtWindowStage_ExtensionAssignment(string? value, string? expected, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        IInfiniFrameWindow returnedWindow = window.SetUserAgent(value);

        // Assert
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
        
        await Assert.That(window.Features.Browser.UserAgent).IsEqualTo(expected);
    }
    
    [Test]
    [Arguments("userAgentName", "userAgentName")]
    [Arguments(null, "")]
    [Arguments("", "")]
    [Arguments(" ", "")]
    public async Task AtWindowStage_ThroughBuilderAssignment(string? value, string? expected, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder => {
            builder.Features.Browser.SetUserAgent(value);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        IInfiniFrameWindowBuilder builder = windowUtility.BuilderSnapshot;

        // Act

        // Assert
        await Assert.That(builder.Features.Browser.UserAgent).IsEqualTo(expected);
        await Assert.That(window.Features.Browser.UserAgent).IsEqualTo(expected);
    }
}
