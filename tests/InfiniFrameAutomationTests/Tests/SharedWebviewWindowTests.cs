// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using InfiniFrameTests.Shared;
using JetBrains.Annotations;

namespace InfiniFrameAutomationTests.Tests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class SharedWebviewWindowTests : InfiniFrameAutomationTestBase {
    [Test]
    [NotInParallel(ParallelControl.Playwright)]
    [UsedImplicitly]
    public async Task Title_ShouldBeExpectedValue() {
        IAutomationPage page = await GetRootPageAsync();

        string title = await page.TitleAsync();

        await Assert.That(title).IsEqualTo(RuntimeContext.DefaultDocumentTitle);
    }
}


