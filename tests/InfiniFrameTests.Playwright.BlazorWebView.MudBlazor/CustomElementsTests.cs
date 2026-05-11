// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrameTests.Playwright.BlazorWebView.MudBlazor.TestUtility;
using InfiniFrameTests.Playwright.Tests;
using InfiniFrameTests.Shared;
using Microsoft.Playwright;

namespace InfiniFrameTests.Playwright.BlazorWebView.MudBlazor;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// ReSharper disable once UnusedType.Global
public sealed class CustomElementsTests : InfiniFramePlaywrightTestBase {
    protected override IPlaywrightRuntimeContext RuntimeContext => PlaywrightContext.Instance;

    [Test]
    [NotInParallel(ParallelControl.Playwright)]
    public async Task CustomElement_Registers_Renders_AndUpdatesFromAttributes(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();

        bool isCustomElementDefined = await EvaluateWhenPageReadyAsync<bool>(
            page,
            // lang=javascript
            "() => window.customElements.get('infiniframe-custom-element') !== undefined"
        );
        if (!isCustomElementDefined) {
            isCustomElementDefined = await WaitForStateChangeAsync(
                false,
                stateProvider: () => EvaluateWhenPageReadyAsync<bool>(
                    page,
                    // lang=javascript
                    "() => window.customElements.get('infiniframe-custom-element') !== undefined"
                )
            );
        }

        await Assert.That(isCustomElementDefined).IsTrue();

        await EvaluateWhenPageReadyAsync(
            page,
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

        string? renderedAlpha = await WaitForStateChangeAsync<string?>(
            null,
            stateProvider: () => EvaluateWhenPageReadyAsync<string?>(
                page,
                // lang=javascript
                "() => document.querySelector('#custom-element-test-host .custom-element-probe-value')?.textContent?.trim() ?? null"
            )
        );

        await Assert.That(renderedAlpha).IsEqualTo("alpha");

        await EvaluateWhenPageReadyAsync(
            page,
            // lang=javascript
            "() => document.getElementById('custom-element-test-host')?.setAttribute('label', 'beta')"
        );

        string? renderedBeta = await WaitForStateChangeAsync(
            renderedAlpha,
            stateProvider: () => EvaluateWhenPageReadyAsync<string?>(
                page,
                // lang=javascript
                "() => document.querySelector('#custom-element-test-host .custom-element-probe-value')?.textContent?.trim() ?? null"
            )
        );

        await Assert.That(renderedBeta).IsEqualTo("beta");
    }

    [Test]
    [NotInParallel(ParallelControl.Playwright)]
    public async Task JsComponent_WithoutInitializer_AutoRegisters_AsCustomElement_ByDefault(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();

        bool isCustomElementDefined = await EvaluateWhenPageReadyAsync<bool>(
            page,
            // lang=javascript
            "() => window.customElements.get('infiniframe-no-init-component') !== undefined"
        );
        if (!isCustomElementDefined) {
            isCustomElementDefined = await WaitForStateChangeAsync(
                false,
                stateProvider: () => EvaluateWhenPageReadyAsync<bool>(
                    page,
                    // lang=javascript
                    "() => window.customElements.get('infiniframe-no-init-component') !== undefined"
                )
            );
        }

        await Assert.That(isCustomElementDefined).IsTrue();

        await EvaluateWhenPageReadyAsync(
            page,
            // lang=javascript
            """
            () => {
                const existingHost = document.getElementById("no-init-component-host");
                if (existingHost) existingHost.remove();

                const host = document.createElement("infiniframe-no-init-component");
                host.id = "no-init-component-host";
                host.setAttribute("label", "gamma");
                document.body.appendChild(host);
            }
            """
        );

        string? renderedGamma = await WaitForStateChangeAsync<string?>(
            null,
            stateProvider: () => EvaluateWhenPageReadyAsync<string?>(
                page,
                // lang=javascript
                "() => document.querySelector('#no-init-component-host .custom-element-probe-value')?.textContent?.trim() ?? null"
            )
        );

        await Assert.That(renderedGamma).IsEqualTo("gamma");

        await EvaluateWhenPageReadyAsync(
            page,
            // lang=javascript
            "() => document.getElementById('no-init-component-host')?.setAttribute('label', 'delta')"
        );

        string? renderedDelta = await WaitForStateChangeAsync(
            renderedGamma,
            stateProvider: () => EvaluateWhenPageReadyAsync<string?>(
                page,
                // lang=javascript
                "() => document.querySelector('#no-init-component-host .custom-element-probe-value')?.textContent?.trim() ?? null"
            )
        );

        await Assert.That(renderedDelta).IsEqualTo("delta");
    }
}
