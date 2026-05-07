// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {beforeEach, describe, expect, it, vi} from "vitest";
import {BlazorCallback, InfiniFrameExternal, InfiniFrameSetup} from "../../Contracts";
import {initWindowExternalBridge} from "./blazorExternalBridge";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
describe("blazorExternalBridge", () => {
    let setup: InfiniFrameSetup;

    beforeEach(() => {
        setup = createSetup();
        delete window.infiniframe;
        delete window.__blazorCallbacks;
        delete window.__blazorDispatchHooked;
        vi.restoreAllMocks();
    });

    it("routes Blazor outbound messages through the InfiniFrame host bridge", () => {
        const postData = vi.fn();
        window.infiniframe = {
            host: {
                postData,
                receiveCallback: vi.fn()
            },
            messaging: undefined!,
            window: undefined!,
            utils: undefined!
        };

        initWindowExternalBridge(setup);

        const external = window.external as InfiniFrameExternal;
        external.sendMessage!("message");
        external.postMessage!("post-message");

        expect(postData).toHaveBeenCalledWith("message");
        expect(postData).toHaveBeenCalledWith("post-message");
    });

    it("creates window.external when the runtime does not provide one", () => {
        Object.defineProperty(window, "external", {
            configurable: true,
            value: undefined,
            writable: true
        });

        window.infiniframe = {
            host: {
                postData: vi.fn(),
                receiveCallback: vi.fn()
            },
            messaging: undefined!,
            window: undefined!,
            utils: undefined!
        };

        initWindowExternalBridge(setup);

        const external = window.external as InfiniFrameExternal;
        expect(external).toBeDefined();
        expect(external.receiveMessage).toBeTypeOf("function");
        expect(external.sendMessage).toBeTypeOf("function");
    });

    it("dispatches host messages to registered Blazor callbacks", () => {
        let hostCallback: BlazorCallback | null = null;
        window.infiniframe = {
            host: {
                postData: vi.fn(),
                receiveCallback: vi.fn(callback => {
                    hostCallback = callback;
                })
            },
            messaging: undefined!,
            window: undefined!,
            utils: undefined!
        };

        initWindowExternalBridge(setup);

        const callback = vi.fn();
        const external = window.external as InfiniFrameExternal;
        external.receiveMessage!(callback);
        hostCallback!("host-message");

        expect(callback).toHaveBeenCalledWith("host-message");
    });

    it("attaches the host receive callback only once", () => {
        const receiveCallback = vi.fn();
        window.infiniframe = {
            host: {
                postData: vi.fn(),
                receiveCallback
            },
            messaging: undefined!,
            window: undefined!,
            utils: undefined!
        };

        initWindowExternalBridge(setup);
        initWindowExternalBridge(setup);

        expect(receiveCallback).toHaveBeenCalledTimes(1);
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
