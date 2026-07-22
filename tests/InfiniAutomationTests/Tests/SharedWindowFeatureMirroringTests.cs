// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniTests;
using Microsoft.Playwright;
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
            "element => element.textContent?.trim().startsWith('{') === true",
            await output.ElementHandleAsync()
        );
        using JsonDocument document = JsonDocument.Parse((await output.TextContentAsync())!);
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
}
