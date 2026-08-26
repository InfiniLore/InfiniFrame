// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Features;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TaskbarExtensionMethodTests {

    [Test]
    public async Task SetTaskbarProgress_ReturnsWindowForChaining(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<ITaskbarInfiniFrameWindowFeature> taskbar = MockFactory.CreateTaskbarMock();
        window.Features.Returns(features.Object);
        features.Taskbar.Returns(taskbar.Object);

        // Act
        IInfiniFrameWindow result = window.Object.SetTaskbarProgress(TaskbarProgressState.Normal, 50, 100);

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task ClearTaskbarProgress_ReturnsWindowForChaining(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<ITaskbarInfiniFrameWindowFeature> taskbar = MockFactory.CreateTaskbarMock();
        window.Features.Returns(features.Object);
        features.Taskbar.Returns(taskbar.Object);

        // Act
        IInfiniFrameWindow result = window.Object.ClearTaskbarProgress();

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task FlashTaskbar_ReturnsWindowForChaining(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<ITaskbarInfiniFrameWindowFeature> taskbar = MockFactory.CreateTaskbarMock();
        window.Features.Returns(features.Object);
        features.Taskbar.Returns(taskbar.Object);

        // Act
        IInfiniFrameWindow result = window.Object.FlashTaskbar(TaskbarFlashMode.All);

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task StopTaskbarFlash_ReturnsWindowForChaining(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<ITaskbarInfiniFrameWindowFeature> taskbar = MockFactory.CreateTaskbarMock();
        window.Features.Returns(features.Object);
        features.Taskbar.Returns(taskbar.Object);

        // Act
        IInfiniFrameWindow result = window.Object.StopTaskbarFlash();

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }
}
