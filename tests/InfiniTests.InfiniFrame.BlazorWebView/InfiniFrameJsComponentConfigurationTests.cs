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
    public async Task Constructor_SetsJSComponentsProperty(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWebViewManager> managerMock = MockFactory.CreateWebViewManagerMock();
        var jsComponents = new JSComponentConfigurationStore();
        Mock<ILogger<InfiniFrameJsComponentConfiguration>> loggerMock = MockFactory.CreateLoggerMock<InfiniFrameJsComponentConfiguration>();

        // Act
        var config = new InfiniFrameJsComponentConfiguration(managerMock.Object, jsComponents, loggerMock.Object);

        // Assert
        await Assert.That(config.JSComponents).IsSameReferenceAs(jsComponents);
    }

    [Test]
    public async Task LastAddComponentException_InitiallyNull(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWebViewManager> managerMock = MockFactory.CreateWebViewManagerMock();
        var jsComponents = new JSComponentConfigurationStore();
        Mock<ILogger<InfiniFrameJsComponentConfiguration>> loggerMock = MockFactory.CreateLoggerMock<InfiniFrameJsComponentConfiguration>();
        var config = new InfiniFrameJsComponentConfiguration(managerMock.Object, jsComponents, loggerMock.Object);

        // Act & Assert
        await Assert.That(config.LastAddComponentException).IsNull();
    }

    [Test]
    public async Task JSComponents_IsInitialized(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWebViewManager> managerMock = MockFactory.CreateWebViewManagerMock();
        var jsComponents = new JSComponentConfigurationStore();
        Mock<ILogger<InfiniFrameJsComponentConfiguration>> loggerMock = MockFactory.CreateLoggerMock<InfiniFrameJsComponentConfiguration>();

        // Act
        var config = new InfiniFrameJsComponentConfiguration(managerMock.Object, jsComponents, loggerMock.Object);

        // Assert
        await Assert.That(config.JSComponents).IsNotNull();
    }

    [Test]
    public async Task LastAddComponentException_IsAccessible(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWebViewManager> managerMock = MockFactory.CreateWebViewManagerMock();
        var jsComponents = new JSComponentConfigurationStore();
        Mock<ILogger<InfiniFrameJsComponentConfiguration>> loggerMock = MockFactory.CreateLoggerMock<InfiniFrameJsComponentConfiguration>();
        var config = new InfiniFrameJsComponentConfiguration(managerMock.Object, jsComponents, loggerMock.Object);

        // Act
        AggregateException? exception = config.LastAddComponentException;

        // Assert
        await Assert.That(exception).IsNull();
    }
}
