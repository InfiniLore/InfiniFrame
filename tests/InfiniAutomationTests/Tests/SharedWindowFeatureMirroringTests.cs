// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniTests;
using Microsoft.Playwright;
using System.Drawing;
using System.Text.Json;

namespace InfiniAutomationTests.Tests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class SharedWindowFeatureMirroringTests : InfiniFramePlaywrightTestBase {
    private static async Task<JsonElement> ProbeFeatureAsync(IPage page, string feature) {
        await page.ClickAsync($"#probe-{feature}-feature");
        ILocator output = page.Locator($"#{feature}-feature-result");
        await page.WaitForFunctionAsync(
            "element => (element.value ?? element.textContent)?.trim().startsWith('{') === true",
            await output.ElementHandleAsync()
        );
        string serializedData = await output.EvaluateAsync<string>(
            "element => element.value ?? element.textContent ?? ''"
        );
        using JsonDocument document = JsonDocument.Parse(serializedData);
        return document.RootElement.Clone();
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task BrowserFeature_Getters_ShouldMirrorNativeWindow(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();
        JsonElement actual = await ProbeFeatureAsync(page, "browser");

        await Assert.That(actual.GetProperty("contextMenu").GetBoolean()).IsEqualTo(RuntimeContext.Window.Features.Browser.IsContextMenuEnabled);
        await Assert.That(actual.GetProperty("mediaAutoplay").GetBoolean()).IsEqualTo(RuntimeContext.Window.Features.Browser.IsMediaAutoplayEnabled);
        await Assert.That(actual.GetProperty("userAgent").GetString()).IsEqualTo(RuntimeContext.Window.Features.Browser.UserAgent);
        await Assert.That(actual.GetProperty("webSecurity").GetBoolean()).IsEqualTo(RuntimeContext.Window.Features.Browser.IsWebSecurityEnabled);
        await Assert.That(actual.GetProperty("smoothScrolling").GetBoolean()).IsEqualTo(RuntimeContext.Window.Features.Browser.IsSmoothScrollingEnabled);
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task DecorationsFeature_Getters_ShouldMirrorNativeWindow(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();
        JsonElement actual = await ProbeFeatureAsync(page, "decorations");

        await Assert.That(actual.GetProperty("chromeless").GetBoolean()).IsEqualTo(RuntimeContext.Window.Features.Decorations.IsChromeless);
        await Assert.That(actual.GetProperty("transparent").GetBoolean()).IsEqualTo(RuntimeContext.Window.Features.Decorations.IsTransparent);
        await Assert.That(actual.GetProperty("title").GetString()).IsEqualTo(RuntimeContext.Window.Features.Decorations.Title);
        await Assert.That(actual.GetProperty("limitLinuxTitle").GetBoolean()).IsEqualTo(RuntimeContext.Window.Features.Decorations.LimitLinuxWindowTitleLength);
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task PositionFeature_Getters_ShouldMirrorNativeWindow(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();
        JsonElement actual = await ProbeFeatureAsync(page, "position");

        await Assert.That(actual.GetProperty("location").GetProperty("x").GetInt32()).IsEqualTo(RuntimeContext.Window.Features.Position.Location.X);
        await Assert.That(actual.GetProperty("location").GetProperty("y").GetInt32()).IsEqualTo(RuntimeContext.Window.Features.Position.Location.Y);
        await Assert.That(actual.GetProperty("top").GetInt32()).IsEqualTo(RuntimeContext.Window.Features.Position.Top);
        await Assert.That(actual.GetProperty("left").GetInt32()).IsEqualTo(RuntimeContext.Window.Features.Position.Left);
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task SizeFeature_Getters_ShouldMirrorNativeWindow(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();
        JsonElement actual = await ProbeFeatureAsync(page, "size");

        await Assert.That(actual.GetProperty("size").GetProperty("width").GetInt32()).IsEqualTo(RuntimeContext.Window.Features.Size.Width);
        await Assert.That(actual.GetProperty("size").GetProperty("height").GetInt32()).IsEqualTo(RuntimeContext.Window.Features.Size.Height);
        await Assert.That(actual.GetProperty("width").GetInt32()).IsEqualTo(RuntimeContext.Window.Features.Size.Width);
        await Assert.That(actual.GetProperty("height").GetInt32()).IsEqualTo(RuntimeContext.Window.Features.Size.Height);
        await Assert.That(actual.GetProperty("resizable").GetBoolean()).IsEqualTo(RuntimeContext.Window.Features.Size.IsResizable);
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task StateFeature_Getters_ShouldMirrorNativeWindow(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();
        JsonElement actual = await ProbeFeatureAsync(page, "state");

        await Assert.That(actual.GetProperty("fullScreen").GetBoolean()).IsEqualTo(RuntimeContext.Window.Features.State.IsFullScreen);
        await Assert.That(actual.GetProperty("maximized").GetBoolean()).IsEqualTo(RuntimeContext.Window.Features.State.IsMaximized);
        await Assert.That(actual.GetProperty("minimized").GetBoolean()).IsEqualTo(RuntimeContext.Window.Features.State.IsMinimized);
        await Assert.That(actual.GetProperty("topMost").GetBoolean()).IsEqualTo(RuntimeContext.Window.Features.State.IsTopMost);
        await Assert.That(actual.GetProperty("zoomFactor").GetInt32()).IsEqualTo(RuntimeContext.Window.Features.State.ZoomFactor);
        await Assert.That(actual.GetProperty("zoomEnabled").GetBoolean()).IsEqualTo(RuntimeContext.Window.Features.State.IsZoomEnabled);
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task LifecycleAndMonitorFeatures_Getters_ShouldMirrorNativeWindow(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();
        JsonElement actual = await ProbeFeatureAsync(page, "lifecycle-monitors");

        await Assert.That(actual.GetProperty("closedOrClosing").GetBoolean()).IsEqualTo(RuntimeContext.Window.Features.Lifecycle.IsClosedOrClosing());
        await Assert.That(actual.GetProperty("dpi").GetDouble()).IsEqualTo(RuntimeContext.Window.Features.Monitors.GetMainMonitorScreenDpi());
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task StateCachedBounds_ShouldMirrorInBothDirectionsAndRestoreState(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();
        IStateInfiniFrameWindowFeature state = RuntimeContext.Window.Features.State;
        Rectangle originalFullScreen = state.CachedPreFullScreenBounds;
        Rectangle originalMaximized = state.CachedPreMaximizedBounds;
        var fromJavascriptFullScreen = new Rectangle(11, 22, 833, 611);
        var fromJavascriptMaximized = new Rectangle(33, 44, 1055, 799);
        var fromNativeFullScreen = new Rectangle(55, 66, 877, 633);
        var fromNativeMaximized = new Rectangle(77, 88, 1099, 811);

        try {
            await page.EvaluateAsync(
                "bounds => { const state = window.infiniframe.window.features.state; state.setCachedPreFullScreenBounds(bounds.fullScreen); state.setCachedPreMaximizedBounds(bounds.maximized); }",
                new {
                    fullScreen = ToJsonShape(fromJavascriptFullScreen),
                    maximized = ToJsonShape(fromJavascriptMaximized)
                });

            await WaitForBoundsAsync(state, fromJavascriptFullScreen, fromJavascriptMaximized, ct);
            state.CachedPreFullScreenBounds = fromNativeFullScreen;
            state.CachedPreMaximizedBounds = fromNativeMaximized;

            var returned = await page.EvaluateAsync<JsonElement>(
                "async () => { const state = window.infiniframe.window.features.state; return { fullScreen: await state.getCachedPreFullScreenBoundsAsync(), maximized: await state.getCachedPreMaximizedBoundsAsync() }; }");

            await AssertRectangleAsync(returned.GetProperty("fullScreen"), fromNativeFullScreen);
            await AssertRectangleAsync(returned.GetProperty("maximized"), fromNativeMaximized);
        }
        finally {
            state.CachedPreFullScreenBounds = originalFullScreen;
            state.CachedPreMaximizedBounds = originalMaximized;
        }
    }

    private static object ToJsonShape(Rectangle value)
        => new { x = value.X, y = value.Y, width = value.Width, height = value.Height };

    private static async Task WaitForBoundsAsync(
        IStateInfiniFrameWindowFeature state,
        Rectangle fullScreen,
        Rectangle maximized,
        CancellationToken ct
    ) {
        for (int attempt = 0; attempt < 50; attempt++) {
            if (state.CachedPreFullScreenBounds == fullScreen && state.CachedPreMaximizedBounds == maximized) return;

            await Task.Delay(20, ct);
        }

        throw new TimeoutException("JavaScript cached-bounds mutations did not reach the native feature state.");
    }

    private static async Task AssertRectangleAsync(JsonElement actual, Rectangle expected) {
        await Assert.That(actual.GetProperty("x").GetInt32()).IsEqualTo(expected.X);
        await Assert.That(actual.GetProperty("y").GetInt32()).IsEqualTo(expected.Y);
        await Assert.That(actual.GetProperty("width").GetInt32()).IsEqualTo(expected.Width);
        await Assert.That(actual.GetProperty("height").GetInt32()).IsEqualTo(expected.Height);
    }
}