// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Shared;
using Microsoft.Playwright;
using System.Text.Json;

namespace InfiniFrameTests.Playwright.Tests;
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
        var initState = await EvaluateWhenPageReadyAsync<JsonElement>(
            page,
            // lang=javascript
            """
            () => ({
                hasNativeHostBridge: window.__infiniframe?.host !== undefined && window.__infiniframe?.host !== null,
                hasInfiniFrameApi: window.infiniframe !== undefined && window.infiniframe !== null,
                hasMessaging: window.infiniframe?.messaging !== undefined && window.infiniframe?.messaging !== null,
                hasWindow: window.infiniframe?.window !== undefined && window.infiniframe?.window !== null,
                hasUtils: window.infiniframe?.utils !== undefined && window.infiniframe?.utils !== null,
            })
            """
        );

        await Assert.That(initState.GetProperty("hasNativeHostBridge").GetBoolean()).IsTrue();
        await Assert.That(initState.GetProperty("hasInfiniFrameApi").GetBoolean()).IsTrue();
        await Assert.That(initState.GetProperty("hasMessaging").GetBoolean()).IsTrue();
        await Assert.That(initState.GetProperty("hasWindow").GetBoolean()).IsTrue();
        await Assert.That(initState.GetProperty("hasUtils").GetBoolean()).IsTrue();
    }

    [Test]
    [NotInParallel(ParallelControl.Playwright)]
    public async Task DynamicallyUpdateTitleFromJs() {
        IPage page = await GetRootPageAsync();
        string? originalTitle = RuntimeContext.Window.Title;

        await EvaluateWhenPageReadyAsync(
            page,
            // lang=javascript
            $"() => window.__infiniframe?.host?.postData({{ id: '__infiniframe:title:change', command: 'Post', data: '{NewTitleFromHostMessage}', version: 2 }})"
        );
        string? updatedTitle = await WaitForStateChangeAsync(
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
            await EvaluateWhenPageReadyAsync(
                page,
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
