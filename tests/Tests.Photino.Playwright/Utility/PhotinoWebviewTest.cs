// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Playwright;
using TUnit.Playwright;

namespace Tests.Photino.Playwright.Utility;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class PhotinoWebviewTest : PageTest {
    public override string BrowserName => "webkit";

    protected async Task<IPage> GetPageAsync(string relativeUrl) {
        var url = new Uri(GlobalPlaywrightContext.PlaywrightConnectionUri, relativeUrl);
        
        IBrowser browser = await Playwright.Chromium.ConnectOverCDPAsync(url.ToString());
        IBrowserContext context = browser.Contexts[0];
        IPage page = context.Pages[0];
        
        return page;
    }
    
    protected async Task<IPage> GetRootPageAsync() {
        return await GetPageAsync("/");
    }
}
