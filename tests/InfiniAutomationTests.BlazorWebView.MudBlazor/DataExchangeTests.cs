// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniAutomationTests.BlazorWebView.MudBlazor.TestUtility;
using InfiniAutomationTests.Tests;
using InfiniTests;
using Microsoft.Playwright;

namespace InfiniAutomationTests.BlazorWebView.MudBlazor;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// ReSharper disable once UnusedType.Global
public sealed class DataExchangeTests : InfiniFramePlaywrightTestBase {
    protected override IPlaywrightRuntimeContext RuntimeContext => PlaywrightContext.Instance;

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task ConsumeProbe_ShouldSendEnteredTitleToWindowLayer(CancellationToken ct = default) {
        const string expectedTitle = "Title sent from MudBlazor";
        IPage page = await GetRootPageAsync();

        await page.FillAsync("#title-data-input", expectedTitle);
        await page.ClickAsync("#title-toggle-button");

        string? actualTitle = await WaitForStateChangeAsync(
            WindowTestState.Default.Title,
            stateProvider: () => RuntimeContext.Window.Features.Decorations.Title
        );

        await Assert.That(actualTitle).IsEqualTo(expectedTitle);

        string? renderedTitle = await WaitForStateChangeAsync(
            WindowTestState.Default.Title,
            stateProvider: () => page.Locator("#current-window-title").TextContentAsync()
        );
        await Assert.That(renderedTitle).IsEqualTo(expectedTitle);
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task ProduceProbe_ShouldWriteWindowDataIntoMudTextField(CancellationToken ct = default) {
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
