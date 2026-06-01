// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniTests;
using Microsoft.Playwright;

namespace InfiniAutomationTests.Tests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class SharedJavascriptInteropTests : InfiniFramePlaywrightTestBase {
    protected abstract string FullscreenToggleButtonSelector { get; }

    protected abstract string TitleToggleButtonSelector { get; }

    protected virtual string ToggledTitle => "New Title";

    [Test]
    [NotInParallel(ParallelControl.Playwright)]
    public async Task FullscreenHtmlButton_ShouldToggleInfiniFrameFullscreen(CancellationToken ct = default) {
        bool originalFullscreenState = RuntimeContext.Window.FullScreen;
        IPage page = await GetRootPageAsync();

        await page.ClickAsync(FullscreenToggleButtonSelector);
        bool newFullscreenState = await WaitForStateChangeAsync(
            originalFullscreenState,
            stateProvider: () => RuntimeContext.Window.FullScreen
        );

        await page.ClickAsync(FullscreenToggleButtonSelector);
        bool finalFullscreenState = await WaitForStateChangeAsync(
            newFullscreenState,
            stateProvider: () => RuntimeContext.Window.FullScreen
        );

        await Assert.That(originalFullscreenState).IsFalse();
        await Assert.That(newFullscreenState).IsTrue();
        await Assert.That(finalFullscreenState).IsFalse();
    }

    [Test]
    [NotInParallel(ParallelControl.Playwright)]
    public async Task TitleHtmlButton_ShouldToggleInfiniFrameTitle(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();
        string? originalTitleState = RuntimeContext.Window.Title;

        try {
            await page.ClickAsync(TitleToggleButtonSelector);
            string? newTitleState = await WaitForStateChangeAsync(
                originalTitleState,
                stateProvider: () => RuntimeContext.Window.Title
            );

            await page.ClickAsync(TitleToggleButtonSelector);
            string? finalTitleState = await WaitForStateChangeAsync(
                newTitleState,
                stateProvider: () => RuntimeContext.Window.Title
            );

            await Assert.That(originalTitleState).IsEqualTo(RuntimeContext.DefaultDocumentTitle);
            await Assert.That(newTitleState).IsEqualTo(ToggledTitle);
            await Assert.That(finalTitleState).IsEqualTo(RuntimeContext.DefaultDocumentTitle);
        }
        finally {
            RuntimeContext.Window.SetTitle(RuntimeContext.DefaultDocumentTitle);
            await EvaluateWhenPageReadyAsync(
                page,
                // lang=javascript
                $"() => {{ document.title = '{RuntimeContext.DefaultDocumentTitle}'; }}"
            );
        }
    }

    [Test]
    [NotInParallel(ParallelControl.Playwright)]
    public async Task GetTitleAsyncFromJs_ShouldReturnNativeWindowTitle(CancellationToken ct = default) {
        // Arrange
        IPage page = await GetRootPageAsync();
        string? originalTitleState = RuntimeContext.Window.Title;

        // Act
        string titleFromJsInitially = await EvaluateWhenPageReadyAsync<string>(
            page,
            // lang=javascript
            "async () => await window.infiniframe.window.getTitleAsync()"
        );

        // Assert
        await Assert.That(titleFromJsInitially).IsEqualTo(originalTitleState);
    }

    [Test]
    [NotInParallel(ParallelControl.Playwright)]
    public async Task GetTitleAsyncFromJs_ShouldReturnNativeWindowTitle_AndShouldReturnCorrectTitle(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();
        string? originalTitleState = RuntimeContext.Window.Title;

        string? titleFromJsInitially = await EvaluateWhenPageReadyAsync<string?>(
            page,
            // lang=javascript
            "async () => await window.infiniframe.window.getTitleAsync()"
        );

        await Assert.That(titleFromJsInitially).IsEqualTo(originalTitleState);

        await page.ClickAsync(TitleToggleButtonSelector);
        string? toggledTitle = await WaitForStateChangeAsync(
            originalTitleState,
            stateProvider: () => RuntimeContext.Window.Title
        );

        string? titleFromJs = await EvaluateWhenPageReadyAsync<string?>(
            page,
            // lang=javascript
            "async () => await window.infiniframe.window.getTitleAsync()"
        );
        
        await Assert.That(toggledTitle).IsEqualTo(ToggledTitle)
            .And!.IsNotEqualTo(originalTitleState);
        await Assert.That(titleFromJs).IsEqualTo(ToggledTitle)
            .And!.IsNotEqualTo(originalTitleState);
    }
}
