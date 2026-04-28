// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {beforeEach, describe, expect, it, vi} from "vitest";
import {installHostBridge} from "./HostBridge";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
type TestWindow = Window & {
    __infiniframe?: {
        host?: {
            postData?: (message: unknown) => void;
            receiveCallback?: (callback: (message: string) => void) => void;
        };
    };
    chrome?: {
        webview?: {
            postMessage: (message: string) => void;
            addEventListener: (type: "message", listener: (event: { data: string }) => void) => void;
        };
    };
};

describe("HostBridge", () => {
    const testWindow = window as TestWindow;

    beforeEach(() => {
        delete testWindow.__infiniframe;
        delete testWindow.chrome;
        vi.restoreAllMocks();
    });

    it("normalizes object envelopes to string for existing postData handlers", () => {
        const existingPostData = vi.fn();
        testWindow.__infiniframe = {
            host: {
                postData: existingPostData,
                receiveCallback: vi.fn()
            }
        };

        installHostBridge();
        testWindow.__infiniframe!.host!.postData!({id: "ping", data: "hello", version: 1});

        expect(existingPostData).toHaveBeenCalledTimes(1);
        expect(existingPostData.mock.calls[0][0]).toBe("{\"id\":\"ping\",\"data\":\"hello\",\"version\":1}");
    });

    it("falls back to object payload when existing postData rejects string payloads", () => {
        const existingPostData = vi.fn((payload: unknown) => {
            if (typeof payload === "string") throw new Error("String payloads not supported.");
        });
        testWindow.__infiniframe = {
            host: {
                postData: existingPostData,
                receiveCallback: vi.fn()
            }
        };

        installHostBridge();
        testWindow.__infiniframe!.host!.postData!({id: "ping", data: "hello", version: 1});

        expect(existingPostData).toHaveBeenCalledTimes(2);
        expect(typeof existingPostData.mock.calls[0][0]).toBe("string");
        expect(existingPostData.mock.calls[1][0]).toEqual({id: "ping", data: "hello", version: 1});
    });

    it("uses platform transport when no existing bridge callback exists", () => {
        const postData = vi.fn();
        testWindow.chrome = {
            webview: {
                postMessage: postData,
                addEventListener: vi.fn()
            }
        };

        installHostBridge();
        testWindow.__infiniframe!.host!.postData!({id: "ping", data: "hello", version: 1});

        expect(postData).toHaveBeenCalledTimes(1);
        expect(postData.mock.calls[0][0]).toBe("{\"id\":\"ping\",\"data\":\"hello\",\"version\":1}");
    });
});
