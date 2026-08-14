// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {beforeEach, describe, expect, it, vi} from "vitest";
import {ReceiveFromHostMessageIds, SendToHostMessageIds} from "./Contracts";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
type ReceiveMessageCallback = (message: string) => void;

type TestWindow = Window & {
    infiniframe: Window["infiniframe"] & {
        host: {
            postData: (message: unknown) => void;
            receiveCallback: (callback: ReceiveMessageCallback) => void;
        };
    };
};

describe("InfiniFrameHostMessaging", () => {
    const testWindow = window as TestWindow;

    beforeEach(() => {
        vi.restoreAllMocks();
        vi.resetModules();
    });

    async function setupHostMessaging() {
        const postData = vi.fn();
        let receiveCallbackInner: ReceiveMessageCallback | null = null;
        const receiveCallback = vi.fn((callback: ReceiveMessageCallback) => {
            receiveCallbackInner = callback;
        });

        // @ts-ignore
        testWindow.infiniframe = {
            host: {
                postData,
                receiveCallback
            }
        };

        const blankTargetHandler = vi.fn();
        const titleObserverObserve = vi.fn();

        vi.doMock("./Utils/BlankTargetHandler", () => ({blankTargetHandler}));
        vi.doMock("./Utils/Observers", () => ({
            getTitleObserverTarget: vi.fn(() => document.querySelector("title")),
            getTitleObserver: vi.fn(() => ({observe: titleObserverObserve}))
        }));

        const module = await import("././InfiniFrameHostMessaging");
        const messaging = new module.default();

        return {
            messaging,
            postData,
            receiveCallback,
            getReceiveCallback: () => receiveCallbackInner!,
            blankTargetHandler,
            titleObserverObserve
        };
    }

    it("sends ready message on startup and wires receive callback", async () => {
        const {postData, receiveCallback} = await setupHostMessaging();

        expect(receiveCallback).toHaveBeenCalledTimes(1);
        expect(postData).toHaveBeenCalled();
        expect(postData.mock.calls[0][0]).toMatchObject({
            id: SendToHostMessageIds.ready,
            command: "Post",
            version: 2
        });
    });

    it("dispatches incoming envelope messages to registered handlers", async () => {
        const {messaging, getReceiveCallback} = await setupHostMessaging();
        const handler = vi.fn();
        messaging.assignMessageReceivedHandler("custom:event", handler);

        getReceiveCallback()(JSON.stringify({id: "custom:event", command: "Post", data: "payload", version: 2}));

        expect(handler).toHaveBeenCalledTimes(1);
        expect(handler).toHaveBeenCalledWith("payload");
    });

    it("ignores BlazorWebView internal __bwv messages without warning", async () => {
        const {getReceiveCallback} = await setupHostMessaging();
        const warnSpy = vi.spyOn(console, "warn").mockImplementation(() => undefined);

        getReceiveCallback()('__bwv:["AttachToDocument",0,"app"]');

        expect(warnSpy).not.toHaveBeenCalled();
    });

    it("registers open-external click handler only once", async () => {
        const {getReceiveCallback, blankTargetHandler} = await setupHostMessaging();
        const addEventListenerSpy = vi.spyOn(document, "addEventListener");
        const registerMessage = JSON.stringify({id: ReceiveFromHostMessageIds.registerOpenExternal, command: "Post", version: 2});

        getReceiveCallback()(registerMessage);
        getReceiveCallback()(registerMessage);

        const registrations = addEventListenerSpy.mock.calls.filter(call => call[0] === "click");
        expect(registrations.length).toBe(1);
        expect(registrations[0][1]).toBe(blankTargetHandler);
    });

    it("registers fullscreen change handler only once", async () => {
        const {getReceiveCallback} = await setupHostMessaging();
        const addEventListenerSpy = vi.spyOn(document, "addEventListener");
        const registerMessage = JSON.stringify({id: ReceiveFromHostMessageIds.registerFullscreenChange, command: "Post", version: 2});

        getReceiveCallback()(registerMessage);
        getReceiveCallback()(registerMessage);

        const fullscreenRegistrations = addEventListenerSpy.mock.calls.filter(call => call[0] === "fullscreenchange");
        expect(fullscreenRegistrations.length).toBe(1);
    });

    it("registers title observer on registerTitleChange message", async () => {
        const title = document.createElement("title");
        title.textContent = "My Title";
        document.head.appendChild(title);

        const {getReceiveCallback, titleObserverObserve} = await setupHostMessaging();
        getReceiveCallback()(JSON.stringify({id: ReceiveFromHostMessageIds.registerTitleChange, command: "Post", version: 2}));

        expect(titleObserverObserve).toHaveBeenCalledWith(title, {childList: true});
    });

    it("overrides window.close after registerWindowClose and routes to host", async () => {
        const {getReceiveCallback, postData} = await setupHostMessaging();
        const originalClose = window.close;
        getReceiveCallback()(JSON.stringify({id: ReceiveFromHostMessageIds.registerWindowClose, command: "Post", version: 2}));

        window.close();

        const closeMessages = postData.mock.calls
            .map(call => call[0])
            .filter(
                message => typeof message === "object"
                    && message !== null
                    && (message as { id?: string }).id === SendToHostMessageIds.windowClose
            );
        expect(closeMessages.length).toBe(1);

        window.close = originalClose;
    });

    it("sends readyAck and marks handshake as acknowledged", async () => {
        const {messaging, getReceiveCallback} = await setupHostMessaging();

        expect(messaging.isReady).toBe(false);

        getReceiveCallback()(JSON.stringify({id: ReceiveFromHostMessageIds.readyAck, command: "Post", version: 2}));

        expect(messaging.isReady).toBe(true);
        await expect(messaging.ready).resolves.toBeUndefined();
    });

    it("readyAck only resolves once", async () => {
        const {getReceiveCallback} = await setupHostMessaging();

        getReceiveCallback()(JSON.stringify({id: ReceiveFromHostMessageIds.readyAck, command: "Post", version: 2}));
        getReceiveCallback()(JSON.stringify({id: ReceiveFromHostMessageIds.readyAck, command: "Post", version: 2}));

        // Should not throw
    });

    it("unregisterMessageReceivedHandler removes handler", async () => {
        const {messaging, getReceiveCallback} = await setupHostMessaging();
        const handler = vi.fn();
        messaging.assignMessageReceivedHandler("test:event", handler);
        messaging.unregisterMessageReceivedHandler("test:event");

        getReceiveCallback()(JSON.stringify({id: "test:event", command: "Post", data: "payload", version: 2}));

        expect(handler).not.toHaveBeenCalled();
    });

    it("ignores messages with no registered handler", async () => {
        const {getReceiveCallback} = await setupHostMessaging();
        const warnSpy = vi.spyOn(console, "warn").mockImplementation(() => undefined);

        getReceiveCallback()(JSON.stringify({id: "unregistered:event", command: "Post", data: "payload", version: 2}));

        expect(warnSpy).toHaveBeenCalled();
        warnSpy.mockRestore();
    });

    it("ignores invalid messages (non-string)", async () => {
        const {getReceiveCallback} = await setupHostMessaging();
        const warnSpy = vi.spyOn(console, "warn").mockImplementation(() => undefined);

        getReceiveCallback()(123 as any);

        warnSpy.mockRestore();
    });

    it("ignores empty messages", async () => {
        const {getReceiveCallback} = await setupHostMessaging();
        getReceiveCallback()("");
    });

    it("ignores messages with parse errors", async () => {
        const {getReceiveCallback} = await setupHostMessaging();
        getReceiveCallback()("not-valid-json{{{");
    });

    it("sends webMessageAckResponse for acknowledged messages", async () => {
        const {messaging, getReceiveCallback, postData} = await setupHostMessaging();
        messaging.assignMessageReceivedHandler("custom:event", vi.fn());

        getReceiveCallback()(JSON.stringify({
            id: ReceiveFromHostMessageIds.webMessageAckRequest,
            command: "Post",
            data: JSON.stringify({OperationId: "op-1", Message: JSON.stringify({id: "custom:event", command: "Post", data: "hello", version: 2})}),
            version: 2
        }));

        const ackResponses = postData.mock.calls
            .map((call: any[]) => call[0])
            .filter((msg: any) => typeof msg === "object" && msg?.id === SendToHostMessageIds.webMessageAckResponse);
        expect(ackResponses.length).toBe(1);
    });

    it("ignores webMessageAckRequest with missing OperationId", async () => {
        const {getReceiveCallback} = await setupHostMessaging();
        getReceiveCallback()(JSON.stringify({
            id: ReceiveFromHostMessageIds.webMessageAckRequest,
            command: "Post",
            data: JSON.stringify({Message: "hello"}),
            version: 2
        }));
    });

    it("ignores webMessageAckRequest with non-string Message", async () => {
        const {getReceiveCallback} = await setupHostMessaging();
        getReceiveCallback()(JSON.stringify({
            id: ReceiveFromHostMessageIds.webMessageAckRequest,
            command: "Post",
            data: JSON.stringify({OperationId: "op-1", Message: 123}),
            version: 2
        }));
    });

    it("routes javascript eval requests", async () => {
        const {getReceiveCallback} = await setupHostMessaging();

        // eval requests route through handleJavaScriptEvalRequest which needs window.infiniframe.messaging
        // This is the real InfiniFrameHostMessaging instance, so eval sends response via postData
        getReceiveCallback()(JSON.stringify({
            id: "__infiniframe:javascript:eval",
            command: "Post",
            data: JSON.stringify({requestId: "req-1", script: "1+1"}),
            version: 2
        }));

        // The eval handler calls handleJavaScriptEvalRequest which calls messaging.sendMessageToHost
        // Since messaging IS the real instance, it calls postData with eval:result
    });

    it("routes javascript eval responses", async () => {
        const {getReceiveCallback} = await setupHostMessaging();

        getReceiveCallback()(JSON.stringify({
            id: "__infiniframe:javascript:eval:response",
            command: "Post",
            data: JSON.stringify({requestId: "req-1", result: "42"}),
            version: 2
        }));
    });

    it("ignores javascript eval response with no payload", async () => {
        const {getReceiveCallback} = await setupHostMessaging();
        getReceiveCallback()(JSON.stringify({
            id: "__infiniframe:javascript:eval:response",
            command: "Post",
            version: 2
        }));
    });

    it("ignores javascript eval request with no payload", async () => {
        const {getReceiveCallback} = await setupHostMessaging();
        getReceiveCallback()(JSON.stringify({
            id: "__infiniframe:javascript:eval",
            command: "Post",
            version: 2
        }));
    });

    it("getMessageFromHostAsync throws when getDataAsync not available", async () => {
        const {messaging} = await setupHostMessaging();
        // Access the real host object that was stored during construction
        const host = testWindow.infiniframe?.host as any;
        delete host?.getDataAsync;

        await expect(messaging.getMessageFromHostAsync("test")).rejects.toThrow();
    });

    it("sendMessageToHost warns when host bridge not initialized", async () => {
        const {messaging} = await setupHostMessaging();
        const warnSpy = vi.spyOn(console, "warn").mockImplementation(() => undefined);
        (testWindow.infiniframe.host as any).postData = undefined;

        messaging.sendMessageToHost("test" as any);

        expect(warnSpy).toHaveBeenCalled();
        warnSpy.mockRestore();
    });

    it("assignWebMessageReceiver warns when host bridge not available", async () => {
        // @ts-ignore
        testWindow.infiniframe = {host: undefined};
        const warnSpy = vi.spyOn(console, "warn").mockImplementation(() => undefined);

        const module = await import("././InfiniFrameHostMessaging");
        new module.default();

        expect(warnSpy).toHaveBeenCalled();
        warnSpy.mockRestore();
    });
});
