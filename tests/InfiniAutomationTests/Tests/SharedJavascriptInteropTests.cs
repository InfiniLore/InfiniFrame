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
public abstract class SharedJavascriptInteropTests : InfiniFramePlaywrightTestBase {
    protected abstract string FullscreenToggleButtonSelector { get; }

    protected abstract string TitleToggleButtonSelector { get; }

    protected virtual string ToggledTitle => "New Title";

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task BrowserFeature_Getters_ShouldMirrorNativeWindow(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();
        JsonElement actual = await EvaluateWhenPageReadyAsync<JsonElement>(page,
            // lang=javascript
            """
            async () => {
                const browser = window.infiniframe.window.features.browser;
                return {
                    contextMenu: await browser.isContextMenuEnabledAsync(),
                    mediaAutoplay: await browser.isMediaAutoplayEnabledAsync(),
                    userAgent: await browser.getUserAgentAsync(),
                    webSecurity: await browser.isWebSecurityEnabledAsync(),
                    smoothScrolling: await browser.isSmoothScrollingEnabledAsync()
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
        JsonElement actual = await EvaluateWhenPageReadyAsync<JsonElement>(page,
            // lang=javascript
            """
            async () => {
                const decorations = window.infiniframe.window.features.decorations;
                return {
                    chromeless: await decorations.isChromelessAsync(),
                    transparent: await decorations.isTransparentAsync(),
                    title: await decorations.getTitleAsync(),
                    limitLinuxTitle: await decorations.getLimitLinuxWindowTitleLengthAsync()
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
        JsonElement actual = await EvaluateWhenPageReadyAsync<JsonElement>(page,
            // lang=javascript
            """
            async () => {
                const position = window.infiniframe.window.features.position;
                return { location: await position.getLocationAsync(), top: await position.getTopAsync(), left: await position.getLeftAsync() };
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
        JsonElement actual = await EvaluateWhenPageReadyAsync<JsonElement>(page,
            // lang=javascript
            """
            async () => {
                const size = window.infiniframe.window.features.size;
                return { size: await size.getSizeAsync(), width: await size.getWidthAsync(), height: await size.getHeightAsync(), resizable: await size.isResizableAsync() };
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
        JsonElement actual = await EvaluateWhenPageReadyAsync<JsonElement>(page,
            // lang=javascript
            """
            async () => {
                const state = window.infiniframe.window.features.state;
                return {
                    fullScreen: await state.isFullScreenAsync(), maximized: await state.isMaximizedAsync(),
                    minimized: await state.isMinimizedAsync(), topMost: await state.isTopMostAsync(),
                    zoomFactor: await state.getZoomFactorAsync(), zoomEnabled: await state.isZoomEnabledAsync()
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
        JsonElement actual = await EvaluateWhenPageReadyAsync<JsonElement>(page,
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

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task FullscreenHtmlButton_ShouldToggleInfiniFrameFullscreen(CancellationToken ct = default) {
        bool originalFullscreenState = RuntimeContext.Window.Features.State.IsFullScreen;
        IPage page = await GetRootPageAsync();

        await page.ClickAsync(FullscreenToggleButtonSelector);
        bool newFullscreenState = await WaitForStateChangeAsync(
            originalFullscreenState,
            stateProvider: () => RuntimeContext.Window.Features.State.IsFullScreen
        );

        await page.ClickAsync(FullscreenToggleButtonSelector);
        bool finalFullscreenState = await WaitForStateChangeAsync(
            newFullscreenState,
            stateProvider: () => RuntimeContext.Window.Features.State.IsFullScreen
        );

        await Assert.That(originalFullscreenState).IsFalse();
        await Assert.That(newFullscreenState).IsTrue();
        await Assert.That(finalFullscreenState).IsFalse();
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task TitleHtmlButton_ShouldToggleInfiniFrameTitle(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();
        string? originalTitleState = RuntimeContext.Window.Features.Decorations.Title;

        try {
            await page.ClickAsync(TitleToggleButtonSelector);
            string? newTitleState = await WaitForStateChangeAsync(
                originalTitleState,
                stateProvider: () => RuntimeContext.Window.Features.Decorations.Title
            );

            await page.ClickAsync(TitleToggleButtonSelector);
            string? finalTitleState = await WaitForStateChangeAsync(
                newTitleState,
                stateProvider: () => RuntimeContext.Window.Features.Decorations.Title
            );

            await Assert.That(originalTitleState).IsEqualTo(RuntimeContext.DefaultDocumentTitle);
            await Assert.That(newTitleState).IsEqualTo(ToggledTitle);
            await Assert.That(finalTitleState).IsEqualTo(RuntimeContext.DefaultDocumentTitle);
        }
        finally {
            RuntimeContext.Window.Features.Decorations.SetTitle(RuntimeContext.DefaultDocumentTitle);
            await EvaluateWhenPageReadyAsync(
                page,
                // lang=javascript
                $"() => {{ document.title = '{RuntimeContext.DefaultDocumentTitle}'; }}"
            );
        }
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task GetTitleAsyncFromJs_ShouldReturnNativeWindowTitle(CancellationToken ct = default) {
        // Arrange
        IPage page = await GetRootPageAsync();
        string? originalTitleState = RuntimeContext.Window.Features.Decorations.Title;

        // Act
        string titleFromJsInitially = await EvaluateWhenPageReadyAsync<string>(
            page,
            // lang=javascript
            "async () => await window.infiniframe.window.features.decorations.getTitleAsync()"
        );

        // Assert
        await Assert.That(titleFromJsInitially).IsEqualTo(originalTitleState);
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task GetTitleAsyncFromJs_ShouldReturnNativeWindowTitle_AndShouldReturnCorrectTitle(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();
        string? originalTitleState = RuntimeContext.Window.Features.Decorations.Title;

        string? titleFromJsInitially = await EvaluateWhenPageReadyAsync<string?>(
            page,
            // lang=javascript
            "async () => await window.infiniframe.window.features.decorations.getTitleAsync()"
        );

        await Assert.That(titleFromJsInitially).IsEqualTo(originalTitleState);

        await page.ClickAsync(TitleToggleButtonSelector);
        string? toggledTitle = await WaitForStateChangeAsync(
            originalTitleState,
            stateProvider: () => RuntimeContext.Window.Features.Decorations.Title
        );

        string? titleFromJs = await EvaluateWhenPageReadyAsync<string?>(
            page,
            // lang=javascript
            "async () => await window.infiniframe.window.features.decorations.getTitleAsync()"
        );

        await Assert.That(toggledTitle).IsEqualTo(ToggledTitle)
            .And!.IsNotEqualTo(originalTitleState);
        await Assert.That(titleFromJs).IsEqualTo(ToggledTitle)
            .And!.IsNotEqualTo(originalTitleState);
    }
}
