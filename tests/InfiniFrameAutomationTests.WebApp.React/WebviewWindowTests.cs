// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrameAutomationTests.WebApp.React.TestUtility;

namespace InfiniFrameAutomationTests.WebApp.React;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[InheritsTests]
public sealed class WebviewWindowTests : SharedWebviewWindowTests {
    protected override IAutomationRuntimeContext RuntimeContext => ReactAutomationRuntimeContext.Instance;
}

