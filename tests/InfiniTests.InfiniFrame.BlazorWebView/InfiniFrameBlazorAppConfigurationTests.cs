// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Threading.Channels;
using InfiniFrame.BlazorWebView;

namespace InfiniTests.InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameBlazorAppConfigurationTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task AppBaseUri_Default_ShouldBeAppProtocol(CancellationToken ct = default) {
        // Arrange

        // Act
        var config = new InfiniFrameBlazorAppConfiguration();

        // Assert
        await Assert.That(config.AppBaseUri).IsNotNull();
        await Assert.That(config.AppBaseUri.Scheme).IsEqualTo("app");
    }

    [Test]
    public async Task HostPage_Default_ShouldBeIndexHtml(CancellationToken ct = default) {
        // Arrange

        // Act
        var config = new InfiniFrameBlazorAppConfiguration();

        // Assert
        await Assert.That(config.HostPage).IsEqualTo("index.html");
    }

    [Test]
    public async Task EnableGlobalUnhandledExceptionHandler_Default_ShouldBeTrue(CancellationToken ct = default) {
        // Arrange

        // Act
        var config = new InfiniFrameBlazorAppConfiguration();

        // Assert
        await Assert.That(config.EnableGlobalUnhandledExceptionHandler).IsTrue();
    }

    [Test]
    public async Task WebMessageQueueCapacity_Default_ShouldBe1024(CancellationToken ct = default) {
        // Arrange

        // Act
        var config = new InfiniFrameBlazorAppConfiguration();

        // Assert
        await Assert.That(config.WebMessageQueueCapacity).IsEqualTo(1024);
    }

    [Test]
    public async Task WebMessageQueueFullMode_Default_ShouldBeDropWrite(CancellationToken ct = default) {
        // Arrange

        // Act
        var config = new InfiniFrameBlazorAppConfiguration();

        // Assert
        await Assert.That(config.WebMessageQueueFullMode).IsEqualTo(BoundedChannelFullMode.DropWrite);
    }

    [Test]
    public async Task Properties_ShouldBeSettable(CancellationToken ct = default) {
        // Arrange
        var config = new InfiniFrameBlazorAppConfiguration();
        var customUri = new Uri("https://example.com/");

        // Act
        config.AppBaseUri = customUri;
        config.HostPage = "custom.html";
        config.EnableGlobalUnhandledExceptionHandler = false;
        config.WebMessageQueueCapacity = 512;
        config.WebMessageQueueFullMode = BoundedChannelFullMode.Wait;

        // Assert
        await Assert.That(config.AppBaseUri).IsSameReferenceAs(customUri);
        await Assert.That(config.HostPage).IsEqualTo("custom.html");
        await Assert.That(config.EnableGlobalUnhandledExceptionHandler).IsFalse();
        await Assert.That(config.WebMessageQueueCapacity).IsEqualTo(512);
        await Assert.That(config.WebMessageQueueFullMode).IsEqualTo(BoundedChannelFullMode.Wait);
    }
}
