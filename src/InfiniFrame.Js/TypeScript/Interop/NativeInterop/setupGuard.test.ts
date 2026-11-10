// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import { beforeEach, describe, expect, it } from "vitest";
import { deleteTestProperty, getTestWindow } from "../../_testHelpers";
import { getSetupGuard } from "./setupGuard";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
describe("getSetupGuard", () => {
	beforeEach(() => {
		deleteTestProperty(getTestWindow(), "infiniframe");
	});

	it("should initialize window.infiniframe if missing", () => {
		// Arrange

		// Act
		const guard = getSetupGuard();

		// Assert
		expect(window.infiniframe).toBeDefined();
		expect(guard).toBeDefined();
	});

	it("should initialize setup object with all flags false", () => {
		// Arrange

		// Act
		const guard = getSetupGuard();

		// Assert
		expect(guard.nativeInteropBridgeInitialized).toBe(false);
		expect(guard.windowExternalBridgeInitialized).toBe(false);
		expect(guard.blazorModulesFetchPatchInitialized).toBe(false);
		expect(guard.blazorCustomElementsPatchInitialized).toBe(false);
		expect(guard.customElementsInitialized).toBe(false);
	});

	it("should return same reference on subsequent calls", () => {
		// Arrange

		// Act
		const guard1 = getSetupGuard();
		const guard2 = getSetupGuard();

		// Assert
		expect(guard1).toBe(guard2);
	});

	it("should preserve existing setup values", () => {
		// Arrange
		getTestWindow().infiniframe = {
			setup: { nativeInteropBridgeInitialized: true },
		};

		// Act
		const guard = getSetupGuard();

		// Assert
		expect(guard.nativeInteropBridgeInitialized).toBe(true);
	});

	it("should preserve existing window.infiniframe properties", () => {
		// Arrange
		const existing = { custom: "value" };
		getTestWindow().infiniframe = existing;

		// Act
		getSetupGuard();

		// Assert
		expect(getTestWindow().infiniframe.custom).toBe("value");
	});
});
