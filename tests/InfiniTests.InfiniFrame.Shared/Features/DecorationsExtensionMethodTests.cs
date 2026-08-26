// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Features;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class DecorationsExtensionMethodTests {

    [Test]
    public async Task SetTransparent_CallsFeature(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IDecorationsInfiniFrameWindowFeature> decorations = MockFactory.CreateDecorationsMock();
        window.Features.Returns(features.Object);
        features.Decorations.Returns(decorations.Object);

        // Act
        IInfiniFrameWindow result = window.Object.SetTransparent();

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task SetBackgroundColor_CallsFeature(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IDecorationsInfiniFrameWindowFeature> decorations = MockFactory.CreateDecorationsMock();
        window.Features.Returns(features.Object);
        features.Decorations.Returns(decorations.Object);

        // Act
        IInfiniFrameWindow result = window.Object.SetBackgroundColor("#FF0000");

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task SetTitle_CallsFeature(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IDecorationsInfiniFrameWindowFeature> decorations = MockFactory.CreateDecorationsMock();
        window.Features.Returns(features.Object);
        features.Decorations.Returns(decorations.Object);

        // Act
        IInfiniFrameWindow result = window.Object.SetTitle("My Window");

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task SetIconFile_CallsFeature(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IDecorationsInfiniFrameWindowFeature> decorations = MockFactory.CreateDecorationsMock();
        window.Features.Returns(features.Object);
        features.Decorations.Returns(decorations.Object);

        // Act
        IInfiniFrameWindow result = window.Object.SetIconFile("/path/to/icon.png");

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task SetLimitLinuxWindowTitleLength_CallsFeature(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IDecorationsInfiniFrameWindowFeature> decorations = MockFactory.CreateDecorationsMock();
        window.Features.Returns(features.Object);
        features.Decorations.Returns(decorations.Object);

        // Act
        IInfiniFrameWindow result = window.Object.SetLimitLinuxWindowTitleLength();

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }
}
