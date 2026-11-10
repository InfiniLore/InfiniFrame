import { beforeEach, describe, expect, it } from "vitest";
import type { JavaScriptInfiniFrameWindowFeature } from "./JavaScriptInfiniFrameWindowFeature";
import { setupFeature } from "./_testHelpers";

function getRequestId(data: unknown): string {
	if (typeof data !== "object" || data === null) {
		throw new Error("Expected an eval request object.");
	}
	const args = "args" in data ? data.args : data;
	if (typeof args !== "object" || args === null || !("requestId" in args)) {
		throw new Error("Expected an eval request ID.");
	}
	return String(args.requestId);
}

describe("JavaScriptInfiniFrameWindowFeature", () => {
	let feature: JavaScriptInfiniFrameWindowFeature;
	let messaging: ReturnType<
		typeof import("./_testHelpers").createMessagingMock
	>;

	beforeEach(async () => {
		messaging = setupFeature();
		const mod = await import("./JavaScriptInfiniFrameWindowFeature");
		feature = new mod.JavaScriptInfiniFrameWindowFeature();
	});

	it("evalAsync sends eval command and resolves on response", async () => {
		const { handleJavaScriptEvalResponse } = await import(
			"./JavaScriptInfiniFrameWindowFeature"
		);
		messaging.sendMessageToHost.mockImplementation(
			(_id: string, data: unknown) => {
				const requestId = getRequestId(data);
				handleJavaScriptEvalResponse({
					requestId,
					result: JSON.stringify("42"),
				});
			},
		);
		const result = await feature.evalAsync("1 + 1");
		expect(result).toBe("42");
	});

	it("evalAsync rejects on error response", async () => {
		const { handleJavaScriptEvalResponse } = await import(
			"./JavaScriptInfiniFrameWindowFeature"
		);
		messaging.sendMessageToHost.mockImplementation(
			(_id: string, data: unknown) => {
				const requestId = getRequestId(data);
				handleJavaScriptEvalResponse({ requestId, error: "Syntax error" });
			},
		);
		await expect(
			feature.evalAsync("throw new Error('Syntax error')"),
		).rejects.toThrow("Syntax error");
	});

	it("handleJavaScriptEvalRequest executes script and sends result", async () => {
		const { handleJavaScriptEvalRequest } = await import(
			"./JavaScriptInfiniFrameWindowFeature"
		);
		handleJavaScriptEvalRequest({ requestId: "req-1", script: "1 + 2" });
		expect(messaging.sendMessageToHost).toHaveBeenCalledWith(
			"__infiniframe:javascript:eval:result",
			expect.objectContaining({ requestId: "req-1", result: "3" }),
		);
	});

	it("handleJavaScriptEvalRequest sends error on exception", async () => {
		const { handleJavaScriptEvalRequest } = await import(
			"./JavaScriptInfiniFrameWindowFeature"
		);
		handleJavaScriptEvalRequest({
			requestId: "req-2",
			script: "throw new Error('fail')",
		});
		expect(messaging.sendMessageToHost).toHaveBeenCalledWith(
			"__infiniframe:javascript:eval:result",
			expect.objectContaining({
				requestId: "req-2",
				error: expect.any(String),
			}),
		);
	});

	it("handleJavaScriptEvalRequest ignores invalid payload", async () => {
		const { handleJavaScriptEvalRequest } = await import(
			"./JavaScriptInfiniFrameWindowFeature"
		);
		handleJavaScriptEvalRequest(null);
		handleJavaScriptEvalRequest({});
		handleJavaScriptEvalRequest({ requestId: "x" });
		handleJavaScriptEvalRequest({ script: "y" });
	});

	it("handleJavaScriptEvalResponse ignores invalid payload", async () => {
		const { handleJavaScriptEvalResponse } = await import(
			"./JavaScriptInfiniFrameWindowFeature"
		);
		handleJavaScriptEvalResponse(null);
		handleJavaScriptEvalResponse({});
		handleJavaScriptEvalResponse({ requestId: "nonexistent" });
	});

	it("handleJavaScriptEvalResponse resolves with null for null result", async () => {
		const { handleJavaScriptEvalResponse } = await import(
			"./JavaScriptInfiniFrameWindowFeature"
		);
		messaging.sendMessageToHost.mockImplementation(
			(_id: string, data: unknown) => {
				const requestId = getRequestId(data);
				handleJavaScriptEvalResponse({ requestId, result: null });
			},
		);
		const result = await feature.evalAsync("undefined");
		expect(result).toBeNull();
	});
});
