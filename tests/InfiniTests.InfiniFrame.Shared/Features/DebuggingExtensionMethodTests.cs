// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Features;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class DebuggingExtensionMethodTests {

    [Test]
    public async Task EnableDevTools_ReturnsWindowForChaining(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IDebuggingInfiniFrameWindowFeature> debugging = MockFactory.CreateDebuggingMock();
        window.Features.Returns(features.Object);
        features.Debugging.Returns(debugging.Object);

        // Act
        IInfiniFrameWindow result = window.Object.EnableDevTools();

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task SupportsWebInspectorAttach_ReturnsValue(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IDebuggingInfiniFrameWindowFeature> debugging = MockFactory.CreateDebuggingMock();
        window.Features.Returns(features.Object);
        features.Debugging.Returns(debugging.Object);
        debugging.SupportsWebInspectorAttach.Returns(true);

        // Act
        bool result = window.Object.SupportsWebInspectorAttach();

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task SupportsRemoteDebuggingEndpoint_ReturnsValue(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IDebuggingInfiniFrameWindowFeature> debugging = MockFactory.CreateDebuggingMock();
        window.Features.Returns(features.Object);
        features.Debugging.Returns(debugging.Object);
        debugging.SupportsRemoteDebuggingEndpoint.Returns(false);

        // Act
        bool result = window.Object.SupportsRemoteDebuggingEndpoint();

        // Assert
        await Assert.That(result).IsFalse();
    }
}
