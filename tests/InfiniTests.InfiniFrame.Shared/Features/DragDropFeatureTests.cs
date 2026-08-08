// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using NSubstitute;

namespace InfiniTests.InfiniFrame.Shared.Features;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class DragDropFeatureTests {

    [Test]
    public async Task EnableDragDrop_SetsEnabledTrue(CancellationToken ct = default) {
        // Arrange
        var feature = Substitute.For<IDragDropInfiniFrameWindowFeature>();

        // Act
        feature.SetEnabled(true);

        // Assert
        feature.Received(1).SetEnabled(true);
    }

    [Test]
    public async Task DisableDragDrop_SetsEnabledFalse(CancellationToken ct = default) {
        // Arrange
        var feature = Substitute.For<IDragDropInfiniFrameWindowFeature>();

        // Act
        feature.SetEnabled(false);

        // Assert
        feature.Received(1).SetEnabled(false);
    }

    [Test]
    public async Task SetAllowedExtensions_StoresExtensions(CancellationToken ct = default) {
        // Arrange
        var feature = Substitute.For<IDragDropInfiniFrameWindowFeature>();
        string[] extensions = new[] { ".txt", ".png" };

        // Act
        feature.SetAllowedExtensions(extensions);

        // Assert
        feature.Received(1).SetAllowedExtensions(extensions);
    }

    [Test]
    public async Task IsEnabled_ReturnsCurrentState(CancellationToken ct = default) {
        // Arrange
        var feature = Substitute.For<IDragDropInfiniFrameWindowFeature>();
        feature.IsEnabled.Returns(true);

        // Act & Assert
        await Assert.That(feature.IsEnabled).IsTrue();
    }

    [Test]
    public async Task AllowedExtensions_ReturnsConfiguredExtensions(CancellationToken ct = default) {
        // Arrange
        var feature = Substitute.For<IDragDropInfiniFrameWindowFeature>();
        var extensions = new List<string> { ".txt", ".pdf" };
        feature.AllowedExtensions.Returns(extensions.AsReadOnly());

        // Act & Assert
        await Assert.That(feature.AllowedExtensions.Count).IsEqualTo(2);
        await Assert.That(feature.AllowedExtensions[0]).IsEqualTo(".txt");
        await Assert.That(feature.AllowedExtensions[1]).IsEqualTo(".pdf");
    }
}
