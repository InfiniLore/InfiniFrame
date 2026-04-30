// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrameTests.Playwright.BlazorWebView.MudBlazor.TestUtility;
using InfiniFrameTests.Playwright.Tests;
using InfiniFrameTests.Shared;
using Microsoft.Playwright;
using System.Text.Json;

namespace InfiniFrameTests.Playwright.BlazorWebView.MudBlazor;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// ReSharper disable once UnusedType.Global
public sealed class ScriptSrcImportTests : InfiniFramePlaywrightTestBase {
    protected override IPlaywrightRuntimeContext RuntimeContext => PlaywrightContext.Instance;

    [Test]
    [NotInParallel(ParallelControl.Playwright)]
    public async Task ClassicScriptSrc_IsLoaded_AndExecutesCode() {
        IPage page = await GetRootPageAsync();

        var state = await ExecuteWhenPageReadyAsync<JsonElement>(
            page,
            // lang=javascript
            """
            () => ({
                loaded: window.__scriptSrcSmokeLoaded === true,
                runCount: window.__scriptSrcSmokeRunCount ?? 0,
                hasEchoFunction: typeof window.scriptSrcSmokeEcho === "function",
                echoResult: typeof window.scriptSrcSmokeEcho === "function"
                    ? window.scriptSrcSmokeEcho("ok")
                    : null
            })
            """
        );

        await Assert.That(state.GetProperty("loaded").GetBoolean()).IsTrue();
        await Assert.That(state.GetProperty("runCount").GetInt32()).IsGreaterThan(0);
        await Assert.That(state.GetProperty("hasEchoFunction").GetBoolean()).IsTrue();
        await Assert.That(state.GetProperty("echoResult").GetString()).IsEqualTo("script-src-smoke:ok");
    }
}
