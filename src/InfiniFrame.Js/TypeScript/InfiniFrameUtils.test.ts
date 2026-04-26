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
        const element = {
            setPointerCapture
        } as unknown as Element;

        utils.setPointerCapture(element, 10);

        expect(setPointerCapture).toHaveBeenCalledWith(10);
    });

    it("forwards releasePointerCapture to element", () => {
        const utils = new InfiniFrameUtils();

        const releasePointerCapture = vi.fn();
        const element = {
            releasePointerCapture
        } as unknown as Element;

        utils.releasePointerCapture(element, 10);

        expect(releasePointerCapture).toHaveBeenCalledWith(10);
    });
});