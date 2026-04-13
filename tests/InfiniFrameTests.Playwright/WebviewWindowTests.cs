// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrameTests.Playwright.TestUtility;
using Microsoft.Playwright;
using InfiniFrameTests.Shared;
using TUnit.Core.Executors;

namespace InfiniFrameTests.Playwright;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WebviewWindowTests : InfiniFrameWebviewTest {

    [Test, NotInParallel(ParallelControl.Playwright)]
    [TestExecutor<UiThreadExecutor>]
    public async Task Title_ShouldBeExpectedValue() {
        // Arrange
        IPage page = await GetRootPageAsync();

        // Act
        string title = await page.TitleAsync();

        // Assert
        await Assert.That(title).IsEqualTo("InfiniFrame Playwright Vue");
    }
}
