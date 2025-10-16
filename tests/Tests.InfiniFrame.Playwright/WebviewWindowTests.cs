// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Playwright;
using Tests.InfiniFrame.Playwright.Utility;
using Tests.Shared;

namespace Tests.InfiniFrame.Playwright;
using Tests.InfiniFrame.Playwright.Utility;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WebviewWindowTests : InfiniFrameWebviewTest {
    
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
}
