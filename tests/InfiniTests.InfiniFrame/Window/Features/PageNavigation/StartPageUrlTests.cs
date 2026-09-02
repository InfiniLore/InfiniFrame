// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.PageNavigation;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class StartPageUrlTests {
    [Test]
    [Arguments("https://example.com/a")]
    [Arguments("https://example.com/b")]
    public async Task AtBuilderStage_DirectAssignment(string value, CancellationToken ct) {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();

        // Act
        builder.Features.PageNavigation.SetStartPageUrl(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.PageNavigation.StartUrl).IsEqualTo(value);
        await Assert.That(initParameters.StartUrl).IsEqualTo(value);
    }

    [Test]
    [Arguments("https://example.com/c")]
    [Arguments("https://example.com/d")]
    public async Task AtBuilderStage_ExtensionAssignment(string value, CancellationToken ct) {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetStartPageUrl(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.PageNavigation.StartUrl).IsEqualTo(value);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.StartUrl).IsEqualTo(value);
    }

    [Test]
    [Arguments("https://example.com/e")]
    [Arguments("https://example.com/f")]
    public async Task AtBuilderStage_UriAssignment(string value, CancellationToken ct) {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();
        Uri uri = new(value);

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetUrl(uri);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.PageNavigation.StartUrl).IsEqualTo(uri.ToString());
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.StartUrl).IsEqualTo(uri.ToString());
    }
}
