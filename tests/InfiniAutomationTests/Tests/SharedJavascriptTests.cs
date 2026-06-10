// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniTests;
using Microsoft.Playwright;
using System.Text.Json;

namespace InfiniAutomationTests.Tests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class SharedJavascriptTests : InfiniFramePlaywrightTestBase {
    private const string NewTitleFromHostMessage = "newly updated title";

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task InfiniWindowIsInitialized(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();
        var initState = await EvaluateWhenPageReadyAsync<JsonElement>(
            page,
            // lang=javascript
            """
            () => ({
                hasInfiniFrameApi: window.infiniframe !== undefined && window.infiniframe !== null,
                hasNativeHostBridge: window.infiniframe?.host !== undefined && window.infiniframe?.host !== null,
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
    [NotInParallelInfiniAutomationTests]
    public async Task DynamicallyUpdateTitleFromJs(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();
        string? originalTitle = RuntimeContext.Window.Features.Decorations.Title;

        await EvaluateWhenPageReadyAsync(
            page,
            // lang=javascript
            $"() => window.infiniframe?.host?.postData({{ id: '__infiniframe:title:change', command: 'Post', data: '{NewTitleFromHostMessage}', version: 2 }})"
        );
        string? updatedTitle = await WaitForStateChangeAsync(
            originalTitle,
            stateProvider: () => RuntimeContext.Window.Features.Decorations.Title
        );

        await Assert.That(updatedTitle).IsEqualTo(NewTitleFromHostMessage);

        RuntimeContext.Window.Features.Decorations.SetTitle(RuntimeContext.DefaultDocumentTitle);
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task WindowClose(CancellationToken ct = default) {
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
