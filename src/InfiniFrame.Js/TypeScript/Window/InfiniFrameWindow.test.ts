// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {beforeEach, describe, expect, it, vi} from "vitest";
import {InfiniFrameHostMessaging, SendToHostMessageIds} from "../Contracts";
import {InfiniFrameWindow} from "./InfiniFrameWindow";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
describe("InfiniFrameWindow", () => {
    const testWindow = window as Window;

    beforeEach(() => {
        vi.restoreAllMocks();
    });

    function createMessagingMocks(getMessageFromHostAsync: (message: string) => Promise<string>): InfiniFrameHostMessaging {
        return {
            sendMessageToHost: vi.fn(),
            getMessageFromHostAsync,
            assignMessageReceivedHandler: vi.fn(),
            unregisterMessageReceivedHandler: vi.fn()
        };
    }

    function assignInfiniFrame(messaging: InfiniFrameHostMessaging) {
        testWindow.infiniframe = {
            messaging,
            window: new InfiniFrameWindow(),
            utils: {
                setPointerCapture: vi.fn(),
                releasePointerCapture: vi.fn()
            }
        };
    }

    it("routes feature mutations through the generic feature endpoint", () => {
        const messaging = createMessagingMocks(vi.fn());
        assignInfiniFrame(messaging);

        const infiniFrameWindow = new InfiniFrameWindow();
        infiniFrameWindow.features.decorations.setTitle("Hello");

        expect(messaging.sendMessageToHost).toHaveBeenCalledWith(
            SendToHostMessageIds.windowFeatureRequest,
            {
                command: "__infiniframe:window:features:decorations:setTitle",
                args: {title: "Hello"}
            }
        );
    });

    it("getTitleAsync requests generic get envelope and returns host payload", async () => {
        const messaging = createMessagingMocks(vi.fn().mockResolvedValue('"Native Title"'));
        assignInfiniFrame(messaging);

        const infiniFrameWindow = new InfiniFrameWindow();
        const title = await infiniFrameWindow.features.decorations.getTitleAsync();

        expect(messaging.getMessageFromHostAsync).toHaveBeenCalledWith(
            "__infiniframe:window:features:decorations:title",
            undefined
        );
        expect(title).toBe("Native Title");
    });

    it("constructs every C#-mirrored window feature", () => {
        const messaging = createMessagingMocks(vi.fn());
        assignInfiniFrame(messaging);

        const featureNames = Object.keys(new InfiniFrameWindow().features);

        expect(featureNames).toEqual([
            "browser",
            "debugging",
            "decorations",
            "filePickerDialogs",
            "invoke",
            "lifecycle",
            "monitors",
            "notifications",
            "pageNavigation",
            "position",
            "size",
            "state",
            "webMessaging",
            "javaScript"
        ]);
    });
});
