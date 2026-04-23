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
public sealed class JavascriptInteropTests : SharedJavascriptInteropTests {
    protected override IAutomationRuntimeContext RuntimeContext => VueAutomationRuntimeContext.Instance;

    protected override string FullscreenToggleButtonSelector => "#fullscreen-toggle-button";

    protected override string TitleToggleButtonSelector => "#title-toggle-button";
}

