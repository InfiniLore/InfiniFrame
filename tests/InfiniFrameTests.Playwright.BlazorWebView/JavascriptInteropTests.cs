// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrameTests.Playwright.BlazorWebView.TestUtility;

namespace InfiniFrameTests.Playwright.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[InheritsTests]
// ReSharper disable once UnusedType.Global
public sealed class JavascriptInteropTests : SharedJavascriptInteropTests {
    protected override IPlaywrightRuntimeContext RuntimeContext => BlazorWebViewPlaywrightRuntimeContext.Instance;

    protected override string FullscreenToggleButtonSelector => "#fullscreen-toggle-button";

    protected override string TitleToggleButtonSelector => "#title-toggle-button";
}
