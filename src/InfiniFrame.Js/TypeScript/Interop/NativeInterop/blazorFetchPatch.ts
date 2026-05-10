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
                            headers: { "Content-Type": "application/json" },
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
