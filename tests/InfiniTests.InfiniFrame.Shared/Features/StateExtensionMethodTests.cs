// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Features;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class StateExtensionMethodTests {

    [Test]
    public async Task SetMaximized_ReturnsWindowForChaining(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IStateInfiniFrameWindowFeature> state = MockFactory.CreateStateMock();
        window.Features.Returns(features.Object);
        features.State.Returns(state.Object);

        // Act
        IInfiniFrameWindow result = window.Object.SetMaximized();

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task SetMinimized_ReturnsWindowForChaining(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IStateInfiniFrameWindowFeature> state = MockFactory.CreateStateMock();
        window.Features.Returns(features.Object);
        features.State.Returns(state.Object);

        // Act
        IInfiniFrameWindow result = window.Object.SetMinimized();

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task SetFullScreen_ReturnsWindowForChaining(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IStateInfiniFrameWindowFeature> state = MockFactory.CreateStateMock();
        window.Features.Returns(features.Object);
        features.State.Returns(state.Object);

        // Act
        IInfiniFrameWindow result = window.Object.SetFullScreen();

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }
}
