// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Browser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class Win32SetWebView2PathTests {
    [Test]
    public async Task AtBuilderStage_DirectAssignment_PassesPathToNativeParameters(CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        const string path = "C:\\WebView2Runtime";

        // Act
        builder.Features.Browser.SetWebView2RuntimePath(path);
        InfiniFrameNativeParameters parameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Browser.WebView2RuntimePath).IsEqualTo(path);
        await Assert.That(parameters.WebView2RuntimePath).IsEqualTo(path);
    }

    [Test]
    public async Task AtBuilderStage_ExtensionAssignment_ReturnsBuilderAndPassesPathToNativeParameters(CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        const string path = "C:\\WebView2Runtime";

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetWebView2RuntimePath(path);
        InfiniFrameNativeParameters parameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(parameters.WebView2RuntimePath).IsEqualTo(path);
    }
}
