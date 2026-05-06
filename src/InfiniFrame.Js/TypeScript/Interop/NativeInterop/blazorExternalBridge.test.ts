// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {beforeEach, describe, expect, it, vi} from "vitest";
import {BlazorCallback, InfiniFrameExternal} from "../../Contracts";
import {initWindowExternalBridge} from "./blazorExternalBridge";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
describe("blazorExternalBridge", () => {
    beforeEach(() => {
        delete window.__infiniframe;
        delete window.__blazorCallbacks;
        delete window.__blazorDispatchHooked;
        vi.restoreAllMocks();
    });

    it("routes Blazor outbound messages through the InfiniFrame host bridge", () => {
        const postData = vi.fn();
        window.__infiniframe = {
            host: {
                postData,
                receiveCallback: vi.fn()
            }
        };

        initWindowExternalBridge();

        const external = window.external as InfiniFrameExternal;
        external.sendMessage!("message");
        external.postMessage!("post-message");

        expect(postData).toHaveBeenCalledWith("message");
        expect(postData).toHaveBeenCalledWith("post-message");
    });

    it("dispatches host messages to registered Blazor callbacks", () => {
        let hostCallback: BlazorCallback | null = null;
        window.__infiniframe = {
            host: {
                postData: vi.fn(),
                receiveCallback: vi.fn(callback => {
                    hostCallback = callback;
                })
            }
        };

        initWindowExternalBridge();

        const callback = vi.fn();
        const external = window.external as InfiniFrameExternal;
        external.receiveMessage!(callback);
        hostCallback!("host-message");

        expect(callback).toHaveBeenCalledWith("host-message");
    });

    it("attaches the host receive callback only once", () => {
        const receiveCallback = vi.fn();
        window.__infiniframe = {
            host: {
                postData: vi.fn(),
                receiveCallback
            }
        };

        initWindowExternalBridge();
        initWindowExternalBridge();

        expect(receiveCallback).toHaveBeenCalledTimes(1);
    });
});
