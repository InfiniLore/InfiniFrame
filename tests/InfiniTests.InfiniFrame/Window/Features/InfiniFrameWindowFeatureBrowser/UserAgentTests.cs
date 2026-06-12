// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.InfiniFrameWindowFeatureBrowser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class UserAgentTests {

    [Test]
    [Arguments("userAgentName")]
    public async Task AtBuilderStage_DirectAssignment_HappyFlow(string value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.Browser.SetUserAgent(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert   
        await Assert.That(builder.Features.Browser.UserAgent).IsEqualTo(value);
        await Assert.That(initParameters.UserAgent).IsEqualTo(value);
    }
    
    [Test]
    [Arguments(null, "")]
    [Arguments("", "")]
    [Arguments(" ", "")]
    public async Task AtBuilderStage_DirectAssignment_UnHappyFlow(string? value, string? expectedValue, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.Browser.SetUserAgent(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert   
        await Assert.That(builder.Features.Browser.UserAgent).IsEqualTo(expectedValue);
        await Assert.That(initParameters.UserAgent).IsEqualTo(expectedValue);
    }

    [Test]
    [Arguments("userAgentName")]
    public async Task AtBuilderStage_ExtensionAssignment_HappyFlow(string value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetUserAgent(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert   
        await Assert.That(builder.Features.Browser.UserAgent).IsEqualTo(value);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.UserAgent).IsEqualTo(value);
    }

    [Test]
    [Arguments(null, "")]
    [Arguments("", "")]
    [Arguments(" ", "")]
    public async Task AtBuilderStage_ExtensionAssignment_UnHappyFlow(string? value, string? expectedValue, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetUserAgent(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert   
        await Assert.That(builder.Features.Browser.UserAgent).IsEqualTo(expectedValue);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.UserAgent).IsEqualTo(expectedValue);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments("userAgentName")]
    public async Task AtWindowStage_DirectAssignment(string value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.Browser.SetUserAgent(value);

        // Assert
        await Assert.That(window.Features.Browser.UserAgent).IsEqualTo(value);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments("userAgentName")]
    public async Task AtWindowStage_ExtensionAssignment(string value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        IInfiniFrameWindow returnedWindow = window.SetUserAgent(value);

        // Assert
        await Assert.That(window.Features.Browser.UserAgent).IsEqualTo(value);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments("userAgentName")]
    public async Task AtWindowStage_ThroughBuilderAssignment_HappyFlow(string value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Browser.SetUserAgent(value);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        IInfiniFrameWindowBuilder builder = windowUtility.BuilderSnapshot;

        // Act

        // Assert
        await Assert.That(builder.Features.Browser.UserAgent).IsEqualTo(value);
        await Assert.That(window.Features.Browser.UserAgent).IsEqualTo(value);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(null, "")]
    [Arguments("", "")]
    [Arguments(" ", "")]
    public async Task AtWindowStage_ThroughBuilderAssignment_UnHappyFlow(string? value, string? expectedValue, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Browser.SetUserAgent(value);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        IInfiniFrameWindowBuilder builder = windowUtility.BuilderSnapshot;

        // Act

        // Assert
        await Assert.That(builder.Features.Browser.UserAgent).IsEqualTo(expectedValue);
        await Assert.That(window.Features.Browser.UserAgent).IsEqualTo(expectedValue);
    }
}
