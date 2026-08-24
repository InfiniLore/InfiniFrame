import {describe, expect, it, vi} from "vitest";
import {setupFeature} from "./_testHelpers";

describe("InvokeInfiniFrameWindowFeature", () => {
    it("constructs without error", async () => {
        vi.resetModules();
        setupFeature();
        vi.doMock("../InfiniFrameHostMessaging", () => ({
            default: class {
                constructor() {
                }
            }
        }));
        const mod = await import("./InvokeInfiniFrameWindowFeature");
        const feature = new mod.InvokeInfiniFrameWindowFeature();
        expect(feature).toBeDefined();
    });
});
