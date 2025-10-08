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
public class SampleTest : PhotinoWebviewTest {
    [Test]
    [NotInParallel(ParallelControl.Playwright)]
    public async Task Test1() {
        // Arrange
        IPage page = await GetRootPageAsync();
        
        // Act
        string title = await page.TitleAsync();

        // Assert
        await Assert.That(title).IsEqualTo("Photino Playwright Test");
    }
    
    [Test]
    [NotInParallel(ParallelControl.Playwright)]
    public async Task Test2() {
        // Arrange
        IPage page = await GetRootPageAsync();
        
        // Act
        string title = await page.TitleAsync();

        // Assert
        await Assert.That(title).IsEqualTo("Photino Playwright Test");
    }
}
