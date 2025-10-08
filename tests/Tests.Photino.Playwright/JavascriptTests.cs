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
public class JavascriptTests : PhotinoWebviewTest {
    
    [Test]
    [NotInParallel(ParallelControl.Playwright)]
    public async Task InfiniWindowIsInitialized() {
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
    [NotInParallel(ParallelControl.Playwright)]
    public async Task DynamicallyUpdateTitleFromJS() {
        // Arrange
        IPage page = await GetRootPageAsync();
        string originalTitle = GlobalPlaywrightContext.Window.Title;
        const string newTitle = "newly updated title";
        
        // Act
        await page.EvaluateAsync(
            // lang=javascript 
            $"() => window.infiniWindow.sendMessageToHost('__infiniWindow:title:change', '{newTitle}')"
        );
        string updatedTitle = await WaitForStateChangeAsync(originalTitle, () => GlobalPlaywrightContext.Window.Title);

        // Assert
        await Assert.That(updatedTitle).IsEqualTo(newTitle);   
        
        // Reset
        GlobalPlaywrightContext.Window.SetTitle(GlobalPlaywrightContext.PhotinoWindowTitle);
    }
}
