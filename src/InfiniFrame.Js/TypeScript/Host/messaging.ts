import { type Platform, nativePost } from './platform.js';
import { type RawEnvelope, normalizeEnvelope, createGetEnvelope } from './envelope.js';

type MessageCallback = (data: string) => void;

export interface InfiniFrameHost {
    postData(envelope: RawEnvelope | string): void;
    receiveCallback(cb: MessageCallback): void;
    getDataAsync(message: string | RawEnvelope): Promise<string>;
}

export interface InfiniFrameBridge {
    onReceiveMessageCallbacks: MessageCallback[];
    host: InfiniFrameHost;
}

declare global {
    interface Window {
        __infiniframe: InfiniFrameBridge;
        __dispatchMessageCallback?: (data: string) => void;
    }
}

function generateRequestId(): string {
    return 'if_req_' + Date.now().toString(36) + '_' + Math.random().toString(36).slice(2);
}

export function initMessagingBridge(platform: Platform): void {
    window.__infiniframe = {
        onReceiveMessageCallbacks: [],

        host: {
            postData(envelope: RawEnvelope | string): void {
                const normalized =
                    typeof envelope === 'string'
                        ? envelope
                        : normalizeEnvelope(envelope, 'Post');
                if (!normalized) return;
                const message =
                    typeof normalized === 'string' ? normalized : JSON.stringify(normalized);
                nativePost(platform, message);
            },

            receiveCallback(cb: MessageCallback): void {
                window.__infiniframe.onReceiveMessageCallbacks.push(cb);
            },

            getDataAsync(message: string | RawEnvelope): Promise<string> {
                const requestId = generateRequestId();
                const getEnvelope = createGetEnvelope(message, requestId);

                if (!getEnvelope) {
                    return Promise.reject(new Error('Host getDataAsync payload is invalid.'));
                }

                return new Promise<string>((resolve, reject) => {
                    const callback: MessageCallback = (raw: string) => {
                        try {
                            const env = JSON.parse(raw);
                            if (!env || env.id !== '__infiniframe:get:response') return;

                            const payload = JSON.parse(env.data || '{}');
                            if (!payload || payload.requestId !== requestId) return;

                            const idx =
                                window.__infiniframe.onReceiveMessageCallbacks.indexOf(callback);
                            if (idx >= 0) {
                                window.__infiniframe.onReceiveMessageCallbacks.splice(idx, 1);
                            }

                            payload.success
                                ? resolve(payload.data || '')
                                : reject(new Error(payload.error || 'getDataAsync failed'));
                        } catch {
                            // ignore parse errors
                        }
                    };

                    window.__infiniframe.host.receiveCallback(callback);
                    window.__infiniframe.host.postData(getEnvelope);
                });
            },
        },
    };
}

export function attachNativeReceiver(platform: Platform): void {
    function dispatch(data: string): void {
        for (const cb of window.__infiniframe.onReceiveMessageCallbacks) {
            try {
                cb(data);
            } catch {
                // ignore callback errors
            }
        }
    }

    if (platform === 'webview') {
        (window.chrome as any).webview.addEventListener('message', (e: MessageEvent) =>
            dispatch(e.data)
        );
    } else if (platform === 'webkit') {
        window.__dispatchMessageCallback = dispatch;
    }
}
