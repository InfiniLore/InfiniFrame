export type Platform = 'webview' | 'webkit' | null;

export function detectPlatform(): Platform {
    if (window.chrome && (window.chrome as any).webview) return 'webview';
    if (
        window.webkit &&
        window.webkit.messageHandlers &&
        window.webkit.messageHandlers['infiniFrameInterop']
    ) return 'webkit';
    return null;
}

export function nativePost(platform: Platform, message: string): void {
    if (platform === 'webview') {
        (window.chrome as any).webview.postMessage(message);
    } else if (platform === 'webkit') {
        window.webkit!.messageHandlers['infiniFrameInterop'].postMessage(message);
    } else {
        console.warn('[InfiniFrame] No native bridge available:', message);
    }
}
