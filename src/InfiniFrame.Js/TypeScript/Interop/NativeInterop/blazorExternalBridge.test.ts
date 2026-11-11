// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import { beforeEach, describe, expect, it, vi } from "vitest";
import {
	deleteTestProperty,
	getTestWindow,
	requireValue,
} from "../../_testHelpers";
import type {
	BlazorCallback,
	InfiniFrameExternal,
	InfiniFrameSetup,
} from "../../Contracts";
import { initWindowExternalBridge } from "./blazorExternalBridge";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
const unavailable = <T>(): T => undefined as unknown as T;

describe("blazorExternalBridge", () => {
	let setup: InfiniFrameSetup;

	beforeEach(() => {
		setup = createSetup();
		deleteTestProperty(getTestWindow(), "infiniframe");
		deleteTestProperty(getTestWindow(), "__blazorCallbacks");
		deleteTestProperty(getTestWindow(), "__blazorDispatchHooked");
		vi.restoreAllMocks();
	});

	it("routes Blazor outbound messages through the InfiniFrame host bridge", () => {
		const postData = vi.fn();
		getTestWindow().infiniframe = {
			host: {
				postData,
				receiveCallback: vi.fn(),
			},
			messaging: unavailable(),
			window: unavailable(),
			utils: unavailable(),
		};

		initWindowExternalBridge(setup);

		const external = window.external as InfiniFrameExternal;
		requireValue(external.sendMessage)("message");
		requireValue(external.postMessage)("post-message");

		expect(postData).toHaveBeenCalledWith("message");
		expect(postData).toHaveBeenCalledWith("post-message");
	});

	it("creates window.external when the runtime does not provide one", () => {
		Object.defineProperty(window, "external", {
			configurable: true,
			value: undefined,
			writable: true,
		});

		window.infiniframe = {
			host: {
				postData: vi.fn(),
				receiveCallback: vi.fn(),
			},
			messaging: unavailable(),
			window: unavailable(),
			utils: unavailable(),
		};

		initWindowExternalBridge(setup);

		const external = window.external as InfiniFrameExternal;
		expect(external).toBeDefined();
		expect(external.receiveMessage).toBeTypeOf("function");
		expect(external.sendMessage).toBeTypeOf("function");
	});

	it("dispatches host messages to registered Blazor callbacks", () => {
		let hostCallback: BlazorCallback | null = null;
		window.infiniframe = {
			host: {
				postData: vi.fn(),
				receiveCallback: vi.fn((callback) => {
					hostCallback = callback;
				}),
			},
			messaging: unavailable(),
			window: unavailable(),
			utils: unavailable(),
		};

		initWindowExternalBridge(setup);

		const callback = vi.fn();
		const external = window.external as InfiniFrameExternal;
		requireValue(external.receiveMessage)(callback);
		requireValue<BlazorCallback>(hostCallback)("host-message");

		expect(callback).toHaveBeenCalledWith("host-message");
	});

	it("attaches the host receive callback only once", () => {
		const receiveCallback = vi.fn();
		window.infiniframe = {
			host: {
				postData: vi.fn(),
				receiveCallback,
			},
			messaging: unavailable(),
			window: unavailable(),
			utils: unavailable(),
		};

		initWindowExternalBridge(setup);
		initWindowExternalBridge(setup);

		expect(receiveCallback).toHaveBeenCalledTimes(1);
	});

	it("warns when host bridge postData is not available", () => {
		const warnSpy = vi.spyOn(console, "warn").mockImplementation(() => {});
		getTestWindow().infiniframe = {
			host: {
				postData: undefined,
				receiveCallback: vi.fn(),
			},
			messaging: unavailable(),
			window: unavailable(),
			utils: unavailable(),
		};

		initWindowExternalBridge(setup);

		const external = window.external as InfiniFrameExternal;
		requireValue(external.sendMessage)("test");

		expect(warnSpy).toHaveBeenCalled();
		warnSpy.mockRestore();
	});

	it("warns when host bridge is not available", () => {
		const warnSpy = vi.spyOn(console, "warn").mockImplementation(() => {});
		window.infiniframe = {
			host: undefined,
			messaging: unavailable(),
			window: unavailable(),
			utils: unavailable(),
		};

		initWindowExternalBridge(setup);

		const external = window.external as InfiniFrameExternal;
		requireValue(external.sendMessage)("test");

		expect(warnSpy).toHaveBeenCalled();
		warnSpy.mockRestore();
	});

	it("tolerates throwing Blazor callbacks", () => {
		let hostCallback: BlazorCallback | null = null;
		window.infiniframe = {
			host: {
				postData: vi.fn(),
				receiveCallback: vi.fn((callback) => {
					hostCallback = callback;
				}),
			},
			messaging: unavailable(),
			window: unavailable(),
			utils: unavailable(),
		};

		initWindowExternalBridge(setup);

		const throwingCallback = vi.fn(() => {
			throw new Error("callback error");
		});
		const normalCallback = vi.fn();
		const external = window.external as InfiniFrameExternal;
		requireValue(external.receiveMessage)(throwingCallback);
		requireValue(external.receiveMessage)(normalCallback);

		// Should not throw even though first callback throws
		requireValue<BlazorCallback>(hostCallback)("host-message");

		expect(throwingCallback).toHaveBeenCalled();
		expect(normalCallback).toHaveBeenCalledWith("host-message");
	});

	it("uses existing window.external when available", () => {
		const existingExternal = { sendMessage: vi.fn() };
		Object.defineProperty(window, "external", {
			configurable: true,
			value: existingExternal,
			writable: true,
		});

		window.infiniframe = {
			host: {
				postData: vi.fn(),
				receiveCallback: vi.fn(),
			},
			messaging: unavailable(),
			window: unavailable(),
			utils: unavailable(),
		};

		initWindowExternalBridge(setup);

		expect(window.external).toBe(existingExternal);
	});

	it("does nothing if already initialized", () => {
		setup.windowExternalBridgeInitialized = true;
		const receiveCallback = vi.fn();
		window.infiniframe = {
			host: {
				postData: vi.fn(),
				receiveCallback,
			},
			messaging: unavailable(),
			window: unavailable(),
			utils: unavailable(),
		};

		initWindowExternalBridge(setup);

		expect(receiveCallback).not.toHaveBeenCalled();
	});
});

function createSetup(): InfiniFrameSetup {
	return {
		nativeInteropBridgeInitialized: false,
		windowExternalBridgeInitialized: false,
		blazorModulesFetchPatchInitialized: false,
		blazorCustomElementsPatchInitialized: false,
		customElementsInitialized: false,
	};
}
