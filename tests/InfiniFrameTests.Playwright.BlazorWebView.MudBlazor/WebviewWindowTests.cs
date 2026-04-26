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
public sealed class WebviewWindowTests : SharedWebviewWindowTests {
    protected override IPlaywrightRuntimeContext RuntimeContext => PlaywrightContext.Instance;
}
