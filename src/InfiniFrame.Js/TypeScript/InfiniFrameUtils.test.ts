// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import { describe, expect, it, vi } from "vitest";
import { InfiniFrameUtils } from "./InfiniFrameUtils";

function createElementMock(
	methods: Partial<Pick<Element, "setPointerCapture" | "releasePointerCapture" | "hasPointerCapture">>,
): Element {
	const element = document.createElement("div");
	Object.assign(element, methods);
	return element;
}

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
describe("InfiniFrameUtils", () => {
	describe("setPointerCapture", () => {
		it("forwards to element when not already captured", () => {
			const utils = new InfiniFrameUtils();
			const setPointerCapture = vi.fn();
			const hasPointerCapture = vi.fn(() => false);
			const element = createElementMock({
				setPointerCapture,
				hasPointerCapture,
			});

			utils.setPointerCapture(element, 10);

			expect(setPointerCapture).toHaveBeenCalledWith(10);
		});

		it("skips when element is null", () => {
			const utils = new InfiniFrameUtils();
			utils.setPointerCapture(null, 10);
		});

		it("skips when pointerId is null", () => {
			const utils = new InfiniFrameUtils();
			const element = createElementMock({
				setPointerCapture: vi.fn(),
				hasPointerCapture: vi.fn(),
			});
			utils.setPointerCapture(element, null);
			expect(element.setPointerCapture).not.toHaveBeenCalled();
		});

		it("skips when already captured", () => {
			const utils = new InfiniFrameUtils();
			const setPointerCapture = vi.fn();
			const hasPointerCapture = vi.fn(() => true);
			const element = createElementMock({
				setPointerCapture,
				hasPointerCapture,
			});

			utils.setPointerCapture(element, 10);

			expect(setPointerCapture).not.toHaveBeenCalled();
		});
	});

	describe("releasePointerCapture", () => {
		it("forwards to element when captured", () => {
			const utils = new InfiniFrameUtils();
			const releasePointerCapture = vi.fn();
			const hasPointerCapture = vi.fn(() => true);
			const element = createElementMock({
				releasePointerCapture,
				hasPointerCapture,
			});

			utils.releasePointerCapture(element, 10);

			expect(hasPointerCapture).toHaveBeenCalledWith(10);
			expect(releasePointerCapture).toHaveBeenCalledWith(10);
		});

		it("skips when element is null", () => {
			const utils = new InfiniFrameUtils();
			utils.releasePointerCapture(null, 10);
		});

		it("skips when pointerId is null", () => {
			const utils = new InfiniFrameUtils();
			const element = createElementMock({
				releasePointerCapture: vi.fn(),
				hasPointerCapture: vi.fn(),
			});
			utils.releasePointerCapture(element, null);
			expect(element.releasePointerCapture).not.toHaveBeenCalled();
		});

		it("skips when not captured", () => {
			const utils = new InfiniFrameUtils();
			const releasePointerCapture = vi.fn();
			const hasPointerCapture = vi.fn(() => false);
			const element = createElementMock({
				releasePointerCapture,
				hasPointerCapture,
			});

			utils.releasePointerCapture(element, 10);

			expect(releasePointerCapture).not.toHaveBeenCalled();
		});
	});
});
