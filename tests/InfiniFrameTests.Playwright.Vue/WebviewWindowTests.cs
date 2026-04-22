// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrameTests.Playwright.Vue.TestUtility;
using Microsoft.Playwright;
using InfiniFrameTests.Shared;

namespace InfiniFrameTests.Playwright.Vue;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WebviewWindowTests : InfiniFrameWebviewTest {

    [Test, NotInParallel(ParallelControl.Playwright)]
    public async Task Title_ShouldBeExpectedValue() {
        // Arrange
        IPage page = await GetRootPageAsync();

        // Act
        string title = await page.TitleAsync();

        // Assert
        await Assert.That(title).IsEqualTo("InfiniFrame Playwright Vue");
    }
}
