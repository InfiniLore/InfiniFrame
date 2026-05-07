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
installNativeInteropBridge(setup);
initWindowExternalBridge(setup);
initBlazorModulesFetchPatch(setup);
initBlazorCustomElementsPatch(setup);
initCustomElements(setup);

if (!window.infiniframe.messaging || !window.infiniframe.window || !window.infiniframe.utils) {
    window.infiniframe = new InfiniFrame(window.infiniframe);
}
