// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {beforeEach, describe, it, expect, vi} from "vitest";
import {InfiniFrame} from "./InfiniFrame";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
describe("InfiniFrame", () => {

    beforeEach(() => {
        const win = window as any;
        if (!win.infiniframe?.messaging) {
            win.infiniframe = {
                messaging: {
                    sendMessageToHost: vi.fn(),
                    getMessageFromHostAsync: vi.fn(),
                    assignMessageReceivedHandler: vi.fn(),
                    unregisterMessageReceivedHandler: vi.fn()
                },
                utils: {
                    setPointerCapture: vi.fn(),
                    releasePointerCapture: vi.fn()
                }
            };
        }
    });

    it("initializes HostMessaging and Utils", () => {
        const instance = new InfiniFrame();

        expect(instance.messaging).toBeDefined();
        expect(instance.utils).toBeDefined();
    });

    it("creates independent instances", () => {
        const a = new InfiniFrame();
        const b = new InfiniFrame();

        expect(a.messaging).not.toBe(b.messaging);
        expect(a.utils).not.toBe(b.utils);
    });

    it("replaces an incomplete preinitialized window API", () => {
        const instance = new InfiniFrame({window: {} as any});

        expect(instance.window.features).toBeDefined();
        expect(instance.window.features.decorations).toBeDefined();
    });

    it("preserves existing window when features are already set", () => {
        const existingWindow = {features: {decorations: {}}};
        const instance = new InfiniFrame({window: existingWindow as any});

        expect(instance.window).toBe(existingWindow);
    });

    it("does not define a legacy window.__infiniframe host", async () => {
        const setSpy = vi.spyOn(Object, "defineProperty");

        const win = window as any;

        // ensure clean state
        delete win.infiniframe;
        delete win.__infiniframe;

        await import("./InfiniFrame");

        // library should not define host via Object.defineProperty
        const hostDefinitionCalls = setSpy.mock.calls.filter(call =>
            String(call[1])?.includes?.("host")
        );

        expect(hostDefinitionCalls.length).toBe(0);
        expect(win.__infiniframe).toBeUndefined();

        setSpy.mockRestore();
    });
});
