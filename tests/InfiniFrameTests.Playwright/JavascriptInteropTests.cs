// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
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
        bool originalFullscreenState = await GlobalPlaywrightContext.GetWindowFullscreenAsync();
        IPage page = await GetRootPageAsync();
        const string buttonId = "#fullscreen-toggle-button";

        // Act
        await page.ClickAsync(buttonId);
        bool newFullscreenState = await WaitForStateChangeAsync(
            originalFullscreenState,
            stateProvider: static () => GlobalPlaywrightContext.GetWindowFullscreenAsync()
        );

        await page.ClickAsync(buttonId);
        bool finalFullscreenState = await WaitForStateChangeAsync(
            newFullscreenState,
            stateProvider: static () => GlobalPlaywrightContext.GetWindowFullscreenAsync()
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
        string originalTitleState = await GlobalPlaywrightContext.GetWindowTitleAsync();

        try {
            // Act
            await page.ClickAsync(buttonId);
            await Task.Delay(5_000);
            string newTitleState = await WaitForStateChangeAsync(
                originalTitleState,
                stateProvider: static () => GlobalPlaywrightContext.GetWindowTitleAsync()
            );

            await page.ClickAsync(buttonId);
            await Task.Delay(5_000);
            string finalTitleState = await WaitForStateChangeAsync(
                newTitleState,
                stateProvider: static () => GlobalPlaywrightContext.GetWindowTitleAsync()
            );

            // Assert
            await Assert.That(originalTitleState).IsEqualTo(GlobalPlaywrightContext.DefaultDocumentTitle);
            await Assert.That(newTitleState).IsEqualTo("New Title");
            await Assert.That(finalTitleState).IsEqualTo(GlobalPlaywrightContext.DefaultDocumentTitle);
        }
        finally {
            await GlobalPlaywrightContext.SetWindowTitleAsync(GlobalPlaywrightContext.DefaultDocumentTitle);
            await page.EvaluateAsync(
                // lang=javascript
                $"() => {{ document.title = '{GlobalPlaywrightContext.DefaultDocumentTitle}'; }}"
            );
        }

    }
}
