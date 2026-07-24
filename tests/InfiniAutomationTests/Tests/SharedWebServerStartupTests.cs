// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniTests;
using Microsoft.Playwright;

namespace InfiniAutomationTests.Tests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class SharedWebServerStartupTests : InfiniFramePlaywrightTestBase {
    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task Run_ShouldStartKestrelAndNavigateWebViewToRoot() {
        // Reaching the document through the WebView's CDP page proves that the native message
        // loop navigated to the HTTP server started by InfiniFrameWebApplication.Run().
        IPage page = await GetRootPageAsync();

        var uri = new Uri(page.Url);
        string bodyText = await page.Locator("body").InnerTextAsync();

        await Assert.That(uri.Scheme).IsEqualTo(Uri.UriSchemeHttp);
        await Assert.That(uri.IsLoopback).IsTrue();
        await Assert.That(bodyText).IsNotEmpty();
    }
}
