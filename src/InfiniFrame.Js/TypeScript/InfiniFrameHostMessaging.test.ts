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
});
