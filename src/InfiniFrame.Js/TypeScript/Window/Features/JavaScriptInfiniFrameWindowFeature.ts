// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {JavaScriptInfiniFrameWindowFeature as Contract} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
const pendingEvals = new Map<string, {resolve: (value: string | null) => void, reject: (reason: Error) => void}>();
let evalCounter = 0;

export function handleJavaScriptEvalRequest(payload: unknown) {
    if (!payload || typeof payload !== "object") return;
    const {requestId, script} = payload as {requestId?: string, script?: string};
    if (!requestId || !script) return;

    try {
        const result = eval(script);
        const resultJson = result === undefined ? null : JSON.stringify(result);
        window.infiniframe.messaging.sendMessageToHost(
            "__infiniframe:javascript:eval:result",
            {requestId, result: resultJson}
        );
    }
    catch (e) {
        const message = e instanceof Error ? e.message : String(e);
        window.infiniframe.messaging.sendMessageToHost(
            "__infiniframe:javascript:eval:result",
            {requestId, error: message}
        );
    }
}

export function handleJavaScriptEvalResponse(payload: unknown) {
    if (!payload || typeof payload !== "object") return;
    const {requestId, result, error} = payload as {requestId?: string, result?: string | null, error?: string};
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

export class JavaScriptInfiniFrameWindowFeature extends InfiniFrameWindowFeature implements Contract {
    constructor(){super("javaScript");}

    evalAsync(script: string): Promise<string | null> {
        return new Promise<string | null>((resolve, reject) => {
            const requestId = `ts_eval_${++evalCounter}`;
            pendingEvals.set(requestId, {resolve, reject});
            this.post("eval", {script, requestId});
        });
    }
}
