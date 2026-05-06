// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {beforeEach, describe, expect, it, vi} from "vitest";
import {initBlazorModulesFetchPatch} from "./blazorFetchPatch";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
describe("blazorFetchPatch", () => {
    const originalFetch = window.fetch;

    beforeEach(() => {
        window.fetch = originalFetch;
        vi.restoreAllMocks();
    });

    it("returns an empty Blazor modules manifest without calling the original fetch", async () => {
        const fetch = vi.fn(() => Promise.resolve(new Response("original")));
        window.fetch = fetch;

        initBlazorModulesFetchPatch();

        const response = await window.fetch("http://localhost/_framework/blazor.modules.json");

        expect(fetch).not.toHaveBeenCalled();
        expect(response.status).toBe(200);
        expect(await response.text()).toBe("[]");
    });

    it("delegates non-Blazor module URLs to the original fetch", async () => {
        const fetch = vi.fn(() => Promise.resolve(new Response("original")));
        window.fetch = fetch;

        initBlazorModulesFetchPatch();

        const response = await window.fetch("http://localhost/app.js");

        expect(fetch).toHaveBeenCalledWith("http://localhost/app.js", undefined);
        expect(await response.text()).toBe("original");
    });
});
