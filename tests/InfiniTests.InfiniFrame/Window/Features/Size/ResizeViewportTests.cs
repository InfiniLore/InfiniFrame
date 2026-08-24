// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.RegularExpressions;
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Size;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class ResizeViewportTests {
    private static readonly Regex ViewportPattern = new(@"^(\d+)x(\d+)$", RegexOptions.Compiled);

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    [SkipOnWindowsArm("Test is flaky on ARM")]
    [DefaultInfiniTestsTimeout(35_000)]
    public async Task NativeResize_ShouldUpdateBrowserViewport(CancellationToken ct = default) {
        // Arrange
        var firstViewport = new TaskCompletionSource<(int Width, int Height)>(TaskCreationOptions.RunContinuationsAsynchronously);
        var resizedViewport = new TaskCompletionSource<(int Width, int Height)>(TaskCreationOptions.RunContinuationsAsynchronously);

        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder
                .SetSize(520, 360)
                .SetStartPageContent("""
                    <!DOCTYPE html>
                    <html>
                    <body>
                    <script>
                    (() => {
                      function postViewport() {
                        const value = window.innerWidth + 'x' + window.innerHeight;
                        const envelope = JSON.stringify({
                          id: 'vp',
                          command: 'Post',
                          data: value,
                          version: 2
                        });
                        if (window.chrome && window.chrome.webview && window.chrome.webview.postMessage) {
                          window.chrome.webview.postMessage(envelope);
                          return true;
                        }
                        if (window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.infiniFrameInterop && window.webkit.messageHandlers.infiniFrameInterop.postMessage) {
                          window.webkit.messageHandlers.infiniFrameInterop.postMessage(envelope);
                          return true;
                        }
                        if (window.external && typeof window.external.sendMessage === 'function') {
                          window.external.sendMessage(envelope);
                          return true;
                        }
                        return false;
                      }

                      function postInitialViewportWithRetry(remainingAttempts) {
                        if (postViewport()) return;
                        if (remainingAttempts <= 0) return;
                        setTimeout(() => postInitialViewportWithRetry(remainingAttempts - 1), 100);
                      }

                      postInitialViewportWithRetry(120);
                      window.addEventListener('load', () => postInitialViewportWithRetry(120));
                      window.addEventListener('resize', () => { postViewport(); });
                    })();
                    </script>
                    </body>
                    </html>
                    """)
                .RegisterWebMessagePostHandler("vp", handler: (_, payload) => {
                    if (!TryParseViewport(payload, out (int Width, int Height) viewport)) return;

                    if (!firstViewport.Task.IsCompleted) {
                        firstViewport.TrySetResult(viewport);
                        return;
                    }

                    (int Width, int Height) initial = firstViewport.Task.Result;
                    if (viewport.Width != initial.Width || viewport.Height != initial.Height) {
                        resizedViewport.TrySetResult(viewport);
                    }
                });
        }, ct);

        IInfiniFrameWindow window = windowUtility.Window;

        (int Width, int Height) initialViewport = await firstViewport.Task.WaitAsync(TimeSpan.FromSeconds(20), ct);
        int originalWidth = window.Features.Size.Width;
        int originalHeight = window.Features.Size.Height;
        int targetWidth = originalWidth + 180;
        int targetHeight = originalHeight + 120;

        // Act
        window.SetSize(targetWidth, targetHeight);
        _ = await PollUtility.WaitForChangeAsync(getValue: () => window.Features.Size.Width, originalWidth, TimeSpan.FromSeconds(5), ct);
        _ = await PollUtility.WaitForChangeAsync(getValue: () => window.Features.Size.Height, originalHeight, TimeSpan.FromSeconds(5), ct);

        (int Width, int Height) newViewport = await resizedViewport.Task.WaitAsync(TimeSpan.FromSeconds(15), ct);

        // Assert
        await Assert.That(newViewport.Width).IsGreaterThan(initialViewport.Width);
        await Assert.That(newViewport.Height).IsGreaterThan(initialViewport.Height);
    }

    private static bool TryParseViewport(string? message, out (int Width, int Height) viewport) {
        if (string.IsNullOrWhiteSpace(message)) {
            viewport = default;
            return false;
        }

        Match match = ViewportPattern.Match(message);
        if (!match.Success) {
            viewport = default;
            return false;
        }

        viewport = (
            Width: int.Parse(match.Groups[1].Value),
            Height: int.Parse(match.Groups[2].Value)
        );
        return true;
    }
}
