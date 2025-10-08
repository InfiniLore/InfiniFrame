// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Playwright;
using Tests.Shared.Photino;

namespace Tests.Photino.Playwright;
using Tests.Photino.Playwright.Utility;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WebviewWindowTests : PhotinoWebviewTest {
    [Test]
    [NotInParallel(ParallelControl.Playwright)]
    public async Task Title_ShouldBeExpectedValue() {
        // Arrange
        IPage page = await GetRootPageAsync();
        
        // Act
        string title = await page.TitleAsync();

        // Assert
        await Assert.That(title).IsEqualTo("Photino Playwright Vue");
    }
    
    [Test]
    [NotInParallel(ParallelControl.Playwright)]
    public async Task FullscreenButton_ShouldToggleFullscreen() {
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
