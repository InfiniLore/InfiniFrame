// ---------------------------------------------------------------------------------------------------------------------
import type {BrowserInfiniFrameWindowFeature as Contract} from "../../Contracts";
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {ReceiveFromHostMessageIds} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class BrowserInfiniFrameWindowFeature extends InfiniFrameWindowFeature implements Contract {
    private contextMenuEnabled = true;
    private zoomEnabled = true;
    private browserShortcutsEnabled = true;

    constructor() {
        super("browser");

        window.infiniframe?.messaging?.assignMessageReceivedHandler(
            ReceiveFromHostMessageIds.setContextMenuEnabled, payload => {
                if (!payload) return;
                try {
                    const {enabled} = JSON.parse(payload);
                    this.contextMenuEnabled = !!enabled;
                } catch { /* ignore malformed payload */
                }
            }
        );

        window.infiniframe?.messaging?.assignMessageReceivedHandler(
            ReceiveFromHostMessageIds.setZoomEnabled, payload => {
                if (!payload) return;
                try {
                    const {enabled} = JSON.parse(payload);
                    this.zoomEnabled = !!enabled;
                } catch { /* ignore malformed payload */
                }
            }
        );

        window.infiniframe?.messaging?.assignMessageReceivedHandler(
            ReceiveFromHostMessageIds.setBrowserShortcutsEnabled, payload => {
                if (!payload) return;
                try {
                    const {enabled} = JSON.parse(payload);
                    this.browserShortcutsEnabled = !!enabled;
                } catch { /* ignore malformed payload */
                }
            }
        );

        this.installGuards();
    }

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

    private installGuards(): void {
        document.addEventListener("keydown", (e: KeyboardEvent) => {
            if (this.browserShortcutsEnabled) return;
            const ctrl = e.ctrlKey || e.metaKey;
            const k = e.key.toLowerCase();
            if (ctrl && (k === "t" || k === "n" || k === "w" || k === "r" || k === "p"
                || k === "u" || k === "j" || k === "l" || k === "i" || k === "o"
                || k === "h" || (e.shiftKey && k === "i"))) {
                e.preventDefault();
                e.stopPropagation();
                return;
            }
            if (k === "f11") {
                e.preventDefault();
                e.stopPropagation();
            }
        }, true);

        document.addEventListener("contextmenu", (e: Event) => {
            if (!this.contextMenuEnabled) {
                e.preventDefault();
                e.stopPropagation();
            }
        }, true);

        document.addEventListener("wheel", (e: WheelEvent) => {
            if (!this.zoomEnabled && (e.ctrlKey || e.metaKey)) {
                e.preventDefault();
                e.stopPropagation();
            }
        }, {capture: true, passive: false});

        document.addEventListener("keydown", (e: KeyboardEvent) => {
            if (this.zoomEnabled) return;
            const ctrl = e.ctrlKey || e.metaKey;
            const k = e.key;
            if ((ctrl && (k === "+" || k === "-" || k === "=" || k === "0")) || k === "F5") {
                e.preventDefault();
                e.stopPropagation();
            }
        }, true);
    }
}
