// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {beforeEach, describe, expect, it, vi} from "vitest";
import {installHostBridge} from "./HostBridge";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
type TestWindow = Window & {
    infiniframe?: {
        host?: {
            postMessage?: (message: unknown) => void;
            receiveMessage?: (callback: (message: string) => void) => void;
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
        delete testWindow.infiniframe;
        delete testWindow.chrome;
        vi.restoreAllMocks();
    });

    it("normalizes object envelopes to string for existing postMessage handlers", () => {
        const existingPostMessage = vi.fn();
        testWindow.infiniframe = {
            host: {
                postMessage: existingPostMessage,
                receiveMessage: vi.fn()
            }
        };

        installHostBridge();
        testWindow.infiniframe!.host!.postMessage!({id: "ping", data: "hello", version: 1});

        expect(existingPostMessage).toHaveBeenCalledTimes(1);
        expect(existingPostMessage.mock.calls[0][0]).toBe("{\"id\":\"ping\",\"data\":\"hello\",\"version\":1}");
    });

    it("falls back to object payload when existing postMessage rejects string payloads", () => {
        const existingPostMessage = vi.fn((payload: unknown) => {
            if (typeof payload === "string") throw new Error("String payloads not supported.");
        });
        testWindow.infiniframe = {
            host: {
                postMessage: existingPostMessage,
                receiveMessage: vi.fn()
            }
        };

        installHostBridge();
        testWindow.infiniframe!.host!.postMessage!({id: "ping", data: "hello", version: 1});

        expect(existingPostMessage).toHaveBeenCalledTimes(2);
        expect(typeof existingPostMessage.mock.calls[0][0]).toBe("string");
        expect(existingPostMessage.mock.calls[1][0]).toEqual({id: "ping", data: "hello", version: 1});
    });

    it("uses platform transport when no existing bridge callback exists", () => {
        const postMessage = vi.fn();
        testWindow.chrome = {
            webview: {
                postMessage,
                addEventListener: vi.fn()
            }
        };

        installHostBridge();
        testWindow.infiniframe!.host!.postMessage!({id: "ping", data: "hello", version: 1});

        expect(postMessage).toHaveBeenCalledTimes(1);
        expect(postMessage.mock.calls[0][0]).toBe("{\"id\":\"ping\",\"data\":\"hello\",\"version\":1}");
    });
});
