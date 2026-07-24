// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniAutomationTests.Tests;
using InfiniAutomationTests.WebApp.Vue.TestUtility;

namespace InfiniAutomationTests.WebApp.Vue;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[InheritsTests]
public sealed class WebServerStartupTests : SharedWebServerStartupTests {
    protected override IPlaywrightRuntimeContext RuntimeContext => PlaywrightContext.Instance;
}
