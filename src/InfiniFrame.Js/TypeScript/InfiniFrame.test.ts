// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {beforeEach, describe, expect, it, vi} from "vitest";
import {SendToHostMessageIds} from "./Contracts/IInfiniFrameHostMessaging";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
describe("InfiniFrame", () => {
    beforeEach(() => {
        vi.resetModules();
    });

    it("forwards pointer capture APIs to element", async () => {
        const {InfiniFrame} = await import("./InfiniFrame");
        const instance = new InfiniFrame();

        const setPointerCapture = vi.fn();
        const releasePointerCapture = vi.fn();
        const element = {
            setPointerCapture,
            releasePointerCapture
        } as unknown as Element;

        instance.setPointerCapture(element, 10);
        instance.releasePointerCapture(element, 10);

        expect(setPointerCapture).toHaveBeenCalledWith(10);
        expect(releasePointerCapture).toHaveBeenCalledWith(10);
    });
});
