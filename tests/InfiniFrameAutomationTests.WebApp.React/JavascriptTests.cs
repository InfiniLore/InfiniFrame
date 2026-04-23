// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrameAutomationTests.Tests;
using InfiniFrameAutomationTests.WebApp.React.TestUtility;

namespace InfiniFrameAutomationTests.WebApp.React;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[InheritsTests]
public sealed class JavascriptTests : SharedJavascriptTests {
    protected override IAutomationRuntimeContext RuntimeContext => ReactAutomationRuntimeContext.Instance;
}

