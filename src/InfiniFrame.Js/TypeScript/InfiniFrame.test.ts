// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {describe, it, expect, vi} from "vitest";
import {InfiniFrame} from "./InfiniFrame";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
describe("InfiniFrame", () => {

    it("initializes HostMessaging and Utils", () => {
        const instance = new InfiniFrame();

        expect(instance.hostMessaging).toBeDefined();
        expect(instance.utils).toBeDefined();
    });

    it("creates independent instances", () => {
        const a = new InfiniFrame();
        const b = new InfiniFrame();

        expect(a.hostMessaging).not.toBe(b.hostMessaging);
        expect(a.utils).not.toBe(b.utils);
    });

    it("does not define or mutate window.infiniframe.host", async () => {
        const setSpy = vi.spyOn(Object, "defineProperty");

        const win = window as any;

        // ensure clean state
        delete win.infiniframe;

        await import("./InfiniFrame");

        // library should not define host via Object.defineProperty
        const hostDefinitionCalls = setSpy.mock.calls.filter(call =>
            String(call[1])?.includes?.("host")
        );

        expect(hostDefinitionCalls.length).toBe(0);

        setSpy.mockRestore();
    });
});