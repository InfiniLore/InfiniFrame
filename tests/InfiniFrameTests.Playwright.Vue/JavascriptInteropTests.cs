// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrameTests.Playwright.Vue.TestUtility;

namespace InfiniFrameTests.Playwright.Vue;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[InheritsTests]
public sealed class JavascriptInteropTests : SharedJavascriptInteropTests {
    protected override IPlaywrightRuntimeContext RuntimeContext => VuePlaywrightRuntimeContext.Instance;

    protected override string FullscreenToggleButtonSelector => "#fullscreen-toggle-button";

    protected override string TitleToggleButtonSelector => "#title-toggle-button";
}