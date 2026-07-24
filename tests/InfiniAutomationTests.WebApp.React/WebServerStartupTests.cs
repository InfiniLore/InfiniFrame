// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniAutomationTests.Tests;
using InfiniAutomationTests.WebApp.React.TestUtility;

namespace InfiniAutomationTests.WebApp.React;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[InheritsTests]
public sealed class WebServerStartupTests : SharedWebServerStartupTests {
    protected override IPlaywrightRuntimeContext RuntimeContext => PlaywrightContext.Instance;
}
