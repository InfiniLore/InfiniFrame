// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrameAutomationTests.BlazorWebView.TestUtility;

namespace InfiniFrameAutomationTests.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[InheritsTests]
// ReSharper disable once UnusedType.Global
public sealed class WebviewWindowTests : SharedWebviewWindowTests {
    protected override IAutomationRuntimeContext RuntimeContext => BlazorWebViewAutomationRuntimeContext.Instance;
}


