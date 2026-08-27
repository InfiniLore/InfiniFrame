/**
 * JavaScript execution feature. Allows the C# host to request evaluation of
 * arbitrary JavaScript expressions in the browser context and receive the result.
 *
 * @module JavaScriptInfiniFrameWindowFeature
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {JavaScriptInfiniFrameWindowFeature as Contract} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
const pendingEvals = new Map<string, { resolve: (value: string | null) => void, reject: (reason: Error) => void }>();
let evalCounter = 0;

/**
 * Handles an incoming JavaScript eval request from the C# host. Executes the script
 * using `new Function` and sends the result back via the messaging bridge.
 *
 * @param payload - The eval request payload containing `requestId` and `script`.
 */
export function handleJavaScriptEvalRequest(payload: unknown) {
    if (!payload || typeof payload !== "object") return;
    const {requestId, script} = payload as { requestId?: string, script?: string };
    if (!requestId || !script) return;

    try {
        const result = new Function(`return (${script})`)();
        const resultJson = result === undefined ? null : JSON.stringify(result);
        window.infiniframe.messaging.sendMessageToHost(
            "__infiniframe:javascript:eval:result",
            {requestId, result: resultJson}
        );
    } catch (e) {
        const message = e instanceof Error ? e.message : String(e);
        window.infiniframe.messaging.sendMessageToHost(
            "__infiniframe:javascript:eval:result",
            {requestId, error: message}
        );
    }
}

/**
 * Handles the response to a previously issued JavaScript eval request. Resolves or
 * rejects the pending promise associated with the request ID.
 *
 * @param payload - The eval response payload containing `requestId`, `result`, and optionally `error`.
 */
export function handleJavaScriptEvalResponse(payload: unknown) {
    if (!payload || typeof payload !== "object") return;
    const {requestId, result, error} = payload as { requestId?: string, result?: string | null, error?: string };
    if (!requestId) return;
    const pending = pendingEvals.get(requestId);
    if (!pending) return;
    pendingEvals.delete(requestId);
    if (error) pending.reject(new Error(error));
    else {
        const parsed = result !== null && result !== undefined ? JSON.parse(result) : null;
        pending.resolve(parsed as string | null);
    }
}

/**
 * Provides JavaScript execution capabilities in the browser context, enabling the
 * C# host to evaluate arbitrary scripts and retrieve results.
 */
export class JavaScriptInfiniFrameWindowFeature extends InfiniFrameWindowFeature implements Contract {
    /**
     * Creates a new JavaScript feature instance.
     */
    constructor() {
        super("javaScript");
    }

    /**
     * Evaluates a JavaScript expression in the browser context and returns the result.
     *
     * @param script - The JavaScript expression to evaluate.
     * @returns A promise that resolves to the JSON-serialized result string, or `null`
     * if the expression returns `undefined`.
     */
    evalAsync(script: string): Promise<string | null> {
        return new Promise<string | null>((resolve, reject) => {
            const requestId = `ts_eval_${++evalCounter}`;
            pendingEvals.set(requestId, {resolve, reject});
            this.post("eval", {script, requestId});
        });
    }
}
