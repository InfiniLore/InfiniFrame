// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrameAutomationTests.BlazorWebView.TestUtility;
using InfiniFrameAutomationTests.Tests;

namespace InfiniFrameAutomationTests.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[InheritsTests]
// ReSharper disable once UnusedType.Global
public sealed class JavascriptInteropTests : SharedJavascriptInteropTests {
    protected override IAutomationRuntimeContext RuntimeContext => BlazorWebViewAutomationRuntimeContext.Instance;

    protected override string FullscreenToggleButtonSelector => "#fullscreen-toggle-button";

    protected override string TitleToggleButtonSelector => "#title-toggle-button";
}


