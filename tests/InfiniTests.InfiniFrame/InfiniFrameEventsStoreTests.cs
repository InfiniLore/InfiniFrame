// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameEventsStoreTests {

    [Test]
    public async Task AllEventProperties_AreInitialized(CancellationToken ct = default) {
        // Arrange & Act
        var store = new InfiniFrameEventsStore();

        // Assert
        await Assert.That(store.WindowLocationChanged).IsNotNull();
        await Assert.That(store.WindowSizeChanged).IsNotNull();
        await Assert.That(store.WindowFocusIn).IsNotNull();
        await Assert.That(store.WindowMaximized).IsNotNull();
        await Assert.That(store.WindowRestored).IsNotNull();
        await Assert.That(store.WindowFocusOut).IsNotNull();
        await Assert.That(store.WindowMinimized).IsNotNull();
        await Assert.That(store.WindowClosingRequested).IsNotNull();
        await Assert.That(store.Closing).IsNotNull();
        await Assert.That(store.WindowClosed).IsNotNull();
        await Assert.That(store.WindowCreating).IsNotNull();
        await Assert.That(store.WindowCreated).IsNotNull();
        await Assert.That(store.WebMessageReceived).IsNotNull();
        await Assert.That(store.DebuggingEvent).IsNotNull();
        await Assert.That(store.WebMessagePostData).IsNotNull();
        await Assert.That(store.WebMessageGetData).IsNotNull();
        await Assert.That(store.FileDropped).IsNotNull();
        await Assert.That(store.CustomScheme).IsNotNull();
        await Assert.That(store.NavigationStarting).IsNotNull();
    }

    [Test]
    public async Task CopyTo_CopiesAllHandlers(CancellationToken ct = default) {
        // Arrange
        var source = new InfiniFrameEventsStore();
        var target = new InfiniFrameEventsStore();
        source.WindowFocusIn.Add(_ => {});
        source.WindowMinimized.Add(_ => {});
        source.WindowClosed.Add(_ => {});
        source.FileDropped.Add((_, _) => {});

        // Act
        source.CopyTo(target);

        // Assert
        int focusInCount = target.WindowFocusIn.Snapshot.Length;
        int minimizedCount = target.WindowMinimized.Snapshot.Length;
        int closedCount = target.WindowClosed.Snapshot.Length;
        int fileDroppedCount = target.FileDropped.Snapshot.Length;
        await Assert.That(focusInCount).IsEqualTo(1);
        await Assert.That(minimizedCount).IsEqualTo(1);
        await Assert.That(closedCount).IsEqualTo(1);
        await Assert.That(fileDroppedCount).IsEqualTo(1);
    }

    [Test]
    public async Task CopyTo_CopiesWebMessageHandlers(CancellationToken ct = default) {
        // Arrange
        var source = new InfiniFrameEventsStore();
        var target = new InfiniFrameEventsStore();
        source.WebMessagePostData.Add("msg1", handler: (_, _) => {});
        source.WebMessageGetData.Add("msg2", handler: (_, _) => "response");

        // Act
        source.CopyTo(target);

        // Assert
        bool hasPost = target.WebMessagePostData.ContainsKey("msg1");
        bool hasGet = target.WebMessageGetData.ContainsKey("msg2");
        await Assert.That(hasPost).IsTrue();
        await Assert.That(hasGet).IsTrue();
    }

    [Test]
    public async Task CopyTo_CopiesCustomSchemeHandlers(CancellationToken ct = default) {
        // Arrange
        var source = new InfiniFrameEventsStore();
        var target = new InfiniFrameEventsStore();
        source.CustomScheme.Add("app", handler: (_, _) => (null, null));

        // Act
        source.CopyTo(target);

        // Assert
        bool hasCustomScheme = target.CustomScheme.ContainsKey("app");
        await Assert.That(hasCustomScheme).IsTrue();
    }

    [Test]
    public async Task CopyTo_CopiesNavigationStartingHandlers(CancellationToken ct = default) {
        // Arrange
        var source = new InfiniFrameEventsStore();
        var target = new InfiniFrameEventsStore();
        source.NavigationStarting.Add((_, _) => NavigationStartingResult.Allow);

        // Act
        source.CopyTo(target);

        // Assert
        int count = target.NavigationStarting.Snapshot.Length;
        await Assert.That(count).IsEqualTo(1);
    }

    [Test]
    public async Task CopyTo_CopiesClosingHandlers(CancellationToken ct = default) {
        // Arrange
        var source = new InfiniFrameEventsStore();
        var target = new InfiniFrameEventsStore();
        source.Closing.Add((_, _) => default);

        // Act
        source.CopyTo(target);

        // Assert
        int count = target.Closing.Snapshot.Length;
        await Assert.That(count).IsEqualTo(1);
    }

    [Test]
    public async Task CopyTo_CopiesLocationChangedHandlers(CancellationToken ct = default) {
        // Arrange
        var source = new InfiniFrameEventsStore();
        var target = new InfiniFrameEventsStore();
        source.WindowLocationChanged.Add((_, _) => {});

        // Act
        source.CopyTo(target);

        // Assert
        int count = target.WindowLocationChanged.Snapshot.Length;
        await Assert.That(count).IsEqualTo(1);
    }

    [Test]
    public async Task CopyTo_CopiesSizeChangedHandlers(CancellationToken ct = default) {
        // Arrange
        var source = new InfiniFrameEventsStore();
        var target = new InfiniFrameEventsStore();
        source.WindowSizeChanged.Add((_, _) => {});

        // Act
        source.CopyTo(target);

        // Assert
        int count = target.WindowSizeChanged.Snapshot.Length;
        await Assert.That(count).IsEqualTo(1);
    }

    [Test]
    public async Task CopyTo_CopiesWindowCreatingHandlers(CancellationToken ct = default) {
        // Arrange
        var source = new InfiniFrameEventsStore();
        var target = new InfiniFrameEventsStore();
        source.WindowCreating.Add(_ => {});

        // Act
        source.CopyTo(target);

        // Assert
        int count = target.WindowCreating.Snapshot.Length;
        await Assert.That(count).IsEqualTo(1);
    }

    [Test]
    public async Task CopyTo_CopiesWindowCreatedHandlers(CancellationToken ct = default) {
        // Arrange
        var source = new InfiniFrameEventsStore();
        var target = new InfiniFrameEventsStore();
        source.WindowCreated.Add(_ => {});

        // Act
        source.CopyTo(target);

        // Assert
        int count = target.WindowCreated.Snapshot.Length;
        await Assert.That(count).IsEqualTo(1);
    }

    [Test]
    public async Task CopyTo_CopiesWindowClosingRequestedHandlers(CancellationToken ct = default) {
        // Arrange
        var source = new InfiniFrameEventsStore();
        var target = new InfiniFrameEventsStore();
        source.WindowClosingRequested.Add(_ => {});

        // Act
        source.CopyTo(target);

        // Assert
        int count = target.WindowClosingRequested.Snapshot.Length;
        await Assert.That(count).IsEqualTo(1);
    }
}
