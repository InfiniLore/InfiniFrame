// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {beforeEach, describe, expect, it, vi} from "vitest";
import {installNativeInteropBridge} from "./NativeInteropBridge";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
describe("NativeInteropBridge", () => {
    beforeEach(() => {
        delete window.infiniframe;
        delete window.chrome;
        vi.restoreAllMocks();
    });

    it("normalizes object envelopes to string for existing postData handlers", () => {
        const existingPostData = vi.fn();
        window.infiniframe = {
            host: {
                postData: existingPostData,
                receiveCallback: vi.fn()
            },
            messaging: undefined!,
            window: undefined!,
            utils: undefined!
        };

        installNativeInteropBridge(setup);
        window.infiniframe.host!.postData({id: "ping", command: "Post", data: "hello", version: 2});

        expect(existingPostData).toHaveBeenCalledTimes(1);
        expect(existingPostData.mock.calls[0][0]).toBe("{\"id\":\"ping\",\"command\":\"Post\",\"data\":\"hello\",\"version\":2}");
    });

    it("falls back to object payload when existing postData rejects string payloads", () => {
        const existingPostData = vi.fn((payload: unknown) => {
            if (typeof payload === "string") throw new Error("String payloads not supported.");
        });
        window.infiniframe = {
            host: {
                postData: existingPostData,
                receiveCallback: vi.fn()
            },
            messaging: undefined!,
            window: undefined!,
            utils: undefined!
        };

        installNativeInteropBridge(setup);
        window.infiniframe.host!.postData({id: "ping", command: "Post", data: "hello", version: 2});

        expect(existingPostData).toHaveBeenCalledTimes(2);
        expect(typeof existingPostData.mock.calls[0][0]).toBe("string");
        expect(existingPostData.mock.calls[1][0]).toEqual({id: "ping", command: "Post", data: "hello", version: 2});
    });

    it("uses platform transport when no existing bridge callback exists", () => {
        const postData = vi.fn();
        window.chrome = {
            webview: {
                postMessage: postData,
                addEventListener: vi.fn()
            }
        };

        installNativeInteropBridge(setup);
        window.infiniframe.host!.postData({id: "ping", command: "Post", data: "hello", version: 2});

        expect(postData).toHaveBeenCalledTimes(1);
        expect(postData.mock.calls[0][0]).toBe("{\"id\":\"ping\",\"command\":\"Post\",\"data\":\"hello\",\"version\":2}");
    });
});
