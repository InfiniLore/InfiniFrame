// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Playwright;
using Tests.Photino.Playwright.Utility;
using Tests.Shared.Photino;

namespace Tests.Photino.Playwright;
using InfiniLore.Photino.NET;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class JavascriptInteropTests : PhotinoWebviewTest {
    
    [Test]
    [NotInParallel(ParallelControl.Playwright)]
    public async Task FullscreenHtmlButton_ShouldTogglePhotinoFullscreen() {
        // Arrange
        bool originalFullscreenState = GlobalPlaywrightContext.Window.FullScreen;
        IPage page = await GetRootPageAsync();
        const string buttonId = "#fullscreen-toggle-button";
        
        // Act
        await page.ClickAsync(buttonId);
        bool newFullscreenState = await WaitForStateChangeAsync(
            originalFullscreenState, 
            static () => GlobalPlaywrightContext.Window.FullScreen
        ) ;
        
        await page.ClickAsync(buttonId);
        bool finalFullscreenState = await WaitForStateChangeAsync(
            newFullscreenState, 
            static () => GlobalPlaywrightContext.Window.FullScreen
        );
        
        // Assert
        await Assert.That(originalFullscreenState).IsFalse();
        await Assert.That(newFullscreenState).IsTrue();
        await Assert.That(finalFullscreenState).IsFalse();
    }
    
    [Test]
    [NotInParallel(ParallelControl.Playwright)]
    public async Task TitleHtmlButton_ShouldTogglePhotinoTitle() {
        // Arrange
        string originalTitleState = GlobalPlaywrightContext.Window.Title;
        IPage page = await GetRootPageAsync();
        const string buttonId = "#title-toggle-button";
        
        // Act
        await page.ClickAsync(buttonId);
        string newTitleState = await WaitForStateChangeAsync(
            originalTitleState, 
            static () => GlobalPlaywrightContext.Window.Title
        ) ;
        
        await page.ClickAsync(buttonId);
        string finalTitleState = await WaitForStateChangeAsync(
            newTitleState, 
            static () => GlobalPlaywrightContext.Window.Title
        );
        
        // Assert
        await Assert.That(originalTitleState).IsEqualTo(GlobalPlaywrightContext.PhotinoWindowTitle);
        await Assert.That(newTitleState).IsEqualTo("New Title");
        await Assert.That(finalTitleState).IsEqualTo(GlobalPlaywrightContext.VueDocumentTitle);

        GlobalPlaywrightContext.Window.SetTitle(GlobalPlaywrightContext.PhotinoWindowTitle);
    }
}
