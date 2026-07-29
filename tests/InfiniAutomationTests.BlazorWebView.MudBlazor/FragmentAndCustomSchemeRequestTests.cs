// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniAutomationTests.BlazorWebView.MudBlazor.TestUtility;
using InfiniAutomationTests.Tests;
using InfiniFrame.BlazorWebView;
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
    public async Task FragmentNavigation_PreservesUrlAndSupportsFetchXhrAndMessaging(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();
        var fragmentUri = new Uri("app://localhost/index.html#settings");

        try {
            RuntimeContext.Window.Features.PageNavigation.Load(fragmentUri);
            await page.WaitForURLAsync(
                fragmentUri.AbsoluteUri,
                new PageWaitForURLOptions { WaitUntil = WaitUntilState.DOMContentLoaded, Timeout = 20_000 });
            await WaitForInfiniFrameReadyAsync(page);

            JsonElement result = await EvaluateWhenPageReadyAsync<JsonElement>(
                page,
                // lang=javascript
                """
                async () => {
                    const fetchResponse = await fetch("app://localhost/fragment-fetch.txt");
                    const xhrResult = await new Promise((resolve, reject) => {
                        const xhr = new XMLHttpRequest();
                        xhr.open("GET", "app://localhost/fragment-fetch.txt");
                        xhr.onload = () => resolve({ status: xhr.status, body: xhr.responseText.trim() });
                        xhr.onerror = () => reject(new Error("XMLHttpRequest failed"));
                        xhr.send();
                    });
                    const title = await window.infiniframe.window.features.decorations.getTitleAsync();

                    return {
                        url: window.location.href,
                        hash: window.location.hash,
                        hasSettings: document.getElementById("settings") !== null,
                        fetchStatus: fetchResponse.status,
                        fetchBody: (await fetchResponse.text()).trim(),
                        xhrStatus: xhrResult.status,
                        xhrBody: xhrResult.body,
                        messageTitle: title
                    };
                }
                """
            );

            await Assert.That(page.Url).IsEqualTo(fragmentUri.AbsoluteUri);
            await Assert.That(result.GetProperty("url").GetString()).IsEqualTo(fragmentUri.AbsoluteUri);
            await Assert.That(result.GetProperty("hash").GetString()).IsEqualTo("#settings");
            await Assert.That(result.GetProperty("hasSettings").GetBoolean()).IsTrue();
            await Assert.That(result.GetProperty("fetchStatus").GetInt32()).IsEqualTo(200);
            await Assert.That(result.GetProperty("fetchBody").GetString()).IsEqualTo("InfiniFrame fragment fetch payload");
            await Assert.That(result.GetProperty("xhrStatus").GetInt32()).IsEqualTo(200);
            await Assert.That(result.GetProperty("xhrBody").GetString()).IsEqualTo("InfiniFrame fragment fetch payload");
            await Assert.That(result.GetProperty("messageTitle").GetString()).IsEqualTo(RuntimeContext.Window.Features.Decorations.Title);
        }
        finally {
            RuntimeContext.Window.Features.PageNavigation.Load(new Uri(InfiniFrameWebViewManager.AppBaseUri));
        }
    }
}
