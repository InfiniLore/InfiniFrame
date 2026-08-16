// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;
using InfiniFrame;
using InfiniFrame.Debugging;
using InfiniFrame.DragDrop;
using InfiniFrame.Interop;

namespace InfiniTests.InfiniFrame.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameEventsStoreTests {

    [Test]
    public async Task Constructor_AllEventsAreNotNull(CancellationToken ct = default) {
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
    public async Task CopyTo_CopiesWindowClosedHandlers(CancellationToken ct = default) {
        // Arrange
        var source = new InfiniFrameEventsStore();
        var target = new InfiniFrameEventsStore();
        bool handlerCalled = false;
        source.WindowClosed.Add(_ => handlerCalled = true);

        // Act
        source.CopyTo(target);

        // Assert
        IInfiniFrameWindow window = MockFactory.CreateWindowMock().Object;
        target.WindowClosed.Invoke(window);
        await Assert.That(handlerCalled).IsTrue();
    }

    [Test]
    public async Task CopyTo_CopiesWindowClosingRequestedHandlers(CancellationToken ct = default) {
        // Arrange
        var source = new InfiniFrameEventsStore();
        var target = new InfiniFrameEventsStore();
        bool handlerCalled = false;
        source.WindowClosingRequested.Add(_ => handlerCalled = true);

        // Act
        source.CopyTo(target);

        // Assert
        IInfiniFrameWindow window = MockFactory.CreateWindowMock().Object;
        target.WindowClosingRequested.Invoke(window);
        await Assert.That(handlerCalled).IsTrue();
    }

    [Test]
    public async Task CopyTo_CopiesWindowFocusInHandlers(CancellationToken ct = default) {
        // Arrange
        var source = new InfiniFrameEventsStore();
        var target = new InfiniFrameEventsStore();
        bool handlerCalled = false;
        source.WindowFocusIn.Add(_ => handlerCalled = true);

        // Act
        source.CopyTo(target);

        // Assert
        IInfiniFrameWindow window = MockFactory.CreateWindowMock().Object;
        target.WindowFocusIn.Invoke(window);
        await Assert.That(handlerCalled).IsTrue();
    }

    [Test]
    public async Task CopyTo_CopiesWindowFocusOutHandlers(CancellationToken ct = default) {
        // Arrange
        var source = new InfiniFrameEventsStore();
        var target = new InfiniFrameEventsStore();
        bool handlerCalled = false;
        source.WindowFocusOut.Add(_ => handlerCalled = true);

        // Act
        source.CopyTo(target);

        // Assert
        IInfiniFrameWindow window = MockFactory.CreateWindowMock().Object;
        target.WindowFocusOut.Invoke(window);
        await Assert.That(handlerCalled).IsTrue();
    }

    [Test]
    public async Task CopyTo_CopiesWindowMaximizedHandlers(CancellationToken ct = default) {
        // Arrange
        var source = new InfiniFrameEventsStore();
        var target = new InfiniFrameEventsStore();
        bool handlerCalled = false;
        source.WindowMaximized.Add(_ => handlerCalled = true);

        // Act
        source.CopyTo(target);

        // Assert
        IInfiniFrameWindow window = MockFactory.CreateWindowMock().Object;
        target.WindowMaximized.Invoke(window);
        await Assert.That(handlerCalled).IsTrue();
    }

    [Test]
    public async Task CopyTo_CopiesWindowMinimizedHandlers(CancellationToken ct = default) {
        // Arrange
        var source = new InfiniFrameEventsStore();
        var target = new InfiniFrameEventsStore();
        bool handlerCalled = false;
        source.WindowMinimized.Add(_ => handlerCalled = true);

        // Act
        source.CopyTo(target);

        // Assert
        IInfiniFrameWindow window = MockFactory.CreateWindowMock().Object;
        target.WindowMinimized.Invoke(window);
        await Assert.That(handlerCalled).IsTrue();
    }

    [Test]
    public async Task CopyTo_CopiesWindowRestoredHandlers(CancellationToken ct = default) {
        // Arrange
        var source = new InfiniFrameEventsStore();
        var target = new InfiniFrameEventsStore();
        bool handlerCalled = false;
        source.WindowRestored.Add(_ => handlerCalled = true);

        // Act
        source.CopyTo(target);

        // Assert
        IInfiniFrameWindow window = MockFactory.CreateWindowMock().Object;
        target.WindowRestored.Invoke(window);
        await Assert.That(handlerCalled).IsTrue();
    }

    [Test]
    public async Task CopyTo_CopiesWindowCreatingHandlers(CancellationToken ct = default) {
        // Arrange
        var source = new InfiniFrameEventsStore();
        var target = new InfiniFrameEventsStore();
        bool handlerCalled = false;
        source.WindowCreating.Add(_ => handlerCalled = true);

        // Act
        source.CopyTo(target);

        // Assert
        IInfiniFrameWindow window = MockFactory.CreateWindowMock().Object;
        target.WindowCreating.Invoke(window);
        await Assert.That(handlerCalled).IsTrue();
    }

    [Test]
    public async Task CopyTo_CopiesWindowCreatedHandlers(CancellationToken ct = default) {
        // Arrange
        var source = new InfiniFrameEventsStore();
        var target = new InfiniFrameEventsStore();
        bool handlerCalled = false;
        source.WindowCreated.Add(_ => handlerCalled = true);

        // Act
        source.CopyTo(target);

        // Assert
        IInfiniFrameWindow window = MockFactory.CreateWindowMock().Object;
        target.WindowCreated.Invoke(window);
        await Assert.That(handlerCalled).IsTrue();
    }

    [Test]
    public async Task CopyTo_CopiesWebMessageReceivedHandlers(CancellationToken ct = default) {
        // Arrange
        var source = new InfiniFrameEventsStore();
        var target = new InfiniFrameEventsStore();
        bool handlerCalled = false;
        source.WebMessageReceived.Add((w, e) => handlerCalled = true);

        // Act
        source.CopyTo(target);

        // Assert
        IInfiniFrameWindow window = MockFactory.CreateWindowMock().Object;
        var evt = new InfiniFrameWebMessageReceivedEvent("msg", "origin");
        target.WebMessageReceived.Invoke(window, evt);
        await Assert.That(handlerCalled).IsTrue();
    }

    [Test]
    public async Task CopyTo_CopiesDebuggingEventHandlers(CancellationToken ct = default) {
        // Arrange
        var source = new InfiniFrameEventsStore();
        var target = new InfiniFrameEventsStore();
        bool handlerCalled = false;
        source.DebuggingEvent.Add((w, e) => handlerCalled = true);

        // Act
        source.CopyTo(target);

        // Assert
        IInfiniFrameWindow window = MockFactory.CreateWindowMock().Object;
        var evt = new InfiniFrameDebugEventArgs {
            Kind = InfiniFrameDebugEventKind.ScriptError,
            TimestampUtc = DateTime.UtcNow
        };
        target.DebuggingEvent.Invoke(window, evt);
        await Assert.That(handlerCalled).IsTrue();
    }

    [Test]
    public async Task CopyTo_CopiesWindowLocationChangedHandlers(CancellationToken ct = default) {
        // Arrange
        var source = new InfiniFrameEventsStore();
        var target = new InfiniFrameEventsStore();
        Point? received = null;
        source.WindowLocationChanged.Add((w, p) => received = p);

        // Act
        source.CopyTo(target);

        // Assert
        IInfiniFrameWindow window = MockFactory.CreateWindowMock().Object;
        target.WindowLocationChanged.Invoke(window, new Point(100, 200));
        await Assert.That(received).IsEqualTo(new Point(100, 200));
    }

    [Test]
    public async Task CopyTo_CopiesWindowSizeChangedHandlers(CancellationToken ct = default) {
        // Arrange
        var source = new InfiniFrameEventsStore();
        var target = new InfiniFrameEventsStore();
        Size? received = null;
        source.WindowSizeChanged.Add((w, s) => received = s);

        // Act
        source.CopyTo(target);

        // Assert
        IInfiniFrameWindow window = MockFactory.CreateWindowMock().Object;
        target.WindowSizeChanged.Invoke(window, new Size(800, 600));
        await Assert.That(received).IsEqualTo(new Size(800, 600));
    }

    [Test]
    public async Task CopyTo_CopiesClosingHandlers(CancellationToken ct = default) {
        // Arrange
        var source = new InfiniFrameEventsStore();
        var target = new InfiniFrameEventsStore();
        bool handlerCalled = false;
        source.Closing.Add((w, e) => { handlerCalled = true; return WindowClosingResult.Close; });

        // Act
        source.CopyTo(target);

        // Assert
        IInfiniFrameWindow window = MockFactory.CreateWindowMock().Object;
        target.Closing.Invoke(window, EventArgs.Empty);
        await Assert.That(handlerCalled).IsTrue();
    }

    [Test]
    public async Task CopyTo_CopiesNavigationStartingHandlers(CancellationToken ct = default) {
        // Arrange
        var source = new InfiniFrameEventsStore();
        var target = new InfiniFrameEventsStore();
        bool handlerCalled = false;
        source.NavigationStarting.Add((w, e) => { handlerCalled = true; return NavigationStartingResult.Allow; });

        // Act
        source.CopyTo(target);

        // Assert
        IInfiniFrameWindow window = MockFactory.CreateWindowMock().Object;
        var args = new NavigationStartingEventArgs("https://example.com", false, false, true);
        target.NavigationStarting.Invoke(window, args);
        await Assert.That(handlerCalled).IsTrue();
    }

    [Test]
    public async Task CopyTo_CopiesWebMessagePostDataHandlers(CancellationToken ct = default) {
        // Arrange
        var source = new InfiniFrameEventsStore();
        var target = new InfiniFrameEventsStore();
        string? receivedValue = null;
        source.WebMessagePostData.Add("test-key", (w, v) => receivedValue = v);

        // Act
        source.CopyTo(target);

        // Assert
        IInfiniFrameWindow window = MockFactory.CreateWindowMock().Object;
        target.WebMessagePostData.TryInvoke("test-key", window, "test-value");
        await Assert.That(receivedValue).IsEqualTo("test-value");
    }

    [Test]
    public async Task CopyTo_EmptySource_DoesNotThrow(CancellationToken ct = default) {
        // Arrange
        var source = new InfiniFrameEventsStore();
        var target = new InfiniFrameEventsStore();

        // Act & Assert
        await Assert.That(() => source.CopyTo(target)).ThrowsNothing();
    }
}
