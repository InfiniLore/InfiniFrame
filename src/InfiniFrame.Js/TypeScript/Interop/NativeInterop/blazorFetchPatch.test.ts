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

    it("handles http://localhost blazor.modules.json", async () => {
        const fetch = vi.fn(() => Promise.resolve(new Response("original")));
        window.fetch = fetch;

        initBlazorModulesFetchPatch(setup);

        const response = await window.fetch("http://localhost/_framework/blazor.modules.json");

        expect(fetch).not.toHaveBeenCalled();
        expect(response.status).toBe(200);
    });

    it("handles app://localhost blazor.modules.json", async () => {
        const fetch = vi.fn(() => Promise.resolve(new Response("original")));
        window.fetch = fetch;

        initBlazorModulesFetchPatch(setup);

        const response = await window.fetch("app://localhost/_framework/blazor.modules.json");

        expect(fetch).not.toHaveBeenCalled();
        expect(response.status).toBe(200);
    });

    it("handles trailing slash variants", async () => {
        const fetch = vi.fn(() => Promise.resolve(new Response("original")));
        window.fetch = fetch;

        initBlazorModulesFetchPatch(setup);

        const response = await window.fetch("https://localhost/_framework/blazor.modules.json/");

        expect(fetch).not.toHaveBeenCalled();
        expect(response.status).toBe(200);
    });

    it("handles Request object input", async () => {
        const fetch = vi.fn(() => Promise.resolve(new Response("original")));
        window.fetch = fetch;

        initBlazorModulesFetchPatch(setup);

        const request = new Request("https://localhost/_framework/blazor.modules.json");
        const response = await window.fetch(request);

        expect(fetch).not.toHaveBeenCalled();
        expect(response.status).toBe(200);
    });

    it("handles URL object input", async () => {
        const fetch = vi.fn(() => Promise.resolve(new Response("original")));
        window.fetch = fetch;

        initBlazorModulesFetchPatch(setup);

        const url = new URL("https://localhost/_framework/blazor.modules.json");
        const response = await window.fetch(url);

        expect(fetch).not.toHaveBeenCalled();
        expect(response.status).toBe(200);
    });

    it("passes init options to original fetch", async () => {
        const fetch = vi.fn(() => Promise.resolve(new Response("original")));
        window.fetch = fetch;

        initBlazorModulesFetchPatch(setup);

        await window.fetch("https://localhost/api/data", {method: "POST"});

        expect(fetch).toHaveBeenCalledWith("https://localhost/api/data", {method: "POST"});
    });

    it("does nothing if already initialized", () => {
        setup.blazorModulesFetchPatchInitialized = true;
        const fetch = vi.fn(() => Promise.resolve(new Response("original")));
        window.fetch = fetch;

        initBlazorModulesFetchPatch(setup);

        expect(window.fetch).toBe(fetch);
    });

    it("falls through on invalid URL", async () => {
        const fetch = vi.fn(() => Promise.resolve(new Response("original")));
        window.fetch = fetch;

        initBlazorModulesFetchPatch(setup);

        // An invalid relative URL that will cause new URL() to throw
        const response = await window.fetch("");

        expect(fetch).toHaveBeenCalled();
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
