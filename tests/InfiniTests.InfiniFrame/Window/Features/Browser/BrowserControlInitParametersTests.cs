// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Browser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class BrowserControlInitParametersTests {
    [Test]
    [Arguments("--autoplay-policy=no-user-gesture-required")]
    [Arguments("--disable-web-security")]
    public async Task AtBuilderStage_DirectAssignment(string value, CancellationToken ct) {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();

        // Act
        builder.Features.Browser.SetBrowserControlInitParameters(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Browser.BrowserControlInitParameters).IsEqualTo(value);
        await Assert.That(initParameters.BrowserControlInitParameters).IsEqualTo(value);
    }

    [Test]
    [Arguments("--autoplay-policy=no-user-gesture-required")]
    [Arguments("--disable-features=IsolateOrigins")]
    public async Task AtBuilderStage_ExtensionAssignment(string value, CancellationToken ct) {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetBrowserControlInitParameters(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Browser.BrowserControlInitParameters).IsEqualTo(value);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.BrowserControlInitParameters).IsEqualTo(value);
    }
}
