// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {beforeEach, describe, expect, it, vi} from "vitest";
import {deleteTestProperty} from "./_testHelpers";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
describe("Index", () => {
    beforeEach(() => {
        deleteTestProperty(window, "__blazorCallbacks");
        deleteTestProperty(window, "__blazorDispatchHooked");
        deleteTestProperty(window, "infiniframe");
        deleteTestProperty(window, "chrome");
        vi.restoreAllMocks();
        vi.resetModules();
    });

    it("does not recreate the InfiniFrame API or resend ready when the bundle is loaded twice", async () => {
        const postMessage = vi.fn();
        window.chrome = {
            webview: {
                postMessage,
                addEventListener: vi.fn(),
            },
        };

        await import("./Index");
        const firstInfiniFrame = window.infiniframe;

        vi.resetModules();
        await import("./Index");

        expect(window.infiniframe).toBe(firstInfiniFrame);
        expect(postMessage).toHaveBeenCalledTimes(1);
    });
});
