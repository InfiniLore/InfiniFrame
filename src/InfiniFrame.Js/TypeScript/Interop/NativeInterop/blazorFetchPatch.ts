/**
 * @file Blazor fetch patch. Intercepts fetch() calls to route Blazor framework requests through the native host.
 */
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {InfiniFrameSetup} from "../../Contracts";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
const BLAZOR_MODULES_URLS = new Set([
    "http://localhost/_framework/blazor.modules.json",
    "http://localhost/_framework/blazor.modules.json/",
    "https://localhost/_framework/blazor.modules.json",
    "https://localhost/_framework/blazor.modules.json/",
    "app://localhost/_framework/blazor.modules.json",
    "app://localhost/_framework/blazor.modules.json/",
]);

/**
 * Patches the global `fetch` function so that requests for `blazor.modules.json` (used by the Blazor module loader)
 * are short-circuited with an empty JSON array response, preventing the browser from attempting a real network fetch.
 *
 * @param setup - The setup guard that tracks which initializations have already been performed.
 * @returns {void}
 */
export function initBlazorModulesFetchPatch(setup: InfiniFrameSetup): void {
    if (setup.blazorModulesFetchPatchInitialized) return;
    setup.blazorModulesFetchPatchInitialized = true;

    const originalFetch = window.fetch;

    window.fetch = function (input: RequestInfo | URL, init?: RequestInit): Promise<Response> {
        try {
            const requestUrl =
                typeof input === "string"
                    ? input
                    : input instanceof URL
                        ? input.href
                        : (input as Request).url ?? "";

            if (requestUrl) {
                const absoluteUrl = new URL(requestUrl, window.location.href).href;

                if (BLAZOR_MODULES_URLS.has(absoluteUrl)) {
                    return Promise.resolve(
                        new Response("[]", {
                            status: 200,
                            statusText: "OK",
                            headers: {"Content-Type": "application/json"},
                        })
                    );
                }
            }
        } catch {
            // fall through to original fetch
        }

        return originalFetch.call(this, input, init);
    };
}
