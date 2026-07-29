// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniAutomationTests.BlazorWebView.MudBlazor.TestUtility;
using InfiniAutomationTests.Tests;
using InfiniTests;
using Microsoft.Playwright;
using System.Text.Json;

namespace InfiniAutomationTests.BlazorWebView.MudBlazor;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class FragmentAndCustomSchemeRequestTests : InfiniFramePlaywrightTestBase {
    protected override IPlaywrightRuntimeContext RuntimeContext => PlaywrightContext.Instance;

    [Test]
    [NotInParallelInfiniAutomationTests]
    [Timeout(60_000)]
    public async Task FragmentNavigation_PreservesUrlAndSupportsFetchXhrAndMessaging(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();
        var fragmentUri = new Uri("app://localhost/#settings");

        RuntimeContext.Window.Features.PageNavigation.Load(fragmentUri);
        await page.WaitForFunctionAsync(
            "expectedUrl => window.location.href === expectedUrl",
            fragmentUri.AbsoluteUri,
            new PageWaitForFunctionOptions { Timeout = 20_000 });
        await page.WaitForSelectorAsync("#settings", new PageWaitForSelectorOptions { Timeout = 20_000 });

        JsonElement pageState = await EvaluateWhenPageReadyAsync<JsonElement>(
            page,
            // lang=javascript
            """
            () => ({
                url: window.location.href,
                hash: window.location.hash,
                hasSettings: document.getElementById("settings") !== null
            })
            """
        );

        await Assert.That(page.Url).IsEqualTo(fragmentUri.AbsoluteUri);
        await Assert.That(pageState.GetProperty("url").GetString()).IsEqualTo(fragmentUri.AbsoluteUri);
        await Assert.That(pageState.GetProperty("hash").GetString()).IsEqualTo("#settings");
        await Assert.That(pageState.GetProperty("hasSettings").GetBoolean()).IsTrue();
        await WaitForInfiniFrameReadyAsync(page);

        JsonElement fetchResult = await EvaluateWhenPageReadyAsync<JsonElement>(
            page,
            // lang=javascript
            """
            async () => {
                const controller = new AbortController();
                const timeout = setTimeout(() => controller.abort(), 8_000);
                try {
                    const response = await fetch("app://localhost/fragment-fetch.json", { signal: controller.signal });
                    return {
                        status: response.status,
                        contentType: response.headers.get("content-type"),
                        body: (await response.text()).trim()
                    };
                } finally {
                    clearTimeout(timeout);
                }
            }
            """
        );
        await Assert.That(fetchResult.GetProperty("status").GetInt32()).IsEqualTo(200);
        await Assert.That(fetchResult.GetProperty("contentType").GetString()).StartsWith("application/json");
        await Assert.That(fetchResult.GetProperty("body").GetString())
            .IsEqualTo("{\"message\":\"InfiniFrame fragment fetch payload\"}");

        JsonElement xhrResult = await EvaluateWhenPageReadyAsync<JsonElement>(
            page,
            // lang=javascript
            """
            () => new Promise((resolve, reject) => {
                const xhr = new XMLHttpRequest();
                xhr.open("GET", "app://localhost/fragment-fetch.json");
                xhr.timeout = 8_000;
                xhr.onload = () => resolve({
                    status: xhr.status,
                    contentType: xhr.getResponseHeader("content-type"),
                    body: xhr.responseText.trim()
                });
                xhr.onerror = () => reject(new Error("XMLHttpRequest failed"));
                xhr.ontimeout = () => reject(new Error("XMLHttpRequest timed out"));
                xhr.send();
            })
            """
        );
        await Assert.That(xhrResult.GetProperty("status").GetInt32()).IsEqualTo(200);
        await Assert.That(xhrResult.GetProperty("contentType").GetString()).StartsWith("application/json");
        await Assert.That(xhrResult.GetProperty("body").GetString())
            .IsEqualTo("{\"message\":\"InfiniFrame fragment fetch payload\"}");

        string messageTitle = await EvaluateWhenPageReadyAsync<string>(
            page,
            "async () => await window.infiniframe.window.features.decorations.getTitleAsync()"
        );
        await Assert.That(messageTitle).IsEqualTo(RuntimeContext.Window.Features.Decorations.Title);
    }
}
