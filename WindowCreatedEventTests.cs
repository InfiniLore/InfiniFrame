// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.WindowEvents;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowCreatedEventTests {
    [Test]
    [Retry(5)]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task TestWindowCreatedEvent(CancellationToken ct = default) {
        // Arrange
        int createdEventCount = 0;
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => builder
                .RegisterWindowCreatedHandler(_ => {
                    // ReSharper disable once AccessToModifiedClosure
                    Interlocked.Increment(ref createdEventCount);
                })
            , ct
        );

        // Assert: event fires synchronously during Build(); no act step needed
        await PollUtility.WaitForChangeAsync(getValue: () => Volatile.Read(ref createdEventCount), 0, TimeSpan.FromSeconds(5), ct);
        await Assert.That(createdEventCount).IsEqualTo(1);
    }

    [Test]
    [Retry(3)]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task TestSendWebMessageFromWindowCreatedDoesNotCrash(CancellationToken ct = default) {
        // Arrange: register a WindowCreated handler that immediately calls SendWebMessage.
        // Before the fix this raised SystemAccessViolationException on Windows because
        // the WebView2 COM objects were not yet initialized at the time WindowCreated fires.
        bool windowCreatedCalled = false;
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => builder
                .RegisterWindowCreatedHandler(window => {
                    window.SendWebMessage("hello-from-window-created");
                    // ReSharper disable once AccessToModifiedClosure
                    Volatile.Write(ref windowCreatedCalled, true);
                })
            , ct
        );

        // Assert: if we reach this point without an exception the crash is fixed.
        // Also verify the handler actually ran (guards against the test being vacuously true).
        await Assert.That(Volatile.Read(ref windowCreatedCalled)).IsTrue();
    }
}
