// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {describe, expect, it, vi} from "vitest";
import {InfiniFrameUtils} from "./InfiniFrameUtils";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
describe("InfiniFrameUtils", () => {
    describe("setPointerCapture", () => {
        it("forwards to element when not already captured", () => {
            const utils = new InfiniFrameUtils();
            const setPointerCapture = vi.fn();
            const hasPointerCapture = vi.fn(() => false);
            const element = {setPointerCapture, hasPointerCapture} as unknown as Element;

            utils.setPointerCapture(element, 10);

            expect(setPointerCapture).toHaveBeenCalledWith(10);
        });

        it("skips when element is null", () => {
            const utils = new InfiniFrameUtils();
            utils.setPointerCapture(null as any, 10);
        });

        it("skips when pointerId is null", () => {
            const utils = new InfiniFrameUtils();
            const element = {setPointerCapture: vi.fn(), hasPointerCapture: vi.fn()} as unknown as Element;
            utils.setPointerCapture(element, null as any);
            expect(element.setPointerCapture).not.toHaveBeenCalled();
        });

        it("skips when already captured", () => {
            const utils = new InfiniFrameUtils();
            const setPointerCapture = vi.fn();
            const hasPointerCapture = vi.fn(() => true);
            const element = {setPointerCapture, hasPointerCapture} as unknown as Element;

            utils.setPointerCapture(element, 10);

            expect(setPointerCapture).not.toHaveBeenCalled();
        });
    });

    describe("releasePointerCapture", () => {
        it("forwards to element when captured", () => {
            const utils = new InfiniFrameUtils();
            const releasePointerCapture = vi.fn();
            const hasPointerCapture = vi.fn(() => true);
            const element = {releasePointerCapture, hasPointerCapture} as unknown as Element;

            utils.releasePointerCapture(element, 10);

            expect(hasPointerCapture).toHaveBeenCalledWith(10);
            expect(releasePointerCapture).toHaveBeenCalledWith(10);
        });

        it("skips when element is null", () => {
            const utils = new InfiniFrameUtils();
            utils.releasePointerCapture(null as any, 10);
        });

        it("skips when pointerId is null", () => {
            const utils = new InfiniFrameUtils();
            const element = {releasePointerCapture: vi.fn(), hasPointerCapture: vi.fn()} as unknown as Element;
            utils.releasePointerCapture(element, null as any);
            expect(element.releasePointerCapture).not.toHaveBeenCalled();
        });

        it("skips when not captured", () => {
            const utils = new InfiniFrameUtils();
            const releasePointerCapture = vi.fn();
            const hasPointerCapture = vi.fn(() => false);
            const element = {releasePointerCapture, hasPointerCapture} as unknown as Element;

            utils.releasePointerCapture(element, 10);

            expect(releasePointerCapture).not.toHaveBeenCalled();
        });
    });
});
