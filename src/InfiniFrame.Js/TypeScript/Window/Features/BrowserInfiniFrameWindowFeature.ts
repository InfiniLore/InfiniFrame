// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {BrowserInfiniFrameWindowFeature as Contract} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class BrowserInfiniFrameWindowFeature extends InfiniFrameWindowFeature implements Contract {
    constructor() { super("browser"); }

    isContextMenuEnabledAsync() {
        return this.get<boolean>("isContextMenuEnabled");
    }

    isMediaAutoplayEnabledAsync() {
        return this.get<boolean>("isMediaAutoplayEnabled");
    }

    getUserAgentAsync() {
        return this.get<string | null>("userAgent");
    }

    isFileSystemAccessEnabledAsync() {
        return this.get<boolean>("isFileSystemAccessEnabled");
    }

    isWebSecurityEnabledAsync() {
        return this.get<boolean>("isWebSecurityEnabled");
    }

    isJavascriptClipboardAccessEnabledAsync() {
        return this.get<boolean>("isJavascriptClipboardAccessEnabled");
    }

    isMediaStreamEnabledAsync() {
        return this.get<boolean>("isMediaStreamEnabled");
    }

    isIgnoreCertificateErrorsEnabledAsync() {
        return this.get<boolean>("isIgnoreCertificateErrorsEnabled");
    }

    getGrantBrowserPermissionsAsync() {
        return this.get<boolean>("grantBrowserPermissions");
    }

    isSmoothScrollingEnabledAsync() {
        return this.get<boolean>("isSmoothScrollingEnabled");
    }

    getBrowserControlInitParametersAsync() {
        return this.get<string | null>("browserControlInitParameters");
    }

    enableContextMenu(enabled = true) {
        return this.post("enableContextMenu", {enabled});
    }

    enableMediaAutoplay(enabled = true) {
        return this.post("enableMediaAutoplay", {enabled});
    }

    setUserAgent(userAgent: string | null) {
        return this.post("setUserAgent", {userAgent});
    }

    win32SetWebView2Path(path: string) {
        return this.post("win32SetWebView2Path", {path});
    }

    clearBrowserAutoFill() {
        return this.post("clearBrowserAutoFill");
    }
}
