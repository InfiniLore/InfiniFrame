// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {beforeEach, describe, expect, it, vi} from "vitest";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
describe("Index", () => {
    beforeEach(() => {
        delete window.__blazorCallbacks;
        delete window.__blazorDispatchHooked;
        // @ts-ignore
        delete window.infiniframe;
        delete window.chrome;
        vi.restoreAllMocks();
        vi.resetModules();
    });

    it("does not recreate the InfiniFrame API or resend ready when the bundle is loaded twice", async () => {
        const postMessage = vi.fn();
        window.chrome = {
            webview: {
                postMessage,
                addEventListener: vi.fn()
            }
        };

        await import("./Index");
        const firstInfiniFrame = window.infiniframe;

        vi.resetModules();
        await import("./Index");

        expect(window.infiniframe).toBe(firstInfiniFrame);
        expect(postMessage).toHaveBeenCalledTimes(1);
    });
});
