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
        
        // Act
        await page.ClickAsync("#fullscreen-toggle-button");
        bool newFullscreenState = await WaitForStateChangeAsync(() => GlobalPlaywrightContext.Window.FullScreen, originalFullscreenState) ;

        // Assert
        await Assert.That(originalFullscreenState).IsFalse();
        await Assert.That(newFullscreenState).IsTrue();
        
        // Cleanup
        await page.ClickAsync("#fullscreen-toggle-button");
        await WaitForStateChangeAsync(() => GlobalPlaywrightContext.Window.FullScreen, newFullscreenState) ;
    }
}
