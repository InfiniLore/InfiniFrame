// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Photino.NET;
using Tests.Shared.Photino;
using TUnit.Playwright;
using InfiniLore.Photino.NET.Server;
using Microsoft.Playwright;

namespace Tests.Photino.Playwright;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SampleTest : PageTest {
    public override string BrowserName { get; } = "webkit";

    [Test]
    [STAThread]
    public async Task Test1() {
        // Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", "--remote-debugging-port=9222");
        
        // Arrange
        using var utility = WindowServerTestUtility.Create(
            serverBuilder => serverBuilder
                .UsePort(9000),
            
            windowBuilder => windowBuilder
                .SetBrowserControlInitParameters("--remote-debugging-port=9222")
        );
        
        IBrowser browser = await Playwright.Chromium.ConnectOverCDPAsync("http://127.0.0.1:9222");
        IBrowserContext context = browser.Contexts[0];
        IPage page = context.Pages[0];

        var title = await page.TitleAsync();
        await Assert.That(title).IsEqualTo("Photino Playwright Test");
        
        
        // IResponse? r = await Page.GotoAsync("http://localhost:9222/json");
        // string content = await (r?.TextAsync() ?? Task.FromResult(string.Empty));
        // await Assert.That(content).IsNotEmpty();
        //
        // IResponse? response = await Page.GotoAsync("http://localhost:9000/");
        // string title = await Page.TitleAsync();
        //
        // // Assert
        // await Assert.That(response).IsNotNull();        
        // await Assert.That(response?.Status).IsEqualTo(200);
        //
        
        
        // windowServer.Dispose();
    }
}
