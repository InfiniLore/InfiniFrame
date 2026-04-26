// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrameTests.Playwright.BlazorWebView.MudBlazor.TestUtility;
using InfiniFrameTests.Playwright.Tests;

namespace InfiniFrameTests.Playwright.BlazorWebView.MudBlazor;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[InheritsTests]
// ReSharper disable once UnusedType.Global
public sealed class JavascriptInteropTests : SharedJavascriptInteropTests {
    protected override IPlaywrightRuntimeContext RuntimeContext => PlaywrightContext.Instance;

    protected override string FullscreenToggleButtonSelector => "#fullscreen-toggle-button";

    protected override string TitleToggleButtonSelector => "#title-toggle-button";
}
