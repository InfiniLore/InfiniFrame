// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Features;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SizeExtensionMethodTests {

    [Test]
    public async Task SetSize_CallsFeature(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<ISizeInfiniFrameWindowFeature> size = MockFactory.CreateSizeMock();
        window.Features.Returns(features.Object);
        features.Size.Returns(size.Object);

        // Act
        IInfiniFrameWindow result = window.Object.SetSize(800, 600);

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task SetMinSize_CallsFeature(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<ISizeInfiniFrameWindowFeature> size = MockFactory.CreateSizeMock();
        window.Features.Returns(features.Object);
        features.Size.Returns(size.Object);

        // Act
        IInfiniFrameWindow result = window.Object.SetMinSize(400, 300);

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task SetMaxSize_CallsFeature(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<ISizeInfiniFrameWindowFeature> size = MockFactory.CreateSizeMock();
        window.Features.Returns(features.Object);
        features.Size.Returns(size.Object);

        // Act
        IInfiniFrameWindow result = window.Object.SetMaxSize(1920, 1080);

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }
}
