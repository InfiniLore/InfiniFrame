// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniTests.InfiniFrame.Application;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameApplicationTests {
    [Test]
    public async Task TrackWindow_AddsWindowToCollection(CancellationToken ct = default) {
        // Arrange
        InfiniFrameApplication app = CreateApplication();
        Mock<IInfiniFrameWindow> window = CreateWindowMock();

        // Act
        app.TrackWindow(window.Object);

        // Assert
        await Assert.That(app.WindowCount).IsEqualTo(1);
    }

    [Test]
    public async Task TrackWindow_FiresWindowCreatedEvent(CancellationToken ct = default) {
        // Arrange
        InfiniFrameApplication app = CreateApplication();
        Mock<IInfiniFrameWindow> window = CreateWindowMock();
        IInfiniFrameWindow? received = null;
        app.WindowCreated += w => received = w;

        // Act
        app.TrackWindow(window.Object);

        // Assert
        await Assert.That(received).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task TrackWindow_MultipleWindows_IncrementsCount(CancellationToken ct = default) {
        // Arrange
        InfiniFrameApplication app = CreateApplication();
        Mock<IInfiniFrameWindow> w1 = CreateWindowMock();
        Mock<IInfiniFrameWindow> w2 = CreateWindowMock();
        Mock<IInfiniFrameWindow> w3 = CreateWindowMock();

        // Act
        app.TrackWindow(w1.Object);
        app.TrackWindow(w2.Object);
        app.TrackWindow(w3.Object);

        // Assert
        await Assert.That(app.WindowCount).IsEqualTo(3);
    }

    [Test]
    public async Task TrackWindow_DuplicateWindow_NotTrackedTwice(CancellationToken ct = default) {
        // Arrange
        InfiniFrameApplication app = CreateApplication();
        Mock<IInfiniFrameWindow> window = CreateWindowMock();

        // Act
        app.TrackWindow(window.Object);
        app.TrackWindow(window.Object);

        // Assert
        await Assert.That(app.WindowCount).IsEqualTo(1);
    }

    [Test]
    public async Task UntrackWindow_RemovesWindowFromCollection(CancellationToken ct = default) {
        // Arrange
        InfiniFrameApplication app = CreateApplication();
        Mock<IInfiniFrameWindow> window = CreateWindowMock();
        app.TrackWindow(window.Object);

        // Act
        app.UntrackWindow(window.Object);

        // Assert
        await Assert.That(app.WindowCount).IsEqualTo(0);
    }

    [Test]
    public async Task UntrackWindow_FiresWindowDestroyedEvent(CancellationToken ct = default) {
        // Arrange
        InfiniFrameApplication app = CreateApplication();
        Mock<IInfiniFrameWindow> window = CreateWindowMock();
        IInfiniFrameWindow? received = null;
        app.WindowDestroyed += w => received = w;
        app.TrackWindow(window.Object);

        // Act
        app.UntrackWindow(window.Object);

        // Assert
        await Assert.That(received).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task UntrackWindow_NotTracked_DoesNotFireEvent(CancellationToken ct = default) {
        // Arrange
        InfiniFrameApplication app = CreateApplication();
        Mock<IInfiniFrameWindow> window = CreateWindowMock();
        bool eventFired = false;
        app.WindowDestroyed += _ => eventFired = true;

        // Act
        app.UntrackWindow(window.Object);

        // Assert
        await Assert.That(eventFired).IsFalse();
    }

    [Test]
    public async Task UntrackWindow_MultipleWindows_DecrementsCount(CancellationToken ct = default) {
        // Arrange
        InfiniFrameApplication app = CreateApplication();
        Mock<IInfiniFrameWindow> w1 = CreateWindowMock();
        Mock<IInfiniFrameWindow> w2 = CreateWindowMock();
        app.TrackWindow(w1.Object);
        app.TrackWindow(w2.Object);

        // Act
        app.UntrackWindow(w1.Object);

        // Assert
        await Assert.That(app.WindowCount).IsEqualTo(1);
    }

    [Test]
    public async Task CloseAll_CallsCloseOnAllWindows(CancellationToken ct = default) {
        // Arrange
        InfiniFrameApplication app = CreateApplication();
        (Mock<IInfiniFrameWindow> mock, Mock<ILifecycleInfiniFrameWindowFeature> lifecycle) w1 = CreateWindowWithLifecycleMock();
        (Mock<IInfiniFrameWindow> mock, Mock<ILifecycleInfiniFrameWindowFeature> lifecycle) w2 = CreateWindowWithLifecycleMock();
        (Mock<IInfiniFrameWindow> mock, Mock<ILifecycleInfiniFrameWindowFeature> lifecycle) w3 = CreateWindowWithLifecycleMock();
        app.TrackWindow(w1.mock.Object);
        app.TrackWindow(w2.mock.Object);
        app.TrackWindow(w3.mock.Object);

        // Act
        app.CloseAll();

        // Assert
        w1.lifecycle.Close().WasCalled(Times.Once);
        w2.lifecycle.Close().WasCalled(Times.Once);
        w3.lifecycle.Close().WasCalled(Times.Once);
    }

    [Test]
    public async Task CloseAll_EmptyCollection_DoesNotThrow(CancellationToken ct = default) {
        // Arrange
        InfiniFrameApplication app = CreateApplication();

        // Act & Assert — no exception means pass
        app.CloseAll();
        await Assert.That(app.WindowCount).IsEqualTo(0);
    }

    [Test]
    public async Task CloseAll_OnlyTrackedWindows_AreClosed(CancellationToken ct = default) {
        // Arrange
        InfiniFrameApplication app = CreateApplication();
        (Mock<IInfiniFrameWindow> mock, Mock<ILifecycleInfiniFrameWindowFeature> lifecycle) tracked = CreateWindowWithLifecycleMock();
        (Mock<IInfiniFrameWindow> mock, Mock<ILifecycleInfiniFrameWindowFeature> lifecycle) notTracked = CreateWindowWithLifecycleMock();
        app.TrackWindow(tracked.mock.Object);

        // Act
        app.CloseAll();

        // Assert
        tracked.lifecycle.Close().WasCalled(Times.Once);
        notTracked.lifecycle.Close().WasNeverCalled();
    }

    [Test]
    public async Task CloseAll_WindowThrowsException_ContinuesClosingOthers(CancellationToken ct = default) {
        // Arrange
        InfiniFrameApplication app = CreateApplication();
        (Mock<IInfiniFrameWindow> mock, Mock<ILifecycleInfiniFrameWindowFeature> lifecycle) w1 = CreateWindowWithLifecycleMock();
        (Mock<IInfiniFrameWindow> mock, Mock<ILifecycleInfiniFrameWindowFeature> lifecycle) w2 = CreateWindowWithLifecycleMock();
        w1.lifecycle.Close().Callback(() => throw new InvalidOperationException("test"));
        app.TrackWindow(w1.mock.Object);
        app.TrackWindow(w2.mock.Object);

        // Act
        app.CloseAll();

        // Assert
        w2.lifecycle.Close().WasCalled(Times.Once);
    }

    [Test]
    public async Task WindowCreated_FiresForEachTrackedWindow(CancellationToken ct = default) {
        // Arrange
        InfiniFrameApplication app = CreateApplication();
        var created = new List<IInfiniFrameWindow>();
        app.WindowCreated += w => created.Add(w);
        Mock<IInfiniFrameWindow> w1 = CreateWindowMock();
        Mock<IInfiniFrameWindow> w2 = CreateWindowMock();

        // Act
        app.TrackWindow(w1.Object);
        app.TrackWindow(w2.Object);

        // Assert
        await Assert.That(created.Count).IsEqualTo(2);
        await Assert.That(created[0]).IsSameReferenceAs(w1.Object);
        await Assert.That(created[1]).IsSameReferenceAs(w2.Object);
    }

    [Test]
    public async Task WindowDestroyed_FiresForEachUntrackedWindow(CancellationToken ct = default) {
        // Arrange
        InfiniFrameApplication app = CreateApplication();
        var destroyed = new List<IInfiniFrameWindow>();
        app.WindowDestroyed += w => destroyed.Add(w);
        Mock<IInfiniFrameWindow> w1 = CreateWindowMock();
        Mock<IInfiniFrameWindow> w2 = CreateWindowMock();
        app.TrackWindow(w1.Object);
        app.TrackWindow(w2.Object);

        // Act
        app.UntrackWindow(w1.Object);
        app.UntrackWindow(w2.Object);

        // Assert
        await Assert.That(destroyed.Count).IsEqualTo(2);
        await Assert.That(destroyed[0]).IsSameReferenceAs(w1.Object);
        await Assert.That(destroyed[1]).IsSameReferenceAs(w2.Object);
    }

    [Test]
    public async Task WindowCount_InitiallyZero(CancellationToken ct = default) {
        // Arrange & Act
        InfiniFrameApplication app = CreateApplication();

        // Assert
        await Assert.That(app.WindowCount).IsEqualTo(0);
    }

    [Test]
    public async Task TrackUntrack_WindowCountReturnsToZero(CancellationToken ct = default) {
        // Arrange
        InfiniFrameApplication app = CreateApplication();
        Mock<IInfiniFrameWindow> window = CreateWindowMock();

        // Act
        app.TrackWindow(window.Object);
        app.UntrackWindow(window.Object);

        // Assert
        await Assert.That(app.WindowCount).IsEqualTo(0);
    }

    // ── Multi-window scenario tests ──────────────────────────────────────────

    [Test]
    public async Task MultiWindow_CreateThreeWindows_AllTracked(CancellationToken ct = default) {
        // Arrange
        InfiniFrameApplication app = CreateApplication();
        Mock<IInfiniFrameWindow> w1 = CreateWindowMock();
        Mock<IInfiniFrameWindow> w2 = CreateWindowMock();
        Mock<IInfiniFrameWindow> w3 = CreateWindowMock();

        // Act
        app.TrackWindow(w1.Object);
        app.TrackWindow(w2.Object);
        app.TrackWindow(w3.Object);

        // Assert
        await Assert.That(app.WindowCount).IsEqualTo(3);
    }

    [Test]
    public async Task MultiWindow_CloseOne_OthersRemain(CancellationToken ct = default) {
        // Arrange
        InfiniFrameApplication app = CreateApplication();
        Mock<IInfiniFrameWindow> w1 = CreateWindowMock();
        Mock<IInfiniFrameWindow> w2 = CreateWindowMock();
        Mock<IInfiniFrameWindow> w3 = CreateWindowMock();
        app.TrackWindow(w1.Object);
        app.TrackWindow(w2.Object);
        app.TrackWindow(w3.Object);

        // Act
        app.UntrackWindow(w1.Object);

        // Assert
        await Assert.That(app.WindowCount).IsEqualTo(2);
    }

    [Test]
    public async Task MultiWindow_CloseAll_AllClosed(CancellationToken ct = default) {
        // Arrange
        InfiniFrameApplication app = CreateApplication();
        (Mock<IInfiniFrameWindow> mock, Mock<ILifecycleInfiniFrameWindowFeature> lifecycle) w1 = CreateWindowWithLifecycleMock();
        (Mock<IInfiniFrameWindow> mock, Mock<ILifecycleInfiniFrameWindowFeature> lifecycle) w2 = CreateWindowWithLifecycleMock();
        (Mock<IInfiniFrameWindow> mock, Mock<ILifecycleInfiniFrameWindowFeature> lifecycle) w3 = CreateWindowWithLifecycleMock();
        app.TrackWindow(w1.mock.Object);
        app.TrackWindow(w2.mock.Object);
        app.TrackWindow(w3.mock.Object);

        // Act
        app.CloseAll();

        // Assert
        w1.lifecycle.Close().WasCalled(Times.Once);
        w2.lifecycle.Close().WasCalled(Times.Once);
        w3.lifecycle.Close().WasCalled(Times.Once);
    }

    [Test]
    public async Task MultiWindow_InterleavedCreateDestroy_CountAlwaysCorrect(CancellationToken ct = default) {
        // Arrange
        InfiniFrameApplication app = CreateApplication();
        Mock<IInfiniFrameWindow> w1 = CreateWindowMock();
        Mock<IInfiniFrameWindow> w2 = CreateWindowMock();
        Mock<IInfiniFrameWindow> w3 = CreateWindowMock();

        // Act & Assert — interleave operations
        app.TrackWindow(w1.Object);
        await Assert.That(app.WindowCount).IsEqualTo(1);

        app.TrackWindow(w2.Object);
        await Assert.That(app.WindowCount).IsEqualTo(2);

        app.UntrackWindow(w1.Object);
        await Assert.That(app.WindowCount).IsEqualTo(1);

        app.TrackWindow(w3.Object);
        await Assert.That(app.WindowCount).IsEqualTo(2);

        app.UntrackWindow(w2.Object);
        await Assert.That(app.WindowCount).IsEqualTo(1);

        app.UntrackWindow(w3.Object);
        await Assert.That(app.WindowCount).IsEqualTo(0);
    }

    [Test]
    public async Task MultiWindow_EventsFireInCorrectOrder(CancellationToken ct = default) {
        // Arrange
        InfiniFrameApplication app = CreateApplication();
        var events = new List<string>();
        app.WindowCreated += w => events.Add($"created:{w.Id}");
        app.WindowDestroyed += w => events.Add($"destroyed:{w.Id}");

        Mock<IInfiniFrameWindow> w1 = CreateWindowMock();
        Mock<IInfiniFrameWindow> w2 = CreateWindowMock();

        // Act
        app.TrackWindow(w1.Object);
        app.TrackWindow(w2.Object);
        app.UntrackWindow(w1.Object);
        app.UntrackWindow(w2.Object);

        // Assert
        await Assert.That(events.Count).IsEqualTo(4);
        await Assert.That(events[0]).IsEqualTo($"created:{w1.Object.Id}");
        await Assert.That(events[1]).IsEqualTo($"created:{w2.Object.Id}");
        await Assert.That(events[2]).IsEqualTo($"destroyed:{w1.Object.Id}");
        await Assert.That(events[3]).IsEqualTo($"destroyed:{w2.Object.Id}");
    }

    [Test]
    public async Task MultiWindow_ConcurrentTrackUntrack_ThreadSafe(CancellationToken ct = default) {
        // Arrange
        InfiniFrameApplication app = CreateApplication();
        Mock<IInfiniFrameWindow>[] windows = Enumerable.Range(0, 10)
            .Select(_ => CreateWindowMock())
            .ToArray();

        // Act — track all, then untrack all from parallel threads
        Parallel.ForEach(windows, w => app.TrackWindow(w.Object));
        await Assert.That(app.WindowCount).IsEqualTo(10);

        Parallel.ForEach(windows, w => app.UntrackWindow(w.Object));
        await Assert.That(app.WindowCount).IsEqualTo(0);
    }

    [Test]
    public async Task IsShutdownRequested_InitiallyFalse(CancellationToken ct = default) {
        // Arrange & Act
        InfiniFrameApplication app = CreateApplication();

        // Assert
        await Assert.That(app.IsShutdownRequested).IsFalse();
    }

    [Test]
    public async Task Id_IsUniquePerInstance(CancellationToken ct = default) {
        // Arrange & Act
        InfiniFrameApplication app1 = CreateApplication();
        InfiniFrameApplication app2 = CreateApplication();

        // Assert
        await Assert.That(app1.Id).IsNotEqualTo(app2.Id);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static InfiniFrameApplication CreateApplication() {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInfiniFrame();
        ServiceProvider provider = services.BuildServiceProvider();
        // Don't call Initialize — tests exercise C# tracking/events/CloseAll, not native handles.
        return (InfiniFrameApplication)provider.GetRequiredService<IInfiniFrameApplication>();
    }

    private static Mock<IInfiniFrameWindow> CreateWindowMock() {
        Mock<IInfiniFrameWindow> mock = MockFactory.CreateWindowMock();
        mock.Id.Returns(Guid.NewGuid());
        mock.LifecycleState.Returns(InfiniFrameWindowLifecycleState.Ready);
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        mock.Features.Returns(features.Object);
        return mock;
    }

    private static (Mock<IInfiniFrameWindow> mock, Mock<ILifecycleInfiniFrameWindowFeature> lifecycle) CreateWindowWithLifecycleMock() {
        Mock<IInfiniFrameWindow> mock = CreateWindowMock();
        Mock<ILifecycleInfiniFrameWindowFeature> lifecycle = MockFactory.CreateLifecycleMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        features.Lifecycle.Returns(lifecycle.Object);
        mock.Features.Returns(features.Object);
        return (mock, lifecycle);
    }
}
