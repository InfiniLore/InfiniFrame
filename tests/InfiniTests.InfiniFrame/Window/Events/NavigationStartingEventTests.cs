// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class NavigationStartingEventTests {
    [Test]
    [NotInParallelInfiniTests]
    public async Task EventStore_Initialization_HasEmptyNavigationStartingHandlers(CancellationToken ct = default) {
        // Arrange & Act
        using var windowUtility = InfiniFrameTestWindow.Create(ct);

        // Assert
        await Assert.That(windowUtility.Window.Events.EventsStore.NavigationStarting.Snapshot.IsDefaultOrEmpty).IsTrue();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task RegisterNavigationStartingHandler_Chaining_ReturnsSameInstance(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        IInfiniFrameWindow result = window.RegisterNavigationStartingHandler((_, _) => NavigationStartingResult.Allow);

        // Assert
        await Assert.That(result).IsSameReferenceAs(window);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task RegisterNavigationStartingHandler_AddsHandlerToEvent(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        // ReSharper disable once ConvertToLocalFunction
        Func<IInfiniFrameWindow, NavigationStartingEventArgs, NavigationStartingResult> handler =
            (_, _) => NavigationStartingResult.Allow;

        // Act
        window.RegisterNavigationStartingHandler(handler);

        // Assert
        await Assert.That(window.Events.EventsStore.NavigationStarting.Snapshot.Length).IsEqualTo(1);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task MultipleHandlers_AllRunInRegistrationOrder(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        List<int> executionOrder = [];

        window.RegisterNavigationStartingHandler((_, _) => {
            lock (executionOrder) executionOrder.Add(1);
            return NavigationStartingResult.Allow;
        });
        window.RegisterNavigationStartingHandler((_, _) => {
            lock (executionOrder) executionOrder.Add(2);
            return NavigationStartingResult.Allow;
        });
        window.RegisterNavigationStartingHandler((_, _) => {
            lock (executionOrder) executionOrder.Add(3);
            return NavigationStartingResult.Allow;
        });

        // Act - simulate the callback
        NavigationStartingResult[] results = window.Events.EventsStore.NavigationStarting.Invoke(
            window,
            new NavigationStartingEventArgs("https://example.com", true, false, true)
        );

        // Assert
        await Assert.That(results.Length).IsEqualTo(3);
        await Assert.That(executionOrder).IsEquivalentTo([1, 2, 3]);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task OnNavigationStarting_WithNoHandlers_ReturnsZero(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        byte result = window.Events.OnNavigationStarting("https://example.com", 1, 0, 1);

        // Assert
        await Assert.That(result).IsEqualTo((byte)0);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task OnNavigationStarting_HandlerReturnsAllow_ReturnsZero(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        window.RegisterNavigationStartingHandler((_, _) => NavigationStartingResult.Allow);

        // Act
        byte result = window.Events.OnNavigationStarting("https://example.com", 1, 0, 1);

        // Assert
        await Assert.That(result).IsEqualTo((byte)0);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task OnNavigationStarting_HandlerReturnsCancel_ReturnsOne(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        window.RegisterNavigationStartingHandler((_, _) => NavigationStartingResult.Cancel);

        // Act
        byte result = window.Events.OnNavigationStarting("https://example.com", 1, 0, 1);

        // Assert
        await Assert.That(result).IsEqualTo((byte)1);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task OnNavigationStarting_FirstHandlerCancels_SecondHandlerStillRuns(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        bool secondHandlerRan = false;

        window.RegisterNavigationStartingHandler((_, _) => NavigationStartingResult.Cancel);
        window.RegisterNavigationStartingHandler((_, _) => {
            secondHandlerRan = true;
            return NavigationStartingResult.Allow;
        });

        // Act
        byte result = window.Events.OnNavigationStarting("https://example.com", 1, 0, 1);

        // Assert
        await Assert.That(result).IsEqualTo((byte)1);
        await Assert.That(secondHandlerRan).IsTrue();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task OnNavigationStarting_ReceivesCorrectArguments(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        string? capturedUrl = null;
        bool? capturedUserInitiated = null;
        bool? capturedRedirect = null;
        bool? capturedMainFrame = null;

        window.RegisterNavigationStartingHandler((_, args) => {
            capturedUrl = args.Url;
            capturedUserInitiated = args.IsUserInitiated;
            capturedRedirect = args.IsRedirect;
            capturedMainFrame = args.IsMainFrame;
            return NavigationStartingResult.Allow;
        });

        // Act
        window.Events.OnNavigationStarting("https://example.com/path", 1, 1, 0);

        // Assert
        await Assert.That(capturedUrl).IsEqualTo("https://example.com/path");
        await Assert.That(capturedUserInitiated).IsTrue();
        await Assert.That(capturedRedirect).IsTrue();
        await Assert.That(capturedMainFrame).IsFalse();
    }
}
