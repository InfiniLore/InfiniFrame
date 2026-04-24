// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrameTests.Playwright.BlazorWebView.TestUtility;
using InfiniFrameTests.Playwright.Tests;
using InfiniFrameTests.Shared;
using Microsoft.Playwright;

namespace InfiniFrameTests.Playwright.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// ReSharper disable once UnusedType.Global
public sealed class CustomElementsTests : InfiniFramePlaywrightTestBase {
    protected override IPlaywrightRuntimeContext RuntimeContext => PlaywrightContext.Instance;

    [Test]
    [NotInParallel(ParallelControl.Playwright)]
    public async Task CustomElement_Registers_Renders_AndUpdatesFromAttributes() {
        IPage page = await GetRootPageAsync();

        bool isCustomElementDefined = await page.EvaluateAsync<bool>(
            // lang=javascript
            "() => window.customElements.get('infiniframe-custom-element') !== undefined"
        );
        if (!isCustomElementDefined) {
            isCustomElementDefined = await WaitForStateChangeAsync(
                false,
                stateProvider: () => page.EvaluateAsync<bool>(
                    // lang=javascript
                    "() => window.customElements.get('infiniframe-custom-element') !== undefined"
                )
            );
        }

        await Assert.That(isCustomElementDefined).IsTrue();

        await page.EvaluateAsync(
            // lang=javascript
            """
            () => {
                const existing = document.getElementById("custom-element-test-host");
                if (existing) existing.remove();

                const host = document.createElement("infiniframe-custom-element");
                host.id = "custom-element-test-host";
                host.setAttribute("label", "alpha");
                document.body.appendChild(host);
            }
            """
        );

        string? renderedAlpha = await WaitForStateChangeAsync(
            (string?)null,
            stateProvider: () => page.EvaluateAsync<string?>(
                // lang=javascript
                "() => document.querySelector('#custom-element-test-host .custom-element-probe-value')?.textContent?.trim() ?? null"
            )
        );

        await Assert.That(renderedAlpha).IsEqualTo("alpha");

        await page.EvaluateAsync(
            // lang=javascript
            "() => document.getElementById('custom-element-test-host')?.setAttribute('label', 'beta')"
        );

        string? renderedBeta = await WaitForStateChangeAsync(
            renderedAlpha,
            stateProvider: () => page.EvaluateAsync<string?>(
                // lang=javascript
                "() => document.querySelector('#custom-element-test-host .custom-element-probe-value')?.textContent?.trim() ?? null"
            )
        );

        await Assert.That(renderedBeta).IsEqualTo("beta");
    }
}
