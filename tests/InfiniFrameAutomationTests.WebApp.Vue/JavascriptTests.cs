// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrameAutomationTests.Tests;
using InfiniFrameAutomationTests.WebApp.Vue.TestUtility;

namespace InfiniFrameAutomationTests.WebApp.Vue;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[InheritsTests]
public sealed class JavascriptTests : SharedJavascriptTests {
    protected override IAutomationRuntimeContext RuntimeContext => VueAutomationRuntimeContext.Instance;
}

