// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Features;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class PositionExtensionMethodTests {

    [Test]
    public async Task SetLocation_ReturnsWindowForChaining(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IPositionInfiniFrameWindowFeature> position = MockFactory.CreatePositionMock();
        window.Features.Returns(features.Object);
        features.Position.Returns(position.Object);

        // Act
        IInfiniFrameWindow result = window.Object.SetLocation(100, 200);

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task Center_ReturnsWindowForChaining(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IPositionInfiniFrameWindowFeature> position = MockFactory.CreatePositionMock();
        window.Features.Returns(features.Object);
        features.Position.Returns(position.Object);

        // Act
        IInfiniFrameWindow result = window.Object.Center();

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }
}
