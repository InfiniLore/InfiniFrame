// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniAutomationTests.BlazorWebView.MudBlazor.TestUtility;
using InfiniAutomationTests.Tests;

namespace InfiniAutomationTests.BlazorWebView.MudBlazor;
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
