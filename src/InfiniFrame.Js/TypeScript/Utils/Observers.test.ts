// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {afterEach, describe, expect, it, vi} from "vitest";
import {getTitleObserver, getTitleObserverTarget} from "./Observers";
import {SendToHostMessageIds} from "../Contracts/IInfiniFrameHostMessaging";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
type TestWindow = Window & {
    infiniFrame?: {
        hostMessaging: {
            sendMessageToHost: (id: string, data?: unknown) => void;
        };
    };
};

describe("Observers", () => {
    const testWindow = window as TestWindow;
    const sendMessageToHost = vi.fn();
    const OriginalMutationObserver = globalThis.MutationObserver;

    afterEach(() => {
        sendMessageToHost.mockReset();
        globalThis.MutationObserver = OriginalMutationObserver;
        document.head.innerHTML = "";
    });

    it("getTitleObserverTarget returns the title element when present", () => {
        const title = document.createElement("title");
        title.textContent = "My Title";
        document.head.appendChild(title);

        expect(getTitleObserverTarget()).toBe(title);
    });

    it("getTitleObserver sends titleChange only for childList mutations", () => {
        let callback: MutationCallback = () => undefined;

        class FakeMutationObserver {
            public observe = vi.fn();
            public disconnect = vi.fn();
            public takeRecords = vi.fn(() => []);

            constructor(cb: MutationCallback) {
                callback = cb;
            }
        }

        globalThis.MutationObserver = FakeMutationObserver as unknown as typeof MutationObserver;
        testWindow.infiniFrame = {
            hostMessaging: {
                sendMessageToHost
            }
        };

        const title = document.createElement("title");
        title.textContent = "Initial";
        document.head.appendChild(title);

        getTitleObserver();
        callback(
            [
                {type: "attributes"} as MutationRecord,
                {type: "childList"} as MutationRecord
            ],
            {} as MutationObserver
        );

        expect(sendMessageToHost).toHaveBeenCalledTimes(1);
        expect(sendMessageToHost).toHaveBeenCalledWith(SendToHostMessageIds.titleChange, "Initial");
    });
});
