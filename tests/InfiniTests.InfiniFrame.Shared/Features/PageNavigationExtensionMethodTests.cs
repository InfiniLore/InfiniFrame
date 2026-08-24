// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Features;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class PageNavigationExtensionMethodTests {

    [Test]
    public async Task LoadUri_CallsFeature(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IPageNavigationInfiniFrameWindowFeature> nav = MockFactory.CreatePageNavigationMock();
        window.Features.Returns(features.Object);
        features.PageNavigation.Returns(nav.Object);

        // Act
        IInfiniFrameWindow result = window.Object.Load(new Uri("https://example.com"));

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task LoadString_CallsFeature(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IPageNavigationInfiniFrameWindowFeature> nav = MockFactory.CreatePageNavigationMock();
        window.Features.Returns(features.Object);
        features.PageNavigation.Returns(nav.Object);

        // Act
        IInfiniFrameWindow result = window.Object.Load("https://example.com");

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task LoadRawString_CallsFeature(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IPageNavigationInfiniFrameWindowFeature> nav = MockFactory.CreatePageNavigationMock();
        window.Features.Returns(features.Object);
        features.PageNavigation.Returns(nav.Object);

        // Act
        IInfiniFrameWindow result = window.Object.LoadRawString("<html></html>");

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task GetCurrentUrl_CallsFeature(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IPageNavigationInfiniFrameWindowFeature> nav = MockFactory.CreatePageNavigationMock();
        window.Features.Returns(features.Object);
        features.PageNavigation.Returns(nav.Object);
        nav.GetCurrentUrl().Returns("https://example.com");

        // Act
        string? url = window.Object.GetCurrentUrl();

        // Assert
        await Assert.That(url).IsEqualTo("https://example.com");
    }

    [Test]
    public async Task GetCurrentUri_CallsFeature(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IPageNavigationInfiniFrameWindowFeature> nav = MockFactory.CreatePageNavigationMock();
        window.Features.Returns(features.Object);
        features.PageNavigation.Returns(nav.Object);
        var expectedUri = new Uri("https://example.com");
        nav.GetCurrentUri().Returns(expectedUri);

        // Act
        Uri? uri = window.Object.GetCurrentUri();

        // Assert
        await Assert.That(uri).IsEqualTo(expectedUri);
    }
}
