// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.InfiniFrame;
using Microsoft.Playwright;
using Tests.InfiniFrame.Playwright.Utility;
using Tests.Shared;

namespace Tests.InfiniFrame.Playwright;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class JavascriptInteropTests : InfiniFrameWebviewTest {

    [Test, NotInParallel(ParallelControl.Playwright)]
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

    [Test, NotInParallel(ParallelControl.Playwright)]
    public async Task TitleHtmlButton_ShouldToggleInfiniFrameTitle() {
        // Arrange
        string originalTitleState = GlobalPlaywrightContext.Window.Title;
        IPage page = await GetRootPageAsync();
        const string buttonId = "#title-toggle-button";

        // Act
        await page.ClickAsync(buttonId);
        string newTitleState = await WaitForStateChangeAsync(
            originalTitleState,
            stateProvider: static () => GlobalPlaywrightContext.Window.Title
        );

        await page.ClickAsync(buttonId);
        string finalTitleState = await WaitForStateChangeAsync(
            newTitleState,
            stateProvider: static () => GlobalPlaywrightContext.Window.Title
        );

        // Assert
        await Assert.That(originalTitleState).IsEqualTo(GlobalPlaywrightContext.InfiniFrameWindowTitle);
        await Assert.That(newTitleState).IsEqualTo("New Title");
        await Assert.That(finalTitleState).IsEqualTo(GlobalPlaywrightContext.VueDocumentTitle);

        // Reset
        GlobalPlaywrightContext.Window.SetTitle(GlobalPlaywrightContext.InfiniFrameWindowTitle);

    }
}
