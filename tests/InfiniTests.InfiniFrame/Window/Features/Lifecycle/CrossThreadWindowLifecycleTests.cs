// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Lifecycle;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CrossThreadWindowLifecycleTests {
    private const string StartString = """
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="UTF-8">
        </head>
        <body>
        </body>
        </html>
        """;

    [Test]
    [SkipOnWindows]
    [SkipOnMacOs("WKWebView can crash in WebKit when windows are repeatedly created and destroyed from managed worker threads")]
    [DefaultInfiniTestsTimeout(30_000)]
    public async Task RepeatedCreateCloseAcrossManagedThreads_DoesNotFail(CancellationToken ct) {
        // Arrange
        const int iterations = 6;

        // Act
        for (int i = 0; i < iterations; i++) {
            await Task.Run(action: () => CreateCloseAndWaitWindow(ct), ct);
        }

    }

    [Test]
    [SkipOnWindows]
    [SkipOnMacOs("WKWebView can crash in WebKit when several windows are created and destroyed concurrently from managed worker threads")]
    [DefaultInfiniTestsTimeout(30_000)]
    public async Task ParallelCreateCloseAcrossManagedThreads_DoesNotFail(CancellationToken ct) {
        // Arrange
        Task[] operations = [
            .. Enumerable.Range(0, 4)
                .Select(_ => Task.Run(action: () => CreateCloseAndWaitWindow(ct), ct))
        ];

        // Act
        await Task.WhenAll(operations);

    }

    private static void CreateCloseAndWaitWindow(CancellationToken ct) {
        ct.ThrowIfCancellationRequested();

        var builder = InfiniFrameWindowBuilder.Create();
        builder
            .SetIconFile("wwwroot/favicon.ico")
            .SetStartPageContent(StartString);

        IInfiniFrameWindow window = builder.Build();
        try {
            window.Close();
            window.WaitForClose();

            if (!window.IsClosedOrClosing()) {
                throw new InvalidOperationException("Window should be closed after Close + WaitForClose.");
            }
        }
        finally {
            if (window is IDisposable disposableWindow) {
                disposableWindow.Dispose();
            }
        }
    }
}