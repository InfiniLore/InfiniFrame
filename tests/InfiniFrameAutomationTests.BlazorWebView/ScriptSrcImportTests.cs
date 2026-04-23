// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrameAutomationTests.BlazorWebView.TestUtility;
using InfiniFrameTests.Shared;
using System.Text.Json;
using InfiniFrameAutomationTests.Tests;

namespace InfiniFrameAutomationTests.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// ReSharper disable once UnusedType.Global
public sealed class ScriptSrcImportTests : InfiniFrameAutomationTestBase {
    protected override IAutomationRuntimeContext RuntimeContext => BlazorWebViewAutomationRuntimeContext.Instance;

    [Test]
    [NotInParallel(ParallelControl.Playwright)]
    public async Task ClassicScriptSrc_IsLoaded_AndExecutesCode() {
        IAutomationPage page = await GetRootPageAsync();

        var state = await page.EvaluateAsync<JsonElement>(
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


