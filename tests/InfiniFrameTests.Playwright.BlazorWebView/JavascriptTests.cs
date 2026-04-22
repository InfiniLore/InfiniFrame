// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrameTests.Playwright.BlazorWebView.TestUtility;

namespace InfiniFrameTests.Playwright.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[InheritsTests]
public sealed class JavascriptTests : SharedJavascriptTests {
    protected override IPlaywrightRuntimeContext RuntimeContext => BlazorWebViewPlaywrightRuntimeContext.Instance;
}
