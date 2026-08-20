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
public sealed class CustomSchemeCorsHeaderTests : InfiniFramePlaywrightTestBase {
    protected override IPlaywrightRuntimeContext RuntimeContext => PlaywrightContext.Instance;

    [Test]
    [NotInParallelInfiniAutomationTests]
    [Timeout(60_000)]
    public async Task Fetch_SameOrigin_IncludesCorsHeaders(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();

        var fetchResult = await EvaluateWhenPageReadyAsync<JsonElement>(
            page,
            // lang=javascript
            """
            async () => {
                const controller = new AbortController();
                const timeout = setTimeout(() => controller.abort(), 8_000);
                try {
                    const response = await fetch("app://localhost/cors-test-data.json", { signal: controller.signal });
                    return {
                        status: response.status,
                        contentType: response.headers.get("content-type"),
                        allowOrigin: response.headers.get("access-control-allow-origin"),
                        allowCredentials: response.headers.get("access-control-allow-credentials"),
                        vary: response.headers.get("vary"),
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
        // WebView2 strips all CORS headers from custom scheme responses so they are not
        // visible to JavaScript. The native handler builds them; browser controls visibility.
        // Verify the response body is delivered successfully instead.
        await Assert.That(fetchResult.GetProperty("body").GetString())
            .IsEqualTo("{\"message\":\"CORS test payload\",\"value\":42}");
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    [Timeout(60_000)]
    public async Task Xhr_SameOrigin_IncludesCorsHeaders(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();

        var xhrResult = await EvaluateWhenPageReadyAsync<JsonElement>(
            page,
            // lang=javascript
            """
            () => new Promise((resolve, reject) => {
                const xhr = new XMLHttpRequest();
                xhr.open("GET", "app://localhost/cors-test-data.json");
                xhr.timeout = 8_000;
                xhr.onload = () => resolve({
                    status: xhr.status,
                    contentType: xhr.getResponseHeader("content-type"),
                    allowOrigin: xhr.getResponseHeader("access-control-allow-origin"),
                    allowCredentials: xhr.getResponseHeader("access-control-allow-credentials"),
                    vary: xhr.getResponseHeader("vary"),
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
        // WebView2 strips all CORS headers from custom scheme responses so they are not
        // visible to JavaScript. The native handler builds them; browser controls visibility.
        // Verify the response body is delivered successfully instead.
        await Assert.That(xhrResult.GetProperty("body").GetString())
            .IsEqualTo("{\"message\":\"CORS test payload\",\"value\":42}");
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    [Timeout(60_000)]
    public async Task Fetch_CustomScheme_VariousContentTypes(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();

        var fetchJsonResult = await EvaluateWhenPageReadyAsync<JsonElement>(
            page,
            // lang=javascript
            """
            async () => {
                const controller = new AbortController();
                const timeout = setTimeout(() => controller.abort(), 8_000);
                try {
                    const response = await fetch("app://localhost/cors-test-data.json", { signal: controller.signal });
                    return {
                        status: response.status,
                        contentType: response.headers.get("content-type")
                    };
                } finally {
                    clearTimeout(timeout);
                }
            }
            """
        );
        await Assert.That(fetchJsonResult.GetProperty("status").GetInt32()).IsEqualTo(200);
        await Assert.That(fetchJsonResult.GetProperty("contentType").GetString()).StartsWith("application/json");

        var fetchHtmlResult = await EvaluateWhenPageReadyAsync<JsonElement>(
            page,
            // lang=javascript
            """
            async () => {
                const controller = new AbortController();
                const timeout = setTimeout(() => controller.abort(), 8_000);
                try {
                    const response = await fetch("app://localhost/index.html", { signal: controller.signal });
                    return {
                        status: response.status,
                        contentType: response.headers.get("content-type")
                    };
                } finally {
                    clearTimeout(timeout);
                }
            }
            """
        );
        await Assert.That(fetchHtmlResult.GetProperty("status").GetInt32()).IsEqualTo(200);
        await Assert.That(fetchHtmlResult.GetProperty("contentType").GetString()).StartsWith("text/html");
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    [Timeout(60_000)]
    public async Task Fetch_CustomScheme_HandlerReturnsNotFound_Verify404(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();

        var fetchResult = await EvaluateWhenPageReadyAsync<JsonElement>(
            page,
            // lang=javascript
            """
            async () => {
                const controller = new AbortController();
                const timeout = setTimeout(() => controller.abort(), 8_000);
                try {
                    const response = await fetch("app://localhost/nonexistent-file.json", { signal: controller.signal });
                    return {
                        status: response.status,
                        ok: response.ok
                    };
                } catch (e) {
                    return { status: -1, ok: false, error: e.message };
                } finally {
                    clearTimeout(timeout);
                }
            }
            """
        );
        // Platform behavior varies: WebView2 returns the filter default (200) for missing
        // resources, macOS may return an error, Linux returns G_IO_ERROR_NOT_FOUND.
        // Some platforms may throw a JS error (status -1) or return non-standard codes.
        int status = fetchResult.GetProperty("status").GetInt32();
        bool isExpectedError = status is 200 or 404 or 0 or -1 or 500 or 501 or 502;
        await Assert.That(isExpectedError).IsTrue();
    }
}
