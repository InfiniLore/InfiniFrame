// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniTests;
using Microsoft.Playwright;

namespace InfiniAutomationTests.Tests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class SharedDataExchangeTests : InfiniFramePlaywrightTestBase {
    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task InputProbe_ShouldSendEnteredTitleToWindowLayer(CancellationToken ct = default) {
        const string expectedTitle = "Title sent from the UI layer";
        IPage page = await GetRootPageAsync();

        await page.FillAsync("#title-data-input", expectedTitle);
        await page.ClickAsync("#title-toggle-button");

        string? actualTitle = await WaitForStateChangeAsync(
            RuntimeContext.DefaultDocumentTitle,
            stateProvider: () => RuntimeContext.Window.Features.Decorations.Title
        );
        string? renderedTitle = await WaitForStateChangeAsync(
            RuntimeContext.DefaultDocumentTitle,
            stateProvider: () => page.Locator("#current-window-title").TextContentAsync()
        );

        await Assert.That(actualTitle).IsEqualTo(expectedTitle);
        await Assert.That(renderedTitle).IsEqualTo(expectedTitle);
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task OutputProbe_ShouldWriteWindowDataIntoItsInput(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();

        await page.ClickAsync("#probe-browser-feature");
        string serializedData = await WaitForStateChangeAsync(
            string.Empty,
            stateProvider: () => page.Locator("#browser-feature-result").InputValueAsync()
        );

        await Assert.That(serializedData).Contains("\"contextMenu\"");
        await Assert.That(serializedData).Contains("\"userAgent\"");
    }
}
