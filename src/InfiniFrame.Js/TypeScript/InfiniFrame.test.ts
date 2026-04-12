// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {beforeEach, describe, expect, it, vi} from "vitest";
import {SendToHostMessageIds} from "./Contracts/IHostMessaging";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
describe("InfiniFrame", () => {
    beforeEach(() => {
        vi.resetModules();
    });

    it("delegates sendMessageToHost to HostMessaging", async () => {
        const sendMessageToHost = vi.fn();

        vi.doMock("./HostMessaging", () => ({
            default: class {
                public sendMessageToHost = sendMessageToHost;
            }
        }));

        const {InfiniFrame} = await import("./InfiniFrame");
        const instance = new InfiniFrame();
        instance.sendMessageToHost(SendToHostMessageIds.ready, "payload");

        expect(sendMessageToHost).toHaveBeenCalledTimes(1);
        expect(sendMessageToHost).toHaveBeenCalledWith(SendToHostMessageIds.ready, "payload");
    });

    it("forwards pointer capture APIs to element", async () => {
        const {InfiniFrame} = await import("./InfiniFrame");
        const instance = new InfiniFrame();

        const setPointerCapture = vi.fn();
        const releasePointerCapture = vi.fn();
        const element = {
            setPointerCapture,
            releasePointerCapture
        } as unknown as Element;

        instance.setPointerCapture(element, 10);
        instance.releasePointerCapture(element, 10);

        expect(setPointerCapture).toHaveBeenCalledWith(10);
        expect(releasePointerCapture).toHaveBeenCalledWith(10);
    });
});
