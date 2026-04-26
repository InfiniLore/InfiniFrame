// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {beforeEach, describe, expect, it, vi} from "vitest";
import {ReceiveFromHostMessageIds, SendToHostMessageIds} from "./Contracts/IInfiniFrameHostMessaging";
import InfiniFrameHostMessaging from "./InfiniFrameHostMessaging";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
type ReceiveMessageCallback = (message: string) => void;

type TestWindow = Window & {
    infiniframe?: {
        host?: {
            postMessage: (message: unknown) => void;
            receiveMessage: (callback: ReceiveMessageCallback) => void;
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
        const postMessage = vi.fn();
        let receiveCallback: ReceiveMessageCallback | null = null;
        const receiveMessage = vi.fn((callback: ReceiveMessageCallback) => {
            receiveCallback = callback;
        });

        testWindow.infiniframe = {
            host: {
                postMessage,
                receiveMessage
            }
        };

        const blankTargetHandler = vi.fn();
        const titleObserverObserve = vi.fn();

        vi.doMock("./BlankTargetHandler", () => ({blankTargetHandler}));
        vi.doMock("./Observers", () => ({
            getTitleObserverTarget: vi.fn(() => document.querySelector("title")),
            getTitleObserver: vi.fn(() => ({observe: titleObserverObserve}))
        }));

        const module = await import("././InfiniFrameHostMessaging");
        const messaging = new module.default();

        return {
            messaging,
            postMessage,
            receiveMessage,
            getReceiveCallback: () => receiveCallback!,
            blankTargetHandler,
            titleObserverObserve
        };
    }

    it("sends ready message on startup and wires receive callback", async () => {
        const {postMessage, receiveMessage} = await setupHostMessaging();

        expect(receiveMessage).toHaveBeenCalledTimes(1);
        expect(postMessage).toHaveBeenCalled();
        expect(postMessage.mock.calls[0][0]).toMatchObject({
            id: SendToHostMessageIds.ready,
            version: 1
        });
    });

    it("dispatches incoming envelope messages to registered handlers", async () => {
        const {messaging, getReceiveCallback} = await setupHostMessaging();
        const handler = vi.fn();
        messaging.assignMessageReceivedHandler("custom:event", handler);

        getReceiveCallback()(JSON.stringify({id: "custom:event", data: "payload", version: 1}));

        expect(handler).toHaveBeenCalledTimes(1);
        expect(handler).toHaveBeenCalledWith("payload");
    });

    it("logs legacy warning only once for repeated legacy messages", async () => {
        const {messaging, getReceiveCallback} = await setupHostMessaging();
        const warnSpy = vi.spyOn(console, "warn").mockImplementation(() => undefined);
        messaging.assignMessageReceivedHandler("legacy", vi.fn());

        getReceiveCallback()("legacy;a");
        getReceiveCallback()("legacy;b");

        const legacyWarnings = warnSpy.mock.calls.filter(call =>
            String(call[0]).includes("legacy inbound host message format")
        );
        expect(legacyWarnings.length).toBe(1);
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
        const registerMessage = JSON.stringify({id: ReceiveFromHostMessageIds.registerOpenExternal, version: 1});

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
        getReceiveCallback()(JSON.stringify({id: ReceiveFromHostMessageIds.registerTitleChange, version: 1}));

        expect(titleObserverObserve).toHaveBeenCalledWith(title, {childList: true});
    });

    it("overrides window.close after registerWindowClose and routes to host", async () => {
        const {getReceiveCallback, postMessage} = await setupHostMessaging();
        const originalClose = window.close;
        getReceiveCallback()(JSON.stringify({id: ReceiveFromHostMessageIds.registerWindowClose, version: 1}));

        window.close();

        const closeMessages = postMessage.mock.calls
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
