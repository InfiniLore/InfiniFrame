// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Shared;
using Microsoft.Playwright;
using System.Text.Json;

namespace InfiniFrameTests.Playwright;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class SharedJavascriptTests : InfiniFramePlaywrightTestBase {
    private const string NewTitleFromHostMessage = "newly updated title";

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [NotInParallel(ParallelControl.Playwright)]
    public async Task InfiniWindowIsInitialized() {
        IPage page = await GetRootPageAsync();

        var initState = await page.EvaluateAsync<JsonElement>(
            // lang=javascript
            """
            () => ({
                hasHostBridge: window.infiniframe?.host !== undefined && window.infiniframe?.host !== null,
                hasInfiniFrameApi: window.infiniFrame !== undefined && window.infiniFrame !== null,
                hasHostMessaging: window.infiniFrame?.HostMessaging !== undefined && window.infiniFrame?.HostMessaging !== null,
                hasSendMessageToHost: typeof window.infiniFrame?.sendMessageToHost === 'function'
            })
            """
        );

        await Assert.That(initState.GetProperty("hasHostBridge").GetBoolean()).IsTrue();
        await Assert.That(initState.GetProperty("hasInfiniFrameApi").GetBoolean()).IsTrue();
        await Assert.That(initState.GetProperty("hasHostMessaging").GetBoolean()).IsTrue();
        await Assert.That(initState.GetProperty("hasSendMessageToHost").GetBoolean()).IsTrue();
    }

    [Test]
    [NotInParallel(ParallelControl.Playwright)]
    public async Task DynamicallyUpdateTitleFromJs() {
        IPage page = await GetRootPageAsync();
        string originalTitle = RuntimeContext.Window.Title;

        await page.EvaluateAsync(
            // lang=javascript
            $"() => window.infiniframe?.host?.postMessage({{ id: '__infiniframe:title:change', data: '{NewTitleFromHostMessage}', version: 1 }})"
        );
        string updatedTitle = await WaitForStateChangeAsync(
            originalTitle,
            stateProvider: () => RuntimeContext.Window.Title
        );

        await Assert.That(updatedTitle).IsEqualTo(NewTitleFromHostMessage);

        RuntimeContext.Window.SetTitle(RuntimeContext.DefaultDocumentTitle);
    }

    [Test]
    [NotInParallel(ParallelControl.Playwright)]
    public async Task WindowClose() {
        IPage page = await GetRootPageAsync();
        int initialCloseRequestCount = RuntimeContext.GetWindowCloseRequestCount();
        RuntimeContext.SuppressWindowCloseRequests(true);

        try {
            await page.EvaluateAsync(
                // lang=javascript
                "() => window.close()"
            );
            int closeRequestCount = await WaitForStateChangeAsync(
                initialCloseRequestCount,
                stateProvider: () => RuntimeContext.GetWindowCloseRequestCount()
            );

            await Assert.That(closeRequestCount).IsEqualTo(initialCloseRequestCount + 1);
        }
        finally {
            RuntimeContext.SuppressWindowCloseRequests(false);
        }
    }
}
