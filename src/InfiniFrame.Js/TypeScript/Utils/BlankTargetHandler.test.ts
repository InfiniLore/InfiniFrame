// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {beforeEach, describe, expect, it, vi} from "vitest";
import {blankTargetHandler} from "./BlankTargetHandler";
import {IInfiniFrameHostMessaging, SendToHostMessageIds} from "../Contracts";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
type TestWindow = Window & {
    infiniframe?: {
        messaging: {
            sendMessageToHost: (id: string, data?: unknown) => void;
        };
    };
};

describe("blankTargetHandler", () => {
    const testWindow = window as TestWindow;
    const sendMessageToHost = vi.fn();

    beforeEach(() => {
        document.body.innerHTML = "";
        sendMessageToHost.mockReset();
        testWindow.infiniframe = {
            // @ts-ignore
            messaging: {
                sendMessageToHost
            }
        };
    });

    it("routes _blank links to host messaging and prevents default", async () => {
        const anchor = document.createElement("a");
        anchor.href = "https://example.com";
        anchor.setAttribute("target", "_blank");
        document.body.appendChild(anchor);

        const event = new MouseEvent("click", {bubbles: true});
        Object.defineProperty(event, "target", {value: anchor});
        const preventDefaultSpy = vi.spyOn(event, "preventDefault");

        await blankTargetHandler(event);

        expect(preventDefaultSpy).toHaveBeenCalledTimes(1);
        expect(sendMessageToHost).toHaveBeenCalledWith(SendToHostMessageIds.openExternalLink, anchor.href);
    });

    it("walks up to parent anchor when click target is nested element", async () => {
        const anchor = document.createElement("a");
        anchor.href = "https://external.example/path";
        const span = document.createElement("span");
        anchor.appendChild(span);
        document.body.appendChild(anchor);

        const event = new MouseEvent("click", {bubbles: true});
        Object.defineProperty(event, "target", {value: span});
        const preventDefaultSpy = vi.spyOn(event, "preventDefault");

        await blankTargetHandler(event);

        expect(preventDefaultSpy).toHaveBeenCalledTimes(1);
        expect(sendMessageToHost).toHaveBeenCalledWith(SendToHostMessageIds.openExternalLink, anchor.href);
    });

    it("ignores internal links without _blank or data-external", async () => {
        const anchor = document.createElement("a");
        anchor.href = "/internal-page";
        document.body.appendChild(anchor);

        const event = new MouseEvent("click", {bubbles: true});
        Object.defineProperty(event, "target", {value: anchor});
        const preventDefaultSpy = vi.spyOn(event, "preventDefault");

        await blankTargetHandler(event);

        expect(preventDefaultSpy).not.toHaveBeenCalled();
        expect(sendMessageToHost).not.toHaveBeenCalled();
    });
});
