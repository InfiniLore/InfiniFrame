// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrameTests.Playwright.WebApp.Vue.TestUtility;

namespace InfiniFrameTests.Playwright.WebApp.Vue;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[InheritsTests]
public sealed class WebviewWindowTests : SharedWebviewWindowTests {
    protected override IPlaywrightRuntimeContext RuntimeContext => VuePlaywrightRuntimeContext.Instance;
}