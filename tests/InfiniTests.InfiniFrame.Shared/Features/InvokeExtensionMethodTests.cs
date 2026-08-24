// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Features;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InvokeExtensionMethodTests {

    [Test]
    public async Task Invoke_ReturnsWindowForChaining(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IInvokeInfiniFrameWindowFeature> invoke = MockFactory.CreateInvokeMock();
        window.Features.Returns(features.Object);
        features.Invoke.Returns(invoke.Object);

        // Act
        IInfiniFrameWindow result = window.Object.Invoke(() => { });

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }
}
