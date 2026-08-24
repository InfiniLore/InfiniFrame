// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class HasInfiniFrameEventsStoreExtensionTests {

    [Test]
    public async Task RegisterLocationChangedHandler_AddsHandler(CancellationToken ct = default) {
        // Arrange
        var target = new TestHasEventsStore();

        // Act
        target.RegisterLocationChangedHandler((_, _) => {});

        // Assert
        int count = target.EventsStore.WindowLocationChanged.Snapshot.Length;
        await Assert.That(count).IsEqualTo(1);
    }

    [Test]
    public async Task RegisterSizeChangedHandler_AddsHandler(CancellationToken ct = default) {
        // Arrange
        var target = new TestHasEventsStore();

        // Act
        target.RegisterSizeChangedHandler((_, _) => {});

        // Assert
        int count = target.EventsStore.WindowSizeChanged.Snapshot.Length;
        await Assert.That(count).IsEqualTo(1);
    }

    [Test]
    public async Task RegisterFocusInHandler_AddsHandler(CancellationToken ct = default) {
        // Arrange
        var target = new TestHasEventsStore();

        // Act
        target.RegisterFocusInHandler(_ => {});

        // Assert
        int count = target.EventsStore.WindowFocusIn.Snapshot.Length;
        await Assert.That(count).IsEqualTo(1);
    }

    [Test]
    public async Task RegisterMaximizedHandler_AddsHandler(CancellationToken ct = default) {
        // Arrange
        var target = new TestHasEventsStore();

        // Act
        target.RegisterMaximizedHandler(_ => {});

        // Assert
        int count = target.EventsStore.WindowMaximized.Snapshot.Length;
        await Assert.That(count).IsEqualTo(1);
    }

    [Test]
    public async Task RegisterRestoredHandler_AddsHandler(CancellationToken ct = default) {
        // Arrange
        var target = new TestHasEventsStore();

        // Act
        target.RegisterRestoredHandler(_ => {});

        // Assert
        int count = target.EventsStore.WindowRestored.Snapshot.Length;
        await Assert.That(count).IsEqualTo(1);
    }

    [Test]
    public async Task RegisterFocusOutHandler_AddsHandler(CancellationToken ct = default) {
        // Arrange
        var target = new TestHasEventsStore();

        // Act
        target.RegisterFocusOutHandler(_ => {});

        // Assert
        int count = target.EventsStore.WindowFocusOut.Snapshot.Length;
        await Assert.That(count).IsEqualTo(1);
    }

    [Test]
    public async Task RegisterMinimizedHandler_AddsHandler(CancellationToken ct = default) {
        // Arrange
        var target = new TestHasEventsStore();

        // Act
        target.RegisterMinimizedHandler(_ => {});

        // Assert
        int count = target.EventsStore.WindowMinimized.Snapshot.Length;
        await Assert.That(count).IsEqualTo(1);
    }

    [Test]
    public async Task RegisterWebMessageReceivedHandler_AddsHandler(CancellationToken ct = default) {
        // Arrange
        var target = new TestHasEventsStore();

        // Act
        target.RegisterWebMessageReceivedHandler((_, _) => {});

        // Assert
        int count = target.EventsStore.WebMessageReceived.Snapshot.Length;
        await Assert.That(count).IsEqualTo(1);
    }

    [Test]
    public async Task RegisterNavigationStartingHandler_AddsHandler(CancellationToken ct = default) {
        // Arrange
        var target = new TestHasEventsStore();

        // Act
        target.RegisterNavigationStartingHandler((_, _) => NavigationStartingResult.Allow);

        // Assert
        int count = target.EventsStore.NavigationStarting.Snapshot.Length;
        await Assert.That(count).IsEqualTo(1);
    }

    [Test]
    public async Task RegisterWindowClosingRequestedHandler_AddsHandler(CancellationToken ct = default) {
        // Arrange
        var target = new TestHasEventsStore();

        // Act
        target.RegisterWindowClosingRequestedHandler(_ => {});

        // Assert
        int count = target.EventsStore.WindowClosingRequested.Snapshot.Length;
        await Assert.That(count).IsEqualTo(1);
    }

    [Test]
    public async Task RegisterWindowClosingHandler_AddsHandler(CancellationToken ct = default) {
        // Arrange
        var target = new TestHasEventsStore();

        // Act
        target.RegisterWindowClosingHandler((_, _) => default);

        // Assert
        int count = target.EventsStore.Closing.Snapshot.Length;
        await Assert.That(count).IsEqualTo(1);
    }

    [Test]
    public async Task RegisterWindowCreatingHandler_AddsHandler(CancellationToken ct = default) {
        // Arrange
        var target = new TestHasEventsStore();

        // Act
        target.RegisterWindowCreatingHandler(_ => {});

        // Assert
        int count = target.EventsStore.WindowCreating.Snapshot.Length;
        await Assert.That(count).IsEqualTo(1);
    }

    [Test]
    public async Task RegisterWindowCreatedHandler_AddsHandler(CancellationToken ct = default) {
        // Arrange
        var target = new TestHasEventsStore();

        // Act
        target.RegisterWindowCreatedHandler(_ => {});

        // Assert
        int count = target.EventsStore.WindowCreated.Snapshot.Length;
        await Assert.That(count).IsEqualTo(1);
    }

    [Test]
    public async Task RegisterWindowClosedHandler_AddsHandler(CancellationToken ct = default) {
        // Arrange
        var target = new TestHasEventsStore();

        // Act
        target.RegisterWindowClosedHandler(_ => {});

        // Assert
        int count = target.EventsStore.WindowClosed.Snapshot.Length;
        await Assert.That(count).IsEqualTo(1);
    }

    [Test]
    public async Task RegisterWebMessagePostHandler_AddsHandler(CancellationToken ct = default) {
        // Arrange
        var target = new TestHasEventsStore();

        // Act
        target.RegisterWebMessagePostHandler("msg_id", handler: (_, _) => {});

        // Assert
        bool contains = target.EventsStore.WebMessagePostData.ContainsKey("msg_id");
        await Assert.That(contains).IsTrue();
    }

    [Test]
    public async Task RegisterWebMessageGetHandler_AddsHandler(CancellationToken ct = default) {
        // Arrange
        var target = new TestHasEventsStore();

        // Act
        target.RegisterWebMessageGetHandler("msg_id", handler: (_, _) => "response");

        // Assert
        bool contains = target.EventsStore.WebMessageGetData.ContainsKey("msg_id");
        await Assert.That(contains).IsTrue();
    }

    [Test]
    public async Task RegisterFileDroppedHandler_AddsHandler(CancellationToken ct = default) {
        // Arrange
        var target = new TestHasEventsStore();

        // Act
        target.RegisterFileDroppedHandler((_, _) => {});

        // Assert
        int count = target.EventsStore.FileDropped.Snapshot.Length;
        await Assert.That(count).IsEqualTo(1);
    }

    [Test]
    public async Task MultipleHandlers_CanBeRegistered(CancellationToken ct = default) {
        // Arrange
        var target = new TestHasEventsStore();

        // Act
        target.RegisterFocusInHandler(_ => {});
        target.RegisterFocusInHandler(_ => {});
        target.RegisterFocusInHandler(_ => {});

        // Assert
        int count = target.EventsStore.WindowFocusIn.Snapshot.Length;
        await Assert.That(count).IsEqualTo(3);
    }

    private class TestHasEventsStore : IHasInfiniFrameEventsStore {
        public IInfiniFrameEventsStore EventsStore { get; } = new InfiniFrameEventsStore();
    }
}
