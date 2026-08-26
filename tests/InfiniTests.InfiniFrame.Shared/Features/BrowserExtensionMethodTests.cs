// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Features;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class BrowserExtensionMethodTests {

    [Test]
    public async Task EnableStatusBar_ReturnsWindowForChaining(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IBrowserInfiniFrameWindowFeature> browser = MockFactory.CreateBrowserMock();
        window.Features.Returns(features.Object);
        features.Browser.Returns(browser.Object);

        // Act
        IInfiniFrameWindow result = window.Object.EnableStatusBar();

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task EnableBrowserShortcuts_ReturnsWindowForChaining(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IBrowserInfiniFrameWindowFeature> browser = MockFactory.CreateBrowserMock();
        window.Features.Returns(features.Object);
        features.Browser.Returns(browser.Object);

        // Act
        IInfiniFrameWindow result = window.Object.EnableBrowserShortcuts(false);

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task EnableContextMenu_ReturnsWindowForChaining(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IBrowserInfiniFrameWindowFeature> browser = MockFactory.CreateBrowserMock();
        window.Features.Returns(features.Object);
        features.Browser.Returns(browser.Object);

        // Act
        IInfiniFrameWindow result = window.Object.EnableContextMenu(false);

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task EnableMediaAutoplay_ReturnsWindowForChaining(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IBrowserInfiniFrameWindowFeature> browser = MockFactory.CreateBrowserMock();
        window.Features.Returns(features.Object);
        features.Browser.Returns(browser.Object);

        // Act
        IInfiniFrameWindow result = window.Object.EnableMediaAutoplay(false);

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task SetUserAgent_ReturnsWindowForChaining(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IBrowserInfiniFrameWindowFeature> browser = MockFactory.CreateBrowserMock();
        window.Features.Returns(features.Object);
        features.Browser.Returns(browser.Object);

        // Act
        IInfiniFrameWindow result = window.Object.SetUserAgent("CustomAgent/1.0");

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }
}
