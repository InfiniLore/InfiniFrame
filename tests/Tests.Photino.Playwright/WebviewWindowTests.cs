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
    [SkipUtility.OnMacOs]
    [SkipUtility.OnLinux]
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
    [SkipUtility.OnMacOs]
    [SkipUtility.OnLinux]
    [NotInParallel(ParallelControl.Playwright)]
    public async Task Js_InfiniWindowIsInitialized() {
        // Arrange
        IPage page = await GetRootPageAsync();
        
        // Act
        bool isInitialized = await page.EvaluateAsync<bool>(
            // lang=javascript 
            "() => window.infiniWindow !== undefined && window.infiniWindow !== null"
        ); 

        // Assert
        await Assert.That(isInitialized).IsTrue();   
    }

    [Test]
    [SkipUtility.OnMacOs]
    [SkipUtility.OnLinux]
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
