// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Features;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class DragDropFeatureTests {

    [Test]
    public async Task EnableDragDrop_SetsEnabledTrue(CancellationToken ct = default) {
        // Arrange
        Mock<IDragDropInfiniFrameWindowFeature> feature = MockFactory.CreateDragDropMock();

        // Act
        feature.Object.SetEnabled(true);

        // Assert
        feature.SetEnabled(true).WasCalled(Times.Once);
    }

    [Test]
    public async Task DisableDragDrop_SetsEnabledFalse(CancellationToken ct = default) {
        // Arrange
        Mock<IDragDropInfiniFrameWindowFeature> feature = MockFactory.CreateDragDropMock();

        // Act
        feature.Object.SetEnabled(false);

        // Assert
        feature.SetEnabled(false).WasCalled(Times.Once);
    }

    [Test]
    public async Task SetAllowedExtensions_StoresExtensions(CancellationToken ct = default) {
        // Arrange
        Mock<IDragDropInfiniFrameWindowFeature> feature = MockFactory.CreateDragDropMock();
        string[] extensions = new[] { ".txt", ".png" };

        // Act
        feature.Object.SetAllowedExtensions(extensions);

        // Assert
        feature.SetAllowedExtensions(extensions).WasCalled(Times.Once);
    }

    [Test]
    public async Task IsEnabled_ReturnsCurrentState(CancellationToken ct = default) {
        // Arrange
        Mock<IDragDropInfiniFrameWindowFeature> feature = MockFactory.CreateDragDropMock();
        feature.IsEnabled.Returns(true);

        // Act & Assert
        await Assert.That(feature.Object.IsEnabled).IsTrue();
    }

    [Test]
    public async Task AllowedExtensions_ReturnsConfiguredExtensions(CancellationToken ct = default) {
        // Arrange
        Mock<IDragDropInfiniFrameWindowFeature> feature = MockFactory.CreateDragDropMock();
        var extensions = new List<string> { ".txt", ".pdf" };
        feature.AllowedExtensions.Returns(extensions.AsReadOnly());

        // Act & Assert
        await Assert.That(feature.Object.AllowedExtensions.Count).IsEqualTo(2);
        await Assert.That(feature.Object.AllowedExtensions[0]).IsEqualTo(".txt");
        await Assert.That(feature.Object.AllowedExtensions[1]).IsEqualTo(".pdf");
    }
}
