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
    public async Task DynamicallyUpdateTitleFromJs() {
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
    
    [Test]
    [Skip("`window.close()` although supported by the library cannot be tested without killing the entire test process.")]
    [NotInParallel(ParallelControl.Playwright)]
    public async Task WindowClose() {
        // Arrange
        IPage page = await GetRootPageAsync();
        
        // Act
       await page.EvaluateAsync(
            // lang=javascript 
            "() => window.close()"
        ); 

        // Assert
        // can't really be asserted in the current context of the PhotinoWindow as it is already closed.
    }
}
