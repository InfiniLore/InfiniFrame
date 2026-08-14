// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Features;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class DragDropExtensionMethodTests {

    [Test]
    public async Task EnableDragDrop_CallsSetEnabled(CancellationToken ct = default) {
        // Arrange
        var window = MockFactory.CreateWindowMock();
        var features = MockFactory.CreateFeaturesMock();
        var feature = MockFactory.CreateDragDropMock();
        window.Features.Returns(features.Object);
        features.DragDrop.Returns(feature.Object);

        // Act
        IInfiniFrameWindow result = window.Object.EnableDragDrop();

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task EnableDragDrop_WithExtensions_SetsEnabledAndExtensions(CancellationToken ct = default) {
        // Arrange
        var window = MockFactory.CreateWindowMock();
        var features = MockFactory.CreateFeaturesMock();
        var feature = MockFactory.CreateDragDropMock();
        window.Features.Returns(features.Object);
        features.DragDrop.Returns(feature.Object);

        // Act
        IInfiniFrameWindow result = window.Object.EnableDragDrop(".txt", ".png");

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task DisableDragDrop_CallsSetEnabledFalse(CancellationToken ct = default) {
        // Arrange
        var window = MockFactory.CreateWindowMock();
        var features = MockFactory.CreateFeaturesMock();
        var feature = MockFactory.CreateDragDropMock();
        window.Features.Returns(features.Object);
        features.DragDrop.Returns(feature.Object);

        // Act
        IInfiniFrameWindow result = window.Object.DisableDragDrop();

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task OnFileDropped_RegistersHandlerOnEventsStore(CancellationToken ct = default) {
        // Arrange
        var window = MockFactory.CreateWindowMock();
        var events = MockFactory.CreateEventsMock();
        var eventsStore = new InfiniFrameEventsStore();
        window.Events.Returns(events.Object);
        events.EventsStore.Returns(eventsStore);

        // Act
        IInfiniFrameWindow result = window.Object.OnFileDropped((_, _) => { });

        // Assert
        await Assert.That(eventsStore.FileDropped.Snapshot.Length).IsEqualTo(1);
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }
}
