// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Playwright.TestUtility;
using Microsoft.Playwright;
using InfiniFrameTests.Shared;

namespace InfiniFrameTests.Playwright;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class JavascriptInteropTests : InfiniFrameWebviewTest {

    [Test]
    [NotInParallel(ParallelControl.Playwright)]
    public async Task FullscreenHtmlButton_ShouldToggleInfiniFrameFullscreen() {
        // Arrange
        bool originalFullscreenState = GlobalPlaywrightContext.Window.FullScreen;
        IPage page = await GetRootPageAsync();
        const string buttonId = "#fullscreen-toggle-button";

        // Act
        await page.ClickAsync(buttonId);
        bool newFullscreenState = await WaitForStateChangeAsync(
            originalFullscreenState,
            stateProvider: static () => GlobalPlaywrightContext.Window.FullScreen
        );

        await page.ClickAsync(buttonId);
        bool finalFullscreenState = await WaitForStateChangeAsync(
            newFullscreenState,
            stateProvider: static () => GlobalPlaywrightContext.Window.FullScreen
        );

        // Assert
        await Assert.That(originalFullscreenState).IsFalse();
        await Assert.That(newFullscreenState).IsTrue();
        await Assert.That(finalFullscreenState).IsFalse();
    }

    [Test]
    [NotInParallel(ParallelControl.Playwright)]
    public async Task TitleHtmlButton_ShouldToggleInfiniFrameTitle() {
        // Arrange
        IPage page = await GetRootPageAsync();
        const string buttonId = "#title-toggle-button";
        string originalTitleState = GlobalPlaywrightContext.Window.Title;

        try {
            // Act
            await page.ClickAsync(buttonId);
            await Task.Delay(5_000);
            string newTitleState = await WaitForStateChangeAsync(
                originalTitleState,
                stateProvider: static () => GlobalPlaywrightContext.Window.Title
            );

            await page.ClickAsync(buttonId);
            await Task.Delay(5_000);
            string finalTitleState = await WaitForStateChangeAsync(
                newTitleState,
                stateProvider: static () => GlobalPlaywrightContext.Window.Title
            );

            // Assert
            await Assert.That(originalTitleState).IsEqualTo(GlobalPlaywrightContext.DefaultDocumentTitle);
            await Assert.That(newTitleState).IsEqualTo("New Title");
            await Assert.That(finalTitleState).IsEqualTo(GlobalPlaywrightContext.DefaultDocumentTitle);
        }
        finally {
            GlobalPlaywrightContext.Window.SetTitle(GlobalPlaywrightContext.DefaultDocumentTitle);
            await page.EvaluateAsync(
                // lang=javascript
                $"() => {{ document.title = '{GlobalPlaywrightContext.DefaultDocumentTitle}'; }}"
            );
        }

    }
}
