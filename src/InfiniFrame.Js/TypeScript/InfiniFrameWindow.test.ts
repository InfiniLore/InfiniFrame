// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {beforeEach, describe, expect, it, vi} from "vitest";
import {IInfiniFrameHostMessaging, SendToHostMessageIds} from "./Contracts";
import {InfiniFrameWindow} from "./InfiniFrameWindow";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
describe("InfiniFrameWindow", () => {
    const testWindow = window as Window;

    beforeEach(() => {
        vi.restoreAllMocks();
    });

    function createMessagingMocks(getMessageFromHostAsync: (message: string) => Promise<string>): IInfiniFrameHostMessaging {
        return {
            sendMessageToHost: vi.fn(),
            getMessageFromHostAsync,
            assignMessageReceivedHandler: vi.fn(),
            unregisterMessageReceivedHandler: vi.fn()
        };
    }

    function assignInfiniFrame(messaging: IInfiniFrameHostMessaging) {
        testWindow.infiniframe = {
            messaging,
            window: {
                setTitle: vi.fn(),
                getTitleAsync: vi.fn(async () => ""),
            },
            utils: {
                setPointerCapture: vi.fn(),
                releasePointerCapture: vi.fn()
            }
        };
    }

    it("setTitle sends titleChange message", () => {
        const messaging = createMessagingMocks(vi.fn());
        assignInfiniFrame(messaging);

        const infiniFrameWindow = new InfiniFrameWindow();
        infiniFrameWindow.setTitle("Hello");

        expect(messaging.sendMessageToHost).toHaveBeenCalledWith(SendToHostMessageIds.titleChange, "Hello");
    });

    it("getTitleAsync requests generic get envelope and returns host payload", async () => {
        const messaging = createMessagingMocks(vi.fn().mockResolvedValue("Native Title"));
        assignInfiniFrame(messaging);

        const infiniFrameWindow = new InfiniFrameWindow();
        const title = await infiniFrameWindow.getTitleAsync();

        expect(messaging.getMessageFromHostAsync).toHaveBeenCalledWith(
            expect.objectContaining({
                id: SendToHostMessageIds.getRequest,
                command: "Get",
                data: {command: "title", args: undefined},
                version: 2
            })
        );
        expect(title).toBe("Native Title");
    });
});
