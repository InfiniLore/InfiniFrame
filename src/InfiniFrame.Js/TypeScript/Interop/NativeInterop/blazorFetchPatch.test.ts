// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {beforeEach, describe, expect, it, vi} from "vitest";
import type {InfiniFrameSetup} from "../../Contracts";
import {initBlazorModulesFetchPatch} from "./blazorFetchPatch";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
describe("blazorFetchPatch", () => {
    const originalFetch = window.fetch;
    let setup: InfiniFrameSetup;

    beforeEach(() => {
        setup = createSetup();
        window.fetch = originalFetch;
        vi.restoreAllMocks();
    });

    it("returns an empty Blazor modules manifest without calling the original fetch", async () => {
        const fetch = vi.fn(() => Promise.resolve(new Response("original")));
        window.fetch = fetch;

        initBlazorModulesFetchPatch(setup);

        const response = await window.fetch("https://localhost/_framework/blazor.modules.json");

        expect(fetch).not.toHaveBeenCalled();
        expect(response.status).toBe(200);
        expect(await response.text()).toBe("[]");
    });

    it("delegates non-Blazor module URLs to the original fetch", async () => {
        const fetch = vi.fn(() => Promise.resolve(new Response("original")));
        window.fetch = fetch;

        initBlazorModulesFetchPatch(setup);

        const response = await window.fetch("https://localhost/app.js");

        expect(fetch).toHaveBeenCalledWith("https://localhost/app.js", undefined);
        expect(await response.text()).toBe("original");
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
