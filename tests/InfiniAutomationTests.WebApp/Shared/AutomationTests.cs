// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniAutomationTests.Tests;
using InfiniAutomationTests.WebApp.Tests;

namespace InfiniAutomationTests.WebApp;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[InheritsTests]
public sealed class DataExchangeTests : SharedDataExchangeTests {
    protected override IPlaywrightRuntimeContext RuntimeContext => PlaywrightContext.Instance;
}

[InheritsTests]
public sealed class JavascriptTests : SharedJavascriptTests {
    protected override IPlaywrightRuntimeContext RuntimeContext => PlaywrightContext.Instance;
}

[InheritsTests]
public sealed class JavascriptInteropTests : SharedJavascriptInteropTests {
    protected override IPlaywrightRuntimeContext RuntimeContext => PlaywrightContext.Instance;
    protected override string FullscreenToggleButtonSelector => "#fullscreen-toggle-button";
    protected override string TitleToggleButtonSelector => "#title-toggle-button";
}

[InheritsTests]
public sealed class WebServerStartupTests : SharedWebServerStartupTests {
    protected override IPlaywrightRuntimeContext RuntimeContext => PlaywrightContext.Instance;
}

[InheritsTests]
public sealed class WebviewWindowTests : SharedWebviewWindowTests {
    protected override IPlaywrightRuntimeContext RuntimeContext => PlaywrightContext.Instance;
}

[InheritsTests]
public sealed class WindowFeatureMirroringTests : SharedWindowFeatureMirroringTests {
    protected override IPlaywrightRuntimeContext RuntimeContext => PlaywrightContext.Instance;
}

[InheritsTests]
public sealed class WindowChromeTests : SharedWindowChromeTests {
    protected override IPlaywrightRuntimeContext RuntimeContext => PlaywrightContext.Instance;
}