// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Playwright.TestUtility;
using Microsoft.Playwright;
using InfiniFrameTests.Shared;
using System.Text.Json;
using TUnit.Core.Executors;

namespace InfiniFrameTests.Playwright;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class JavascriptTests : InfiniFrameWebviewTest {

    [Test, NotInParallel(ParallelControl.Playwright)]
    [TestExecutor<UiThreadExecutor>]
    public async Task InfiniWindowIsInitialized() {
        // Arrange
        IPage page = await GetRootPageAsync();

        // Act
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

        // Assert
        await Assert.That(initState.GetProperty("hasHostBridge").GetBoolean()).IsTrue();
        await Assert.That(initState.GetProperty("hasInfiniFrameApi").GetBoolean()).IsTrue();
        await Assert.That(initState.GetProperty("hasHostMessaging").GetBoolean()).IsTrue();
        await Assert.That(initState.GetProperty("hasSendMessageToHost").GetBoolean()).IsTrue();
    }

    [Test, NotInParallel(ParallelControl.Playwright)]
    [TestExecutor<UiThreadExecutor>]
    public async Task DynamicallyUpdateTitleFromJs() {
        // Arrange
        IPage page = await GetRootPageAsync();
        string originalTitle = GlobalPlaywrightContext.Window.Title;
        const string newTitle = "newly updated title";

        // Act
        await page.EvaluateAsync(
            // lang=javascript 
            $"() => window.infiniframe?.host?.postMessage({{ id: '__infiniframe:title:change', data: '{newTitle}', version: 1 }})"
        );
        string updatedTitle = await WaitForStateChangeAsync(originalTitle, stateProvider: () => GlobalPlaywrightContext.Window.Title);

        // Assert
        await Assert.That(updatedTitle).IsEqualTo(newTitle);

        // Reset
        GlobalPlaywrightContext.Window.SetTitle(GlobalPlaywrightContext.DefaultDocumentTitle);
    }

    [Test, NotInParallel(ParallelControl.Playwright)]
    [TestExecutor<UiThreadExecutor>]
    public async Task WindowClose() {
        // Arrange
        IPage page = await GetRootPageAsync();
        int initialCloseRequestCount = GlobalPlaywrightContext.GetWindowCloseRequestCount();
        GlobalPlaywrightContext.SuppressWindowCloseRequests(true);

        try {
            // Act
            await page.EvaluateAsync(
                // lang=javascript 
                "() => window.close()"
            );
            int closeRequestCount = await WaitForStateChangeAsync(
                initialCloseRequestCount,
                stateProvider: static () => GlobalPlaywrightContext.GetWindowCloseRequestCount()
            );

            // Assert
            await Assert.That(closeRequestCount).IsEqualTo(initialCloseRequestCount + 1);
        }
        finally {
            GlobalPlaywrightContext.SuppressWindowCloseRequests(false);
        }
    }
}
