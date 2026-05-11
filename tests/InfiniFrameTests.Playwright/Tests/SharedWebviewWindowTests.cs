// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrameTests.Shared;
using Microsoft.Playwright;

namespace InfiniFrameTests.Playwright.Tests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class SharedWebviewWindowTests : InfiniFramePlaywrightTestBase {
    [Test]
    [NotInParallel(ParallelControl.Playwright)]
    public async Task Title_ShouldBeExpectedValue(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();

        string title = await page.TitleAsync();

        await Assert.That(title).IsEqualTo(RuntimeContext.DefaultDocumentTitle);
    }
}
