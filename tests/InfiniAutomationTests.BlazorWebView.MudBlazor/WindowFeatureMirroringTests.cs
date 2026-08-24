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
public sealed class WindowFeatureMirroringTests : SharedWindowFeatureMirroringTests {
    protected override IPlaywrightRuntimeContext RuntimeContext => PlaywrightContext.Instance;
}
