// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BlazorWebView;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;

namespace InfiniTests.InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameJsComponentConfigurationTests {

    [Test]
    public async Task Constructor_CanBeInstantiated(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWebViewManager> manager = MockFactory.CreateWebViewManagerMock();
        Mock<ILogger<InfiniFrameJsComponentConfiguration>> logger = MockFactory.CreateLoggerMock<InfiniFrameJsComponentConfiguration>();
        var store = new JSComponentConfigurationStore();

        // Act
        var config = new InfiniFrameJsComponentConfiguration(manager.Object, store, logger.Object);

        // Assert
        await Assert.That(config).IsNotNull();
        await Assert.That(config.JSComponents).IsSameReferenceAs(store);
    }

    [Test]
    public async Task LastAddComponentException_InitiallyNull(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWebViewManager> manager = MockFactory.CreateWebViewManagerMock();
        Mock<ILogger<InfiniFrameJsComponentConfiguration>> logger = MockFactory.CreateLoggerMock<InfiniFrameJsComponentConfiguration>();
        var store = new JSComponentConfigurationStore();
        var config = new InfiniFrameJsComponentConfiguration(manager.Object, store, logger.Object);

        // Act & Assert
        await Assert.That(config.LastAddComponentException).IsNull();
    }
}
