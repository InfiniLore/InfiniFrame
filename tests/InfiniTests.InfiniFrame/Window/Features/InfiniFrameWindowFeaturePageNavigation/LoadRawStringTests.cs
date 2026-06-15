// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.InfiniFrameWindowFeaturePageNavigation;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class LoadRawStringTests {
    [Test]
    [NotInParallelInfiniTests]
    [SkipOnMacOs]
    [SkipOnLinux]
    public async Task AtWindowStage_DirectAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        const string html = "<html><body>load-raw-string-direct</body></html>";

        // Act
        window.Features.PageNavigation.LoadRawString(html);

        // Assert
        await Assert.That(window.Features.PageNavigation).IsNotNull();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnMacOs]
    [SkipOnLinux]
    public async Task AtWindowStage_ExtensionAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        const string html = "<html><body>load-raw-string-extension</body></html>";

        // Act
        IInfiniFrameWindow returnedWindow = window.LoadRawString(html);

        // Assert
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnMacOs]
    [DefaultInfiniTestsTimeout(DefaultInfiniTestsTimeoutAttribute.TimeoutValue + 5_000)]
    public async Task AtWindowStage_AfterClose_DoesNotThrowAndNoOps(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Close();
        await EnsureWindowClosed(window, ct);
        window.Features.PageNavigation.LoadRawString("<html><body>closed-direct</body></html>");
        window.LoadRawString("<html><body>closed-extension</body></html>");

        // Assert
        await Assert.That(window.IsClosedOrClosing()).IsTrue();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnMacOs]
    [DefaultInfiniTestsTimeout(DefaultInfiniTestsTimeoutAttribute.TimeoutValue + 5_000)]
    public async Task AtWindowStage_DuringClosingRequested_DoesNotThrow(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(
            builder: builder => builder.EventsStore.WindowClosingRequested.Add(window => {
                window.LoadRawString("<html><body>closing</body></html>");
            }),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Close();
        await EnsureWindowClosed(window, ct);

        // Assert
        await Assert.That(window.IsClosedOrClosing()).IsTrue();
    }

    private static async Task EnsureWindowClosed(IInfiniFrameWindow window, CancellationToken ct) {
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (!window.IsClosedOrClosing() && DateTime.UtcNow < timeoutAt && !ct.IsCancellationRequested) {
            await Task.Delay(50, ct);
        }
    }
}
