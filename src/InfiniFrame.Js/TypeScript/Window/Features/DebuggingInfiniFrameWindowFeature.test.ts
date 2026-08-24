import {beforeEach, describe, expect, it, vi} from "vitest";
import {setupFeature} from "./_testHelpers";

describe("DebuggingInfiniFrameWindowFeature", () => {
    let feature: any;
    let messaging: ReturnType<typeof import("./_testHelpers").createMessagingMock>;

    beforeEach(async () => {
        vi.resetModules();
        messaging = setupFeature();
        vi.doMock("../InfiniFrameHostMessaging", () => ({
            default: class {
                constructor() {
                }
            }
        }));
        const mod = await import("./DebuggingInfiniFrameWindowFeature");
        feature = new mod.DebuggingInfiniFrameWindowFeature();
        (window as any).infiniframe.messaging = messaging;
    });

    it("isDevToolsEnabledAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(true));
        await feature.isDevToolsEnabledAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("supportsWebInspectorAttachAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(true));
        await feature.supportsWebInspectorAttachAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("isWebInspectorEnabledAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(true));
        await feature.isWebInspectorEnabledAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("supportsRemoteDebuggingEndpointAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(true));
        await feature.supportsRemoteDebuggingEndpointAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("getRemoteDebuggingPortAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(9222));
        await feature.getRemoteDebuggingPortAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("getCapabilitiesAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify({supportsLocalDevTools: true}));
        await feature.getCapabilitiesAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("getDiagnosticsAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify({platform: "test"}));
        await feature.getDiagnosticsAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("tryGetRemoteDebuggingEndpointAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify({success: true}));
        await feature.tryGetRemoteDebuggingEndpointAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("tryProbeEndpointAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify({reachable: true}));
        await feature.tryProbeEndpointAsync("http://localhost:9222");
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("enableDevTools posts command", () => {
        feature.enableDevTools(false);
        expect(messaging.sendMessageToHost).toHaveBeenCalled();
    });
});
