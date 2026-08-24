// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {Rectangle} from "./WindowFeatureTypes";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export interface StateInfiniFrameWindowFeature {
    isFullScreenAsync(): Promise<boolean>;

    isMaximizedAsync(): Promise<boolean>;

    isMinimizedAsync(): Promise<boolean>;

    isTopMostAsync(): Promise<boolean>;

    isFocusedAsync(): Promise<boolean>;

    getZoomFactorAsync(): Promise<number>;

    isZoomEnabledAsync(): Promise<boolean>;

    getCachedPreFullScreenBoundsAsync(): Promise<Rectangle>;

    getCachedPreMaximizedBoundsAsync(): Promise<Rectangle>;

    setCachedPreFullScreenBounds(bounds: Rectangle): void;

    setCachedPreMaximizedBounds(bounds: Rectangle): void;

    setMaximized(maximized?: boolean): void;

    toggleMaximized(): void;

    setMinimized(minimized?: boolean): void;

    setFullScreen(fullScreen?: boolean): void;

    setFocused(): void;

    setZoomFactor(zoom: number): void;

    enableZoom(enabled?: boolean): void;

    setTopMost(topMost?: boolean): void;
}
