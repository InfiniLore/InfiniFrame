// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {StateInfiniFrameWindowFeature as Contract,Rectangle} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class StateInfiniFrameWindowFeature extends InfiniFrameWindowFeature implements Contract {
    constructor(){super("state");}

    isFullScreenAsync() {
        return this.get<boolean>("isFullScreen");
    }

    isMaximizedAsync() {
        return this.get<boolean>("isMaximized");
    }

    isMinimizedAsync() {
        return this.get<boolean>("isMinimized");
    }

    isTopMostAsync() {
        return this.get<boolean>("isTopMost");
    }

    isFocusedAsync() {
        return this.get<boolean>("isFocused");
    }

    getZoomFactorAsync() {
        return this.get<number>("zoomFactor");
    }

    isZoomEnabledAsync() {
        return this.get<boolean>("isZoomEnabled");
    }

    getCachedPreFullScreenBoundsAsync() {
        return this.get<Rectangle>("cachedPreFullScreenBounds");
    }

    getCachedPreMaximizedBoundsAsync() {
        return this.get<Rectangle>("cachedPreMaximizedBounds");
    }

    setMaximized(maximized = true) {
        return this.post("setMaximized", {maximized});
    }

    toggleMaximized() {
        return this.post("toggleMaximized");
    }

    setMinimized(minimized = true) {
        return this.post("setMinimized", {minimized});
    }

    setFullScreen(fullScreen = true) {
        return this.post("setFullScreen", {fullScreen});
    }

    setFocused() {
        return this.post("setFocused");
    }

    setZoomFactor(zoom: number) {
        return this.post("setZoomFactor", {zoom});
    }

    enableZoom(enabled = true) {
        return this.post("enableZoom", {enabled});
    }

    setTopMost(topMost = true) {
        return this.post("setTopMost", {topMost});
    }
}
