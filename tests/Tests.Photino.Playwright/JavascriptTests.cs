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
    [SkipUtility.OnMacOs]
    [SkipUtility.OnLinux]
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
}
