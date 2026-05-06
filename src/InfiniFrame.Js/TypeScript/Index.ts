// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import InfiniFrame from "./InfiniFrame";
import {installNativeInteropBridge} from "./Interop/NativeInterop/NativeInteropBridge";
import {getSetupGuard} from "./Interop/NativeInterop/setupGuard";
import {initWindowExternalBridge} from "./Interop/NativeInterop/blazorExternalBridge";
import {initBlazorModulesFetchPatch} from "./Interop/NativeInterop/blazorFetchPatch";
import {initBlazorCustomElementsPatch, initCustomElements} from "./Interop/NativeInterop/customElements";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export {};
console.log("InfiniFrame WebView JavaScript bridge initialized.");

const setup = getSetupGuard();

installNativeInteropBridge();

if (!setup.windowExternalBridgeInitialized) {
    setup.windowExternalBridgeInitialized = true;
    initWindowExternalBridge();
}

if (!setup.blazorModulesFetchPatchInitialized) {
    setup.blazorModulesFetchPatchInitialized = true;
    initBlazorModulesFetchPatch();
}

if (!setup.blazorCustomElementsPatchInitialized) {
    setup.blazorCustomElementsPatchInitialized = true;
    initBlazorCustomElementsPatch();
}

if (!setup.customElementsInitialized) {
    setup.customElementsInitialized = true;
    initCustomElements();
}

window.infiniframe = new InfiniFrame();
