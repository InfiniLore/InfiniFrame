// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {beforeEach, describe, expect, it, vi} from "vitest";
import type {InfiniFrameSetup} from "../../Contracts";
import {installNativeInteropBridge, resetNativeInteropBridgeState} from "./NativeInteropBridge";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
describe("NativeInteropBridge", () => {
    let setup: InfiniFrameSetup;

    beforeEach(() => {
        setup = createSetup();
        delete (window as any).infiniframe;
        delete (window as any).chrome;
        delete (window as any).webkit;
        resetNativeInteropBridgeState();
        vi.restoreAllMocks();
    });

    describe("initialization guard", () => {
        it("does nothing if already initialized", () => {
            setup.nativeInteropBridgeInitialized = true;
            installNativeInteropBridge(setup);
            expect((window as any).infiniframe).toBeUndefined();
        });

        it("sets nativeInteropBridgeInitialized to true", () => {
            installNativeInteropBridge(setup);
            expect(setup.nativeInteropBridgeInitialized).toBe(true);
        });

        it("creates window.infiniframe if missing", () => {
            installNativeInteropBridge(setup);
            expect(window.infiniframe).toBeDefined();
        });

        it("preserves existing window.infiniframe properties", () => {
            (window as any).infiniframe = {existing: true};
            installNativeInteropBridge(setup);
            expect((window as any).infiniframe.existing).toBe(true);
        });
    });

    describe("postData - string payload", () => {
        it("dispatches string payload via existing postData", () => {
            const existingPostData = vi.fn();
            window.infiniframe = {host: {postData: existingPostData, receiveCallback: vi.fn()}} as any;

            installNativeInteropBridge(setup);
            window.infiniframe.host!.postData("hello world");

            expect(existingPostData).toHaveBeenCalledWith("hello world");
        });

        it("ignores empty string payload", () => {
            const existingPostData = vi.fn();
            const warnSpy = vi.spyOn(console, "warn").mockImplementation(() => {});
            window.infiniframe = {host: {postData: existingPostData, receiveCallback: vi.fn()}} as any;

            installNativeInteropBridge(setup);
            window.infiniframe.host!.postData("   ");

            expect(existingPostData).not.toHaveBeenCalled();
            expect(warnSpy).toHaveBeenCalledWith("Ignoring empty host bridge payload.");
            warnSpy.mockRestore();
        });

        it("falls back to chrome.webview.postMessage when no existing bridge", () => {
            const postData = vi.fn();
            window.chrome = {webview: {postMessage: postData, addEventListener: vi.fn()}} as any;

            installNativeInteropBridge(setup);
            window.infiniframe.host!.postData("test message");

            expect(postData).toHaveBeenCalledWith("test message");
        });

        it("falls back to webKit when no chrome.webview", () => {
            const postData = vi.fn();
            window.webkit = {messageHandlers: {infiniFrameInterop: {postMessage: postData}}} as any;

            installNativeInteropBridge(setup);
            window.infiniframe.host!.postData("test message");

            expect(postData).toHaveBeenCalledWith("test message");
        });

        it("warns when no platform transport available", () => {
            const warnSpy = vi.spyOn(console, "warn").mockImplementation(() => {});
            window.infiniframe = {host: {receiveCallback: vi.fn()}} as any;

            installNativeInteropBridge(setup);
            window.infiniframe.host!.postData("test");

            expect(warnSpy).toHaveBeenCalled();
            warnSpy.mockRestore();
        });

        it("falls back to platform when existing postData throws on string", () => {
            const existingPostData = vi.fn((payload: unknown) => {
                if (typeof payload === "string") throw new Error("No strings");
            });
            const chromePost = vi.fn();
            window.chrome = {webview: {postMessage: chromePost, addEventListener: vi.fn()}} as any;
            window.infiniframe = {host: {postData: existingPostData, receiveCallback: vi.fn()}} as any;

            installNativeInteropBridge(setup);
            window.infiniframe.host!.postData("hello");

            expect(chromePost).toHaveBeenCalledWith("hello");
        });
    });

    describe("postData - envelope payload", () => {
        it("normalizes object envelopes to string for existing postData handlers", () => {
            const existingPostData = vi.fn();
            window.infiniframe = {host: {postData: existingPostData, receiveCallback: vi.fn()}} as any;

            installNativeInteropBridge(setup);
            window.infiniframe.host!.postData({id: "ping", command: "Post", data: "hello", version: 2});

            expect(existingPostData).toHaveBeenCalledTimes(1);
            expect(existingPostData.mock.calls[0][0]).toBe("{\"id\":\"ping\",\"command\":\"Post\",\"data\":\"hello\",\"version\":2}");
        });

        it("falls back to object payload when existing postData rejects string", () => {
            const existingPostData = vi.fn((payload: unknown) => {
                if (typeof payload === "string") throw new Error("String payloads not supported.");
            });
            window.infiniframe = {host: {postData: existingPostData, receiveCallback: vi.fn()}} as any;

            installNativeInteropBridge(setup);
            window.infiniframe.host!.postData({id: "ping", command: "Post", data: "hello", version: 2});

            expect(existingPostData).toHaveBeenCalledTimes(2);
            expect(typeof existingPostData.mock.calls[0][0]).toBe("string");
            expect(existingPostData.mock.calls[1][0]).toEqual({id: "ping", command: "Post", data: "hello", version: 2});
        });

        it("uses platform transport when no existing bridge callback", () => {
            const postData = vi.fn();
            window.chrome = {webview: {postMessage: postData, addEventListener: vi.fn()}} as any;

            installNativeInteropBridge(setup);
            window.infiniframe.host!.postData({id: "ping", command: "Post", data: "hello", version: 2});

            expect(postData).toHaveBeenCalledTimes(1);
        });

        it("ignores envelope with empty id", () => {
            const existingPostData = vi.fn();
            const warnSpy = vi.spyOn(console, "warn").mockImplementation(() => {});
            window.infiniframe = {host: {postData: existingPostData, receiveCallback: vi.fn()}} as any;

            installNativeInteropBridge(setup);
            window.infiniframe.host!.postData({id: "", command: "Post"} as any);

            expect(existingPostData).not.toHaveBeenCalled();
            warnSpy.mockRestore();
        });

        it("ignores null/non-object envelope", () => {
            const existingPostData = vi.fn();
            const warnSpy = vi.spyOn(console, "warn").mockImplementation(() => {});
            window.infiniframe = {host: {postData: existingPostData, receiveCallback: vi.fn()}} as any;

            installNativeInteropBridge(setup);
            window.infiniframe.host!.postData(null as any);

            expect(existingPostData).not.toHaveBeenCalled();
            expect(warnSpy).toHaveBeenCalled();
            warnSpy.mockRestore();
        });

        it("preserves channel field in normalized envelope", () => {
            const existingPostData = vi.fn();
            window.infiniframe = {host: {postData: existingPostData, receiveCallback: vi.fn()}} as any;

            installNativeInteropBridge(setup);
            window.infiniframe.host!.postData({id: "test", command: "Post", channel: "myChannel", version: 2});

            const parsed = JSON.parse(existingPostData.mock.calls[0][0]);
            expect(parsed.channel).toBe("myChannel");
        });

        it("ignores empty channel string", () => {
            const existingPostData = vi.fn();
            window.infiniframe = {host: {postData: existingPostData, receiveCallback: vi.fn()}} as any;

            installNativeInteropBridge(setup);
            window.infiniframe.host!.postData({id: "test", command: "Post", channel: "  ", version: 2});

            const parsed = JSON.parse(existingPostData.mock.calls[0][0]);
            expect(parsed.channel).toBeUndefined();
        });
    });

    describe("receiveCallback", () => {
        it("registers existing receive callback", () => {
            const existingReceive = vi.fn();
            window.infiniframe = {host: {postData: vi.fn(), receiveCallback: existingReceive}} as any;

            installNativeInteropBridge(setup);
            const cb = vi.fn();
            window.infiniframe.host!.receiveCallback(cb);

            expect(existingReceive).toHaveBeenCalled();
        });
    });

    describe("getDataAsync", () => {
        it("returns promise rejection for invalid payload", async () => {
            window.infiniframe = {host: {postData: vi.fn(), receiveCallback: vi.fn()}} as any;

            installNativeInteropBridge(setup);
            await expect(window.infiniframe.host!.getDataAsync("")).rejects.toThrow("invalid");
        });

        it("returns promise rejection for empty string", async () => {
            window.infiniframe = {host: {postData: vi.fn(), receiveCallback: vi.fn()}} as any;

            installNativeInteropBridge(setup);
            await expect(window.infiniframe.host!.getDataAsync("  ")).rejects.toThrow("invalid");
        });

        it("delegates to existing getDataAsync when available (sync result)", async () => {
            const existingGetData = vi.fn(() => "sync-result");
            window.infiniframe = {host: {postData: vi.fn(), receiveCallback: vi.fn(), getDataAsync: existingGetData}} as any;

            installNativeInteropBridge(setup);
            const result = await window.infiniframe.host!.getDataAsync("test-message");

            expect(result).toBe("sync-result");
        });

        it("delegates to existing getDataAsync when available (promise result)", async () => {
            const existingGetData = vi.fn(() => Promise.resolve("async-result"));
            window.infiniframe = {host: {postData: vi.fn(), receiveCallback: vi.fn(), getDataAsync: existingGetData}} as any;

            installNativeInteropBridge(setup);
            const result = await window.infiniframe.host!.getDataAsync("test-message");

            expect(result).toBe("async-result");
        });

        it("falls back when existing getDataAsync throws", async () => {
            const existingGetData = vi.fn(() => { throw new Error("bridge failed"); });
            const chromePost = vi.fn();
            window.chrome = {webview: {postMessage: chromePost, addEventListener: vi.fn()}} as any;
            window.infiniframe = {host: {receiveCallback: vi.fn(), getDataAsync: existingGetData}} as any;

            installNativeInteropBridge(setup);
            vi.useFakeTimers();
            const promise = window.infiniframe.host!.getDataAsync("test");
            vi.advanceTimersByTime(11000);

            await expect(promise).rejects.toThrow();
            vi.useRealTimers();
        });

        it("sends get request envelope via postData", async () => {
            const postData = vi.fn();
            window.infiniframe = {host: {postData, receiveCallback: vi.fn()}} as any;

            installNativeInteropBridge(setup);
            vi.useFakeTimers();
            const promise = window.infiniframe.host!.getDataAsync({id: "test-envelope", version: 2});
            vi.advanceTimersByTime(11000);

            expect(postData).toHaveBeenCalled();
            const envelope = JSON.parse(postData.mock.calls[0][0]);
            expect(envelope.command).toBe("Get");
            expect(envelope.requestId).toBeDefined();

            await expect(promise).rejects.toThrow();
            vi.useRealTimers();
        });

        it("parses JSON string as envelope for get request", async () => {
            const postData = vi.fn();
            window.infiniframe = {host: {postData, receiveCallback: vi.fn()}} as any;

            installNativeInteropBridge(setup);
            vi.useFakeTimers();
            const promise = window.infiniframe.host!.getDataAsync('{"id":"test","version":2}');
            vi.advanceTimersByTime(11000);

            const envelope = JSON.parse(postData.mock.calls[0][0]);
            expect(envelope.command).toBe("Get");
            expect(envelope.id).toBe("test");

            await expect(promise).rejects.toThrow();
            vi.useRealTimers();
        });

        it("treats plain string as message id for get request", async () => {
            const postData = vi.fn();
            window.infiniframe = {host: {postData, receiveCallback: vi.fn()}} as any;

            installNativeInteropBridge(setup);
            vi.useFakeTimers();
            const promise = window.infiniframe.host!.getDataAsync("my-message-id");
            vi.advanceTimersByTime(11000);

            const envelope = JSON.parse(postData.mock.calls[0][0]);
            expect(envelope.id).toBe("my-message-id");
            expect(envelope.command).toBe("Get");

            await expect(promise).rejects.toThrow();
            vi.useRealTimers();
        });

        it("times out when no response received", async () => {
            window.infiniframe = {host: {postData: vi.fn(), receiveCallback: vi.fn()}} as any;

            installNativeInteropBridge(setup);
            vi.useFakeTimers();
            const promise = window.infiniframe.host!.getDataAsync("test");
            vi.advanceTimersByTime(11000);

            await expect(promise).rejects.toThrow("Timed out");
            vi.useRealTimers();
        });

        it("resolves when response matches requestId", async () => {
            let receiveCallbackFn: ((msg: string) => void) | null = null;
            const receiveCallback = vi.fn((cb: (msg: string) => void) => { receiveCallbackFn = cb; });
            const postData = vi.fn();
            window.infiniframe = {host: {postData, receiveCallback}} as any;

            installNativeInteropBridge(setup);

            const promise = window.infiniframe.host!.getDataAsync("test");

            const envelope = JSON.parse(postData.mock.calls[0][0]);
            const requestId = envelope.requestId;

            const response = JSON.stringify({
                id: "__infiniframe:get:response",
                command: "Post",
                data: JSON.stringify({requestId, success: true, data: "result-data"}),
                version: 2
            });
            receiveCallbackFn!(response);

            const result = await promise;
            expect(result).toBe("result-data");
        });

        it("rejects when response indicates failure", async () => {
            let receiveCallbackFn: ((msg: string) => void) | null = null;
            const receiveCallback = vi.fn((cb: (msg: string) => void) => { receiveCallbackFn = cb; });
            const postData = vi.fn();
            window.infiniframe = {host: {postData, receiveCallback}} as any;

            installNativeInteropBridge(setup);

            const promise = window.infiniframe.host!.getDataAsync("test");

            const envelope = JSON.parse(postData.mock.calls[0][0]);
            const requestId = envelope.requestId;

            const response = JSON.stringify({
                id: "__infiniframe:get:response",
                command: "Post",
                data: JSON.stringify({requestId, success: false, error: "host error"}),
                version: 2
            });
            receiveCallbackFn!(response);

            await expect(promise).rejects.toThrow("host error");
        });

        it("ignores response with wrong requestId", async () => {
            let receiveCallbackFn: ((msg: string) => void) | null = null;
            const receiveCallback = vi.fn((cb: (msg: string) => void) => { receiveCallbackFn = cb; });
            const postData = vi.fn();
            window.infiniframe = {host: {postData, receiveCallback}} as any;

            installNativeInteropBridge(setup);

            vi.useFakeTimers();
            const promise = window.infiniframe.host!.getDataAsync("test");

            const response = JSON.stringify({
                id: "__infiniframe:get:response",
                command: "Post",
                data: JSON.stringify({requestId: "wrong-id", success: true, data: "data"}),
                version: 2
            });
            receiveCallbackFn!(response);

            vi.advanceTimersByTime(11000);
            await expect(promise).rejects.toThrow("Timed out");
            vi.useRealTimers();
        });

        it("ignores response with invalid JSON payload", async () => {
            let receiveCallbackFn: ((msg: string) => void) | null = null;
            const receiveCallback = vi.fn((cb: (msg: string) => void) => { receiveCallbackFn = cb; });
            const postData = vi.fn();
            window.infiniframe = {host: {postData, receiveCallback}} as any;

            installNativeInteropBridge(setup);

            vi.useFakeTimers();
            const promise = window.infiniframe.host!.getDataAsync("test");

            const response = JSON.stringify({
                id: "__infiniframe:get:response",
                command: "Post",
                data: "not-valid-json{{{",
                version: 2
            });
            receiveCallbackFn!(response);

            vi.advanceTimersByTime(11000);
            await expect(promise).rejects.toThrow("Timed out");
            vi.useRealTimers();
        });

        it("ignores response with missing data field", async () => {
            let receiveCallbackFn: ((msg: string) => void) | null = null;
            const receiveCallback = vi.fn((cb: (msg: string) => void) => { receiveCallbackFn = cb; });
            const postData = vi.fn();
            window.infiniframe = {host: {postData, receiveCallback}} as any;

            installNativeInteropBridge(setup);

            const promise = window.infiniframe.host!.getDataAsync("test");

            const envelope = JSON.parse(postData.mock.calls[0][0]);
            const requestId = envelope.requestId;

            const response = JSON.stringify({
                id: "__infiniframe:get:response",
                command: "Post",
                data: JSON.stringify({requestId, success: true}),
                version: 2
            });
            receiveCallbackFn!(response);

            const result = await promise;
            expect(result).toBe("");
        });

        it("rejects when payload has wrong shape", async () => {
            let receiveCallbackFn: ((msg: string) => void) | null = null;
            const receiveCallback = vi.fn((cb: (msg: string) => void) => { receiveCallbackFn = cb; });
            const postData = vi.fn();
            window.infiniframe = {host: {postData, receiveCallback}} as any;

            installNativeInteropBridge(setup);

            vi.useFakeTimers();
            const promise = window.infiniframe.host!.getDataAsync("test");

            const response = JSON.stringify({
                id: "__infiniframe:get:response",
                command: "Post",
                data: JSON.stringify({notRequestId: true}),
                version: 2
            });
            receiveCallbackFn!(response);

            vi.advanceTimersByTime(11000);
            await expect(promise).rejects.toThrow("Timed out");
            vi.useRealTimers();
        });
    });
});

function createSetup(): InfiniFrameSetup {
    return {
        nativeInteropBridgeInitialized: false,
        windowExternalBridgeInitialized: false,
        blazorModulesFetchPatchInitialized: false,
        blazorCustomElementsPatchInitialized: false,
        customElementsInitialized: false
    };
}
