// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {beforeEach, describe, expect, it, vi} from "vitest";
import type {InfiniFrameSetup} from "../../Contracts";
import {initBlazorCustomElementsPatch, initCustomElements} from "./customElements";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
function tick(): Promise<void> {
    return Promise.resolve();
}

describe("customElements", () => {
    let elementCounter = 0;
    let setup: InfiniFrameSetup;

    beforeEach(() => {
        setup = createSetup();
        document.body.innerHTML = "";
        delete window.Blazor;
        delete window.registerBlazorCustomElement;
        vi.useRealTimers();
        vi.restoreAllMocks();
    });

    it("registers Blazor custom elements and converts attributes to parameters", async () => {
        const setParameters = vi.fn(() => Promise.resolve());
        const dispose = vi.fn(() => Promise.resolve());
        const add = vi.fn(() => Promise.resolve({setParameters, dispose}));
        const identifier = `infiniframe-test-element-${++elementCounter}`;

        window.Blazor = {
            rootComponents: {
                add
            }
        };

        initCustomElements(setup);
        window.registerBlazorCustomElement!(identifier, [
            {name: "TitleText", type: "string"},
            {name: "Count", type: "int"},
            {name: "Enabled", type: "bool"},
            {name: "Changed", type: "EventCallback"}
        ]);

        const element = document.createElement(identifier);
        element.setAttribute("title-text", "Hello");
        element.setAttribute("count", "4");
        element.setAttribute("enabled", "");
        document.body.appendChild(element);
        await tick();

        expect(add).toHaveBeenCalledWith(element, identifier, {
            TitleText: "Hello",
            Count: 4,
            Enabled: true
        });

        element.setAttribute("count", "5");
        await tick();
        element.setAttribute("enabled", "false");
        await tick();

        expect(setParameters).toHaveBeenCalledWith({Count: 5});
        expect(setParameters).toHaveBeenCalledWith({Enabled: false});

        element.remove();
        await tick();

        expect(dispose).toHaveBeenCalledTimes(1);
    });

    it("auto-registers custom elements missing from the Blazor initializer map", () => {
        vi.useFakeTimers();

        const registerBlazorCustomElement = vi.fn();
        const attachWebRendererInterop = vi.fn(() => "attached");
        window.registerBlazorCustomElement = registerBlazorCustomElement;
        window.Blazor = {
            _internal: {
                attachWebRendererInterop
            }
        };

        initBlazorCustomElementsPatch(setup);

        const result = window.Blazor._internal!.attachWebRendererInterop!(
            {},
            {},
            {
                "initialized-element": [{name: "Value"}],
                "missing-element": [{name: "OtherValue"}]
            },
            {
                initializer: ["initialized-element"]
            }
        );

        vi.runAllTimers();

        expect(result).toBe("attached");
        expect(registerBlazorCustomElement).toHaveBeenCalledTimes(1);
        expect(registerBlazorCustomElement).toHaveBeenCalledWith(
            "missing-element",
            [{name: "OtherValue"}]
        );
    });
});

function createSetup(): InfiniFrameSetup {
    return {
        nativeInteropBridgeInitialized: false,
        windowExternalBridgeInitialized: false,
        blazorModulesFetchPatchInitialized: false,
        blazorCustomElementsPatchInitialized: false,
        customElementsInitialized: false
    };
}
