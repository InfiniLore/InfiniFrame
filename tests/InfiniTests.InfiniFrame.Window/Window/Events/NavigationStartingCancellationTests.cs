// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class NavigationStartingCancellationTests {
    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_AllowHandler_HandlerIsCalled(CancellationToken ct = default) {
        // Arrange
        int handlerCallCount = 0;
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        window.RegisterNavigationStartingHandler((_, _) => {
            Interlocked.Increment(ref handlerCallCount);
            return NavigationStartingResult.Allow;
        });

        // Act - simulate native callback
        byte result = window.Events.OnNavigationStarting("https://example.com", 1, 0, 1);

        // Assert
        await Assert.That(handlerCallCount).IsEqualTo(1);
        await Assert.That(result).IsEqualTo((byte)0);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_CancelHandler_HandlerIsCalled(CancellationToken ct = default) {
        // Arrange
        int handlerCallCount = 0;
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        window.RegisterNavigationStartingHandler((_, _) => {
            Interlocked.Increment(ref handlerCallCount);
            return NavigationStartingResult.Cancel;
        });

        // Act - simulate native callback
        byte result = window.Events.OnNavigationStarting("https://example.com", 1, 0, 1);

        // Assert
        await Assert.That(handlerCallCount).IsEqualTo(1);
        await Assert.That(result).IsEqualTo((byte)1);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_HandlerReceivesCorrectUrl(CancellationToken ct = default) {
        // Arrange
        string? capturedUrl = null;
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        window.RegisterNavigationStartingHandler((_, args) => {
            capturedUrl = args.Url;
            return NavigationStartingResult.Allow;
        });

        // Act
        window.Events.OnNavigationStarting("https://example.com/path?q=1", 0, 0, 1);

        // Assert
        await Assert.That(capturedUrl).IsEqualTo("https://example.com/path?q=1");
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_HandlerReceivesUserInitiatedFlag(CancellationToken ct = default) {
        // Arrange
        bool? capturedFlag = null;
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        window.RegisterNavigationStartingHandler((_, args) => {
            capturedFlag = args.IsUserInitiated;
            return NavigationStartingResult.Allow;
        });

        // Act
        window.Events.OnNavigationStarting("https://example.com", 1, 0, 1);

        // Assert
        await Assert.That(capturedFlag).IsTrue();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_HandlerReceivesMainFrameFlag(CancellationToken ct = default) {
        // Arrange
        bool? capturedFlag = null;
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        window.RegisterNavigationStartingHandler((_, args) => {
            capturedFlag = args.IsMainFrame;
            return NavigationStartingResult.Allow;
        });

        // Act
        window.Events.OnNavigationStarting("https://example.com", 0, 0, 0);

        // Assert
        await Assert.That(capturedFlag).IsFalse();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_DirectAssignment_CancelHandler_ReturnsOne(CancellationToken ct = default) {
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
    public async Task AtWindowStage_AllowThenCancel_FinalResultIsOne(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        window.RegisterNavigationStartingHandler((_, _) => NavigationStartingResult.Allow);
        window.RegisterNavigationStartingHandler((_, _) => NavigationStartingResult.Cancel);

        // Act
        byte result = window.Events.OnNavigationStarting("https://example.com", 1, 0, 1);

        // Assert
        await Assert.That(result).IsEqualTo((byte)1);
    }
}
