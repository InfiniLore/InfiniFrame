// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using TUnit.Playwright;
using Microsoft.Playwright;
using Tests.Shared.Photino;

namespace Tests.Photino.Playwright;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SampleTest : PageTest {
    public override string BrowserName { get; } = "webkit";
    
    [Test]
    [NotInParallel(ParallelControl.Playwright)]
    public async Task Test1() {
        // Arrange
        IBrowser browser = await Playwright.Chromium.ConnectOverCDPAsync(GlobalPlaywright.PlaywrightConnectionString);
        IBrowserContext context = browser.Contexts[0];
        IPage page = context.Pages[0];
        
        // Act
        string title = await page.TitleAsync();

        // Assert
        await Assert.That(title).IsEqualTo("Photino Playwright Test");
    }
    
    [Test]
    [NotInParallel(ParallelControl.Playwright)]
    public async Task Test2() {
        // Arrange
        IBrowser browser = await Playwright.Chromium.ConnectOverCDPAsync(GlobalPlaywright.PlaywrightConnectionString);
        IBrowserContext context = browser.Contexts[0];
        IPage page = context.Pages[0];
        
        // Act
        string title = await page.TitleAsync();

        // Assert
        await Assert.That(title).IsEqualTo("Photino Playwright Test");
    }
}
