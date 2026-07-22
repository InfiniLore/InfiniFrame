// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {Rectangle} from "./WindowFeatureTypes";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export interface InfiniFrameWindowFeatureState {
    isFullScreenAsync(): Promise<boolean>; isMaximizedAsync(): Promise<boolean>; isMinimizedAsync(): Promise<boolean>;
    isTopMostAsync(): Promise<boolean>; isFocusedAsync(): Promise<boolean>;
    getZoomFactorAsync(): Promise<number>; isZoomEnabledAsync(): Promise<boolean>;
    getCachedPreFullScreenBoundsAsync(): Promise<Rectangle>; getCachedPreMaximizedBoundsAsync(): Promise<Rectangle>;
    setMaximized(maximized?: boolean): void; toggleMaximized(): void; setMinimized(minimized?: boolean): void;
    setFullScreen(fullScreen?: boolean): void; setFocused(): void; setZoomFactor(zoom: number): void;
    enableZoom(enabled?: boolean): void; setTopMost(topMost?: boolean): void;
}
