// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {describe, it, expect, vi} from "vitest";
import {InfiniFrameUtils} from "./InfiniFrameUtils";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
describe("InfiniFrameUtils", () => {
    it("forwards setPointerCapture to element", () => {
        const utils = new InfiniFrameUtils();

        const setPointerCapture = vi.fn();
        const hasPointerCapture = vi.fn(() => false);
        
        const element = {
            setPointerCapture,
            hasPointerCapture
        } as unknown as Element;

        utils.setPointerCapture(element, 10);

        expect(setPointerCapture).toHaveBeenCalledWith(10);
    });

    it("forwards releasePointerCapture to element", () => {
        const utils = new InfiniFrameUtils();

        const releasePointerCapture = vi.fn();
        const hasPointerCapture = vi.fn(() => true);

        const element = {
            releasePointerCapture,
            hasPointerCapture
        } as unknown as Element;

        utils.releasePointerCapture(element, 10);

        expect(hasPointerCapture).toHaveBeenCalledWith(10);
        expect(releasePointerCapture).toHaveBeenCalledWith(10);
    });
});