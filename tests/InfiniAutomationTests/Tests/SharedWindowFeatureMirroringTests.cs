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
    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task BrowserFeature_Getters_ShouldMirrorNativeWindow(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();
        var actual = await EvaluateWhenPageReadyAsync<JsonElement>(page,
            // lang=javascript
            """
            async () => {
                const feature = window.infiniframe.window.features.browser;
                return {
                    contextMenu: await feature.isContextMenuEnabledAsync(),
                    mediaAutoplay: await feature.isMediaAutoplayEnabledAsync(),
                    userAgent: await feature.getUserAgentAsync(),
                    webSecurity: await feature.isWebSecurityEnabledAsync(),
                    smoothScrolling: await feature.isSmoothScrollingEnabledAsync()
                };
            }
            """);

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
        var actual = await EvaluateWhenPageReadyAsync<JsonElement>(page,
            // lang=javascript
            """
            async () => {
                const feature = window.infiniframe.window.features.decorations;
                return {
                    chromeless: await feature.isChromelessAsync(), transparent: await feature.isTransparentAsync(),
                    title: await feature.getTitleAsync(), limitLinuxTitle: await feature.getLimitLinuxWindowTitleLengthAsync()
                };
            }
            """);

        await Assert.That(actual.GetProperty("chromeless").GetBoolean()).IsEqualTo(RuntimeContext.Window.Features.Decorations.IsChromeless);
        await Assert.That(actual.GetProperty("transparent").GetBoolean()).IsEqualTo(RuntimeContext.Window.Features.Decorations.IsTransparent);
        await Assert.That(actual.GetProperty("title").GetString()).IsEqualTo(RuntimeContext.Window.Features.Decorations.Title);
        await Assert.That(actual.GetProperty("limitLinuxTitle").GetBoolean()).IsEqualTo(RuntimeContext.Window.Features.Decorations.LimitLinuxWindowTitleLength);
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task PositionFeature_Getters_ShouldMirrorNativeWindow(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();
        var actual = await EvaluateWhenPageReadyAsync<JsonElement>(page,
            // lang=javascript
            """
            async () => {
                const feature = window.infiniframe.window.features.position;
                return { location: await feature.getLocationAsync(), top: await feature.getTopAsync(), left: await feature.getLeftAsync() };
            }
            """);

        await Assert.That(actual.GetProperty("location").GetProperty("x").GetInt32()).IsEqualTo(RuntimeContext.Window.Features.Position.Location.X);
        await Assert.That(actual.GetProperty("location").GetProperty("y").GetInt32()).IsEqualTo(RuntimeContext.Window.Features.Position.Location.Y);
        await Assert.That(actual.GetProperty("top").GetInt32()).IsEqualTo(RuntimeContext.Window.Features.Position.Top);
        await Assert.That(actual.GetProperty("left").GetInt32()).IsEqualTo(RuntimeContext.Window.Features.Position.Left);
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task SizeFeature_Getters_ShouldMirrorNativeWindow(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();
        var actual = await EvaluateWhenPageReadyAsync<JsonElement>(page,
            // lang=javascript
            """
            async () => {
                const feature = window.infiniframe.window.features.size;
                return { size: await feature.getSizeAsync(), width: await feature.getWidthAsync(), height: await feature.getHeightAsync(), resizable: await feature.isResizableAsync() };
            }
            """);

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
        var actual = await EvaluateWhenPageReadyAsync<JsonElement>(page,
            // lang=javascript
            """
            async () => {
                const feature = window.infiniframe.window.features.state;
                return {
                    fullScreen: await feature.isFullScreenAsync(), maximized: await feature.isMaximizedAsync(),
                    minimized: await feature.isMinimizedAsync(), topMost: await feature.isTopMostAsync(),
                    zoomFactor: await feature.getZoomFactorAsync(), zoomEnabled: await feature.isZoomEnabledAsync()
                };
            }
            """);

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
        var actual = await EvaluateWhenPageReadyAsync<JsonElement>(page,
            // lang=javascript
            """
            async () => ({
                closedOrClosing: await window.infiniframe.window.features.lifecycle.isClosedOrClosingAsync(),
                dpi: await window.infiniframe.window.features.monitors.getMainMonitorScreenDpiAsync()
            })
            """);

        await Assert.That(actual.GetProperty("closedOrClosing").GetBoolean()).IsEqualTo(RuntimeContext.Window.Features.Lifecycle.IsClosedOrClosing());
        await Assert.That(actual.GetProperty("dpi").GetDouble()).IsEqualTo(RuntimeContext.Window.Features.Monitors.GetMainMonitorScreenDpi());
    }
}
