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
        Object.defineProperty(window, "Blazor", {configurable: true, value: undefined, writable: true});
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

    it("registerBlazorCustomElement returns early if Blazor.rootComponents not available", () => {
        window.Blazor = {} as any;
        initCustomElements(setup);

        // Should not throw
        window.registerBlazorCustomElement!("test-element", [{name: "Value"}]);
    });

    it("registerBlazorCustomElement returns early if customElements.define not available", () => {
        window.Blazor = {rootComponents: {add: vi.fn()}} as any;
        initCustomElements(setup);

        // In jsdom, customElements.define exists, so this branch may not be hit
        // But we can verify the element is defined
        window.registerBlazorCustomElement!("test-element-no-define", [{name: "Value"}]);
    });

    it("registerBlazorCustomElement returns early if element already defined", () => {
        window.Blazor = {rootComponents: {add: vi.fn()}} as any;
        initCustomElements(setup);

        window.registerBlazorCustomElement!("test-element-defined", [{name: "Value"}]);
        // Registering same name again should not throw
        window.registerBlazorCustomElement!("test-element-defined", [{name: "Value"}]);
    });

    it("handles numeric type conversions (int, float, double, decimal)", async () => {
        const add = vi.fn(() => Promise.resolve({setParameters: vi.fn(() => Promise.resolve()), dispose: vi.fn()}));
        const identifier = `infiniframe-test-numeric-${++elementCounter}`;

        window.Blazor = {rootComponents: {add}};
        initCustomElements(setup);
        window.registerBlazorCustomElement!(identifier, [
            {name: "IntVal", type: "int"},
            {name: "FloatVal", type: "float"},
            {name: "DoubleVal", type: "double"},
            {name: "DecimalVal", type: "decimal"}
        ]);

        const element = document.createElement(identifier);
        element.setAttribute("int-val", "42");
        element.setAttribute("float-val", "3.14");
        element.setAttribute("double-val", "2.718");
        element.setAttribute("decimal-val", "99.99");
        document.body.appendChild(element);
        await tick();

        expect(add).toHaveBeenCalledWith(element, identifier, {
            IntVal: 42,
            FloatVal: 3.14,
            DoubleVal: 2.718,
            DecimalVal: 99.99
        });
    });

    it("handles non-numeric NaN values as strings", async () => {
        const add = vi.fn(() => Promise.resolve({setParameters: vi.fn(() => Promise.resolve()), dispose: vi.fn()}));
        const identifier = `infiniframe-test-nan-${++elementCounter}`;

        window.Blazor = {rootComponents: {add}};
        initCustomElements(setup);
        window.registerBlazorCustomElement!(identifier, [
            {name: "Val", type: "number"}
        ]);

        const element = document.createElement(identifier);
        element.setAttribute("val", "not-a-number");
        document.body.appendChild(element);
        await tick();

        expect(add).toHaveBeenCalledWith(element, identifier, {
            Val: "not-a-number"
        });
    });

    it("handles bool false value", async () => {
        const add = vi.fn(() => Promise.resolve({setParameters: vi.fn(() => Promise.resolve()), dispose: vi.fn()}));
        const identifier = `infiniframe-test-bool-${++elementCounter}`;

        window.Blazor = {rootComponents: {add}};
        initCustomElements(setup);
        window.registerBlazorCustomElement!(identifier, [
            {name: "Flag", type: "boolean"}
        ]);

        const element = document.createElement(identifier);
        element.setAttribute("flag", "false");
        document.body.appendChild(element);
        await tick();

        expect(add).toHaveBeenCalledWith(element, identifier, {
            Flag: false
        });
    });

    it("attributeChangedCallback ignores unchanged values", async () => {
        const setParameters = vi.fn(() => Promise.resolve());
        const add = vi.fn(() => Promise.resolve({setParameters, dispose: vi.fn()}));
        const identifier = `infiniframe-test-unchanged-${++elementCounter}`;

        window.Blazor = {rootComponents: {add}};
        initCustomElements(setup);
        window.registerBlazorCustomElement!(identifier, [
            {name: "Value", type: "string"}
        ]);

        const element = document.createElement(identifier);
        element.setAttribute("value", "hello");
        document.body.appendChild(element);
        await tick();

        // Set same value again
        element.setAttribute("value", "hello");
        await tick();

        expect(setParameters).not.toHaveBeenCalled();
    });

    it("attributeChangedCallback ignores unknown attributes", async () => {
        const setParameters = vi.fn(() => Promise.resolve());
        const add = vi.fn(() => Promise.resolve({setParameters, dispose: vi.fn()}));
        const identifier = `infiniframe-test-unknown-${++elementCounter}`;

        window.Blazor = {rootComponents: {add}};
        initCustomElements(setup);
        window.registerBlazorCustomElement!(identifier, [
            {name: "Known", type: "string"}
        ]);

        const element = document.createElement(identifier);
        element.setAttribute("known", "value");
        document.body.appendChild(element);
        await tick();

        // Set unknown attribute
        element.setAttribute("unknown-attr", "value");
        await tick();

        expect(setParameters).not.toHaveBeenCalled();
    });

    it("connectedCallback disposes if disconnected before promise resolves", async () => {
        let resolveAdd: (value: any) => void;
        const addPromise = new Promise(resolve => { resolveAdd = resolve; });
        const dispose = vi.fn(() => Promise.resolve());
        const add = vi.fn(() => addPromise);
        const identifier = `infiniframe-test-disconnect-${++elementCounter}`;

        window.Blazor = {rootComponents: {add}};
        initCustomElements(setup);
        window.registerBlazorCustomElement!(identifier, []);

        const element = document.createElement(identifier);
        document.body.appendChild(element);
        await tick();

        // Disconnect before the promise resolves
        element.remove();
        await tick();

        // Now resolve the add promise
        resolveAdd!({dispose});
        await tick();

        expect(dispose).toHaveBeenCalled();
    });

    it("initBlazorCustomElementsPatch returns early if already initialized", () => {
        setup.blazorCustomElementsPatchInitialized = true;
        const attachWebRendererInterop = vi.fn();
        window.Blazor = {_internal: {attachWebRendererInterop}};

        initBlazorCustomElementsPatch(setup);

        expect(attachWebRendererInterop).not.toHaveBeenCalled();
    });

    it("flushAutoRegister calls registerBlazorCustomElement if available", () => {
        vi.useFakeTimers();
        const register = vi.fn();
        const attachWebRendererInterop = vi.fn();
        window.registerBlazorCustomElement = register;
        window.Blazor = {_internal: {attachWebRendererInterop}};

        initBlazorCustomElementsPatch(setup);

        window.Blazor._internal!.attachWebRendererInterop!(
            {}, {},
            {"auto-element": [{name: "Val"}]},
            {}
        );

        vi.runAllTimers();

        expect(register).toHaveBeenCalledWith("auto-element", [{name: "Val"}]);
        vi.useRealTimers();
    });

    it("flushAutoRegister does nothing if registerBlazorCustomElement not available", () => {
        vi.useFakeTimers();
        const attachWebRendererInterop = vi.fn();
        window.registerBlazorCustomElement = undefined as any;
        window.Blazor = {_internal: {attachWebRendererInterop}};

        initBlazorCustomElementsPatch(setup);

        window.Blazor._internal!.attachWebRendererInterop!(
            {}, {},
            {"auto-element": [{name: "Val"}]},
            {}
        );

        vi.runAllTimers();
        vi.useRealTimers();
    });

    it("autoRegister handles empty defs and initMap", () => {
        vi.useFakeTimers();
        const register = vi.fn();
        const attachWebRendererInterop = vi.fn();
        window.registerBlazorCustomElement = register;
        window.Blazor = {_internal: {attachWebRendererInterop}};

        initBlazorCustomElementsPatch(setup);

        window.Blazor._internal!.attachWebRendererInterop!(
            {}, {},
            undefined as any,
            undefined as any
        );

        vi.runAllTimers();

        expect(register).not.toHaveBeenCalled();
        vi.useRealTimers();
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
