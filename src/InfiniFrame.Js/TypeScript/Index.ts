// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import InfiniFrame from "./InfiniFrame";
import {getSetupGuard} from "./Interop/NativeInterop/setupGuard";
import {installNativeInteropBridge} from "./Interop/NativeInterop/NativeInteropBridge";
import {initWindowExternalBridge} from "./Interop/NativeInterop/blazorExternalBridge";
import {initBlazorModulesFetchPatch} from "./Interop/NativeInterop/blazorFetchPatch";
import {initBlazorCustomElementsPatch, initCustomElements} from "./Interop/NativeInterop/customElements";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export {};

const setup = getSetupGuard();
installNativeInteropBridge(setup);
initWindowExternalBridge(setup);
initBlazorModulesFetchPatch(setup);
initBlazorCustomElementsPatch(setup);
initCustomElements(setup);

if (!window.infiniframe.messaging || !window.infiniframe.window || !window.infiniframe.utils) {
    window.infiniframe = new InfiniFrame(window.infiniframe);
}

console.log("InfiniFrame WebView JavaScript bridge initialized.");