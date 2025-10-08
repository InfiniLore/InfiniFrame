// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Playwright;
using Tests.Photino.Playwright.Utility;
using Tests.Shared.Photino;

namespace Tests.Photino.Playwright;
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
}
