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
    public async Task Title_ShouldBeExpectedValue(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();

        string title = await page.TitleAsync();

        await Assert.That(title).IsEqualTo(RuntimeContext.DefaultDocumentTitle);
    }
}
