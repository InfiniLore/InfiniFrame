// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniTests;
using Microsoft.Playwright;

namespace InfiniAutomationTests.Tests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class SharedWebviewWindowTests : InfiniFramePlaywrightTestBase {
    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task Run_ShouldStartKestrelAndNavigateWebViewToRoot() {
        // This is the end-to-end regression for the WebServer Run() startup path. Reaching the
        // document through the WebView's CDP page proves that its native loop navigated to Kestrel.
        IPage page = await GetRootPageAsync();

        var uri = new Uri(page.Url);
        string bodyText = await page.Locator("body").InnerTextAsync();

        await Assert.That(uri.Scheme).IsEqualTo(Uri.UriSchemeHttp);
        await Assert.That(uri.IsLoopback).IsTrue();
        await Assert.That(bodyText).IsNotEmpty();
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task Title_ShouldBeExpectedValue(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();

        string title = await page.TitleAsync();

        await Assert.That(title).IsEqualTo(RuntimeContext.DefaultDocumentTitle);
    }
}
