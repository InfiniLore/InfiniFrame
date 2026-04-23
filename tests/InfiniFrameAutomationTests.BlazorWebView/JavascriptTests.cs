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
public sealed class JavascriptTests : SharedJavascriptTests {
    protected override IAutomationRuntimeContext RuntimeContext => BlazorWebViewAutomationRuntimeContext.Instance;
}


