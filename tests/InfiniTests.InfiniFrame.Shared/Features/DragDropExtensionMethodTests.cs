// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using NSubstitute;

namespace InfiniTests.InfiniFrame.Shared.Features;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class DragDropExtensionMethodTests {

    [Test]
    public async Task EnableDragDrop_CallsSetEnabled(CancellationToken ct = default) {
        // Arrange
        var window = Substitute.For<IInfiniFrameWindow>();
        var feature = Substitute.For<IDragDropInfiniFrameWindowFeature>();
        window.Features.DragDrop.Returns(feature);

        // Act
        var result = window.EnableDragDrop();

        // Assert
        feature.Received(1).SetEnabled(true);
        await Assert.That(result).IsEqualTo(window);
    }

    [Test]
    public async Task EnableDragDrop_WithExtensions_SetsEnabledAndExtensions(CancellationToken ct = default) {
        // Arrange
        var window = Substitute.For<IInfiniFrameWindow>();
        var feature = Substitute.For<IDragDropInfiniFrameWindowFeature>();
        window.Features.DragDrop.Returns(feature);

        // Act
        var result = window.EnableDragDrop(".txt", ".png");

        // Assert
        feature.Received(1).SetEnabled(true);
        feature.Received(1).SetAllowedExtensions(Arg.Is<string[]>(e => e != null && e.Length == 2 && e[0] == ".txt" && e[1] == ".png"));
        await Assert.That(result).IsEqualTo(window);
    }

    [Test]
    public async Task DisableDragDrop_CallsSetEnabledFalse(CancellationToken ct = default) {
        // Arrange
        var window = Substitute.For<IInfiniFrameWindow>();
        var feature = Substitute.For<IDragDropInfiniFrameWindowFeature>();
        window.Features.DragDrop.Returns(feature);

        // Act
        var result = window.DisableDragDrop();

        // Assert
        feature.Received(1).SetEnabled(false);
        await Assert.That(result).IsEqualTo(window);
    }

    [Test]
    public async Task OnFileDropped_RegistersHandlerOnEventsStore(CancellationToken ct = default) {
        // Arrange
        var window = Substitute.For<IInfiniFrameWindow>();
        var events = Substitute.For<IInfiniFrameEvents>();
        var eventsStore = new InfiniFrameEventsStore();
        window.Events.Returns(events);
        events.EventsStore.Returns(eventsStore);

        // Act
        var result = window.OnFileDropped((_, _) => { });

        // Assert
        await Assert.That(eventsStore.FileDropped.Snapshot.Length).IsEqualTo(1);
        await Assert.That(result).IsEqualTo(window);
    }
}
