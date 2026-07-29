// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniAutomationTests.Tests;
using InfiniTests;
using Microsoft.Playwright;

namespace InfiniAutomationTests.WebApp.Tests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class SharedWebServerStartupTests : InfiniFramePlaywrightTestBase {
    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task Run_ShouldStartKestrelAndNavigateWebViewToRoot() {
        IPage page = await GetRootPageAsync();

        var uri = new Uri(page.Url);
        string bodyText = await page.Locator("body").InnerTextAsync();

        await Assert.That(uri.Scheme).IsEqualTo(Uri.UriSchemeHttp);
        await Assert.That(uri.IsLoopback).IsTrue();
        await Assert.That(bodyText).IsNotEmpty();
    }
}
