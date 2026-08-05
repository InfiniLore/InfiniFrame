// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using NSubstitute;
using System.Drawing;

namespace InfiniTests.InfiniFrame.Shared.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class FileDroppedEventTests {

    [Test]
    public async Task FileDropped_EventFires_WhenHandlerRegistered(CancellationToken ct = default) {
        // Arrange
        var eventsStore = new InfiniFrameEventsStore();
        var window = Substitute.For<IInfiniFrameWindow>();
        FileDroppedEventArgs? receivedArgs = null;

        eventsStore.FileDropped.Add((_, args) => receivedArgs = args);

        var files = new[] { "/test/file.txt" };
        var location = new Point(10, 20);
        var args = new FileDroppedEventArgs(files, location);

        // Act
        eventsStore.FileDropped.Invoke(window, args);

        // Assert
        await Assert.That(receivedArgs).IsNotNull();
        await Assert.That(receivedArgs!.Files).IsEquivalentTo(files);
        await Assert.That(receivedArgs.DropLocation).IsEqualTo(location);
    }

    [Test]
    public async Task FileDropped_MultipleHandlers_AllInvoked(CancellationToken ct = default) {
        // Arrange
        var eventsStore = new InfiniFrameEventsStore();
        var window = Substitute.For<IInfiniFrameWindow>();
        int handlerCount = 0;

        eventsStore.FileDropped.Add((_, _) => handlerCount++);
        eventsStore.FileDropped.Add((_, _) => handlerCount++);

        var args = new FileDroppedEventArgs(new[] { "file.txt" }, Point.Empty);

        // Act
        eventsStore.FileDropped.Invoke(window, args);

        // Assert
        await Assert.That(handlerCount).IsEqualTo(2);
    }

    [Test]
    public async Task FileDropped_HandlerReceivesCorrectWindow(CancellationToken ct = default) {
        // Arrange
        var eventsStore = new InfiniFrameEventsStore();
        var window = Substitute.For<IInfiniFrameWindow>();
        IInfiniFrameWindow? receivedWindow = null;

        eventsStore.FileDropped.Add((w, _) => receivedWindow = w);

        var args = new FileDroppedEventArgs(new[] { "file.txt" }, Point.Empty);

        // Act
        eventsStore.FileDropped.Invoke(window, args);

        // Assert
        await Assert.That(receivedWindow).IsEqualTo(window);
    }

    [Test]
    public async Task CopyTo_CopiesFileDroppedHandlers(CancellationToken ct = default) {
        // Arrange
        var source = new InfiniFrameEventsStore();
        var target = new InfiniFrameEventsStore();
        bool handlerCalled = false;

        source.FileDropped.Add((_, _) => handlerCalled = true);

        // Act
        source.CopyTo(target);

        var args = new FileDroppedEventArgs(new[] { "file.txt" }, Point.Empty);
        target.FileDropped.Invoke(Substitute.For<IInfiniFrameWindow>(), args);

        // Assert
        await Assert.That(handlerCalled).IsTrue();
    }
}
