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
    [SkipOnMacOs]
    [DefaultInfiniTestsTimeout(15_000)]
    public async Task RepeatedCreateCloseAcrossManagedThreads_DoesNotFail(CancellationToken ct) {
        // Arrange
        const int iterations = 6;

        // Act
        for (int i = 0; i < iterations; i++) {
            await Task.Run(() => CreateCloseAndWaitWindow(ct), ct);
        }

    }

    [Test]
    [SkipOnMacOs]
    [DefaultInfiniTestsTimeout(15_000)]
    public async Task ParallelCreateCloseAcrossManagedThreads_DoesNotFail(CancellationToken ct) {
        // Arrange
        Task[] operations = Enumerable.Range(0, 4)
            .Select(_ => Task.Run(() => CreateCloseAndWaitWindow(ct), ct))
            .ToArray();

        // Act
        await Task.WhenAll(operations);

    }

    private static void CreateCloseAndWaitWindow(CancellationToken ct) {
        ct.ThrowIfCancellationRequested();

        var builder = InfiniFrameWindowBuilder.Create();
        builder
            .SetIconFile("favicon.ico")
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
