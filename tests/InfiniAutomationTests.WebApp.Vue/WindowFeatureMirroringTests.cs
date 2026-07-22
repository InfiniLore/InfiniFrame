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
public sealed class WindowFeatureMirroringTests : SharedWindowFeatureMirroringTests {
    protected override IPlaywrightRuntimeContext RuntimeContext => PlaywrightContext.Instance;
}
