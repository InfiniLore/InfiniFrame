// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import InfiniFrame from "./InfiniFrame";
import {installHostBridge} from "./Interop/NativeHost/HostBridge";
import {getSetupGuard} from "./Host/setupGuard";
import {detectPlatform} from "./Host/platform";
import {attachNativeReceiver, initMessagingBridge} from "./Host/messaging";
import {initWindowExternalBridge} from "./Host/blazorExternalBridge";
import {initBlazorModulesFetchPatch} from "./Host/blazorFetchPatch";
import {initBlazorCustomElementsPatch, initCustomElements} from "./Host/customElements";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export {};
console.log('InfiniFrame WebView JavaScript bridge initialized.');

const setup = getSetupGuard();
const platform = detectPlatform();

if (!setup.messagingBridgeInitialized) {
    setup.messagingBridgeInitialized = true;
    initMessagingBridge(platform);
}

if (!setup.WebviewReceiveAttached) {
    setup.WebviewReceiveAttached = true;
    attachNativeReceiver(platform);
}

if (!setup.windowExternalBridgeInitialized) {
    setup.windowExternalBridgeInitialized = true;
    initWindowExternalBridge(platform);
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

installHostBridge();

window.infiniframe = new InfiniFrame();
