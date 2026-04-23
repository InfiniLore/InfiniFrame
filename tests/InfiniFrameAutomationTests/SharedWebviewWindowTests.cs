// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrameTests.Shared;

namespace InfiniFrameAutomationTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class SharedWebviewWindowTests : InfiniFrameAutomationTestBase {
    [Test]
    [NotInParallel(ParallelControl.Playwright)]
    public async Task Title_ShouldBeExpectedValue() {
        IAutomationPage page = await GetRootPageAsync();

        string title = await page.TitleAsync();

        await Assert.That(title).IsEqualTo(RuntimeContext.DefaultDocumentTitle);
    }
}


