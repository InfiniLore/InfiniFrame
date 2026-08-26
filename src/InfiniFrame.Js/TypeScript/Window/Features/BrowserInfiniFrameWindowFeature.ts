/**
 * Browser settings feature. Controls context menu, media autoplay, web security,
 * browser shortcuts, zoom, and other browser-level behaviors.
 *
 * @module BrowserInfiniFrameWindowFeature
 */

// ---------------------------------------------------------------------------------------------------------------------
import type {BrowserInfiniFrameWindowFeature as Contract} from "../../Contracts";
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {ReceiveFromHostMessageIds} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Provides access to browser-level settings such as context menu visibility,
 * media autoplay, user agent, web security, and browser shortcut blocking.
 */
export class BrowserInfiniFrameWindowFeature extends InfiniFrameWindowFeature implements Contract {
    private contextMenuEnabled = true;
    private zoomEnabled = true;
    private browserShortcutsEnabled = true;

    /**
     * Creates a new browser feature instance and installs client-side guards for
     * context menu, zoom, and browser shortcuts.
     */
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

    /**
     * Checks whether the browser context menu is enabled.
     *
     * @returns A promise that resolves to `true` if the context menu is enabled.
     */
    isContextMenuEnabledAsync() {
        return this.get<boolean>("isContextMenuEnabled");
    }

    /**
     * Checks whether media autoplay is enabled.
     *
     * @returns A promise that resolves to `true` if media autoplay is enabled.
     */
    isMediaAutoplayEnabledAsync() {
        return this.get<boolean>("isMediaAutoplayEnabled");
    }

    /**
     * Retrieves the custom user agent string.
     *
     * @returns A promise that resolves to the user agent string, or `null` if not set.
     */
    getUserAgentAsync() {
        return this.get<string | null>("userAgent");
    }

    /**
     * Checks whether the File System Access API is enabled.
     *
     * @returns A promise that resolves to `true` if file system access is enabled.
     */
    isFileSystemAccessEnabledAsync() {
        return this.get<boolean>("isFileSystemAccessEnabled");
    }

    /**
     * Checks whether web security (same-origin policy) is enforced.
     *
     * @returns A promise that resolves to `true` if web security is enabled.
     */
    isWebSecurityEnabledAsync() {
        return this.get<boolean>("isWebSecurityEnabled");
    }

    /**
     * Checks whether JavaScript clipboard access is enabled.
     *
     * @returns A promise that resolves to `true` if clipboard access is enabled.
     */
    isJavascriptClipboardAccessEnabledAsync() {
        return this.get<boolean>("isJavascriptClipboardAccessEnabled");
    }

    /**
     * Checks whether media stream (camera/microphone) access is enabled.
     *
     * @returns A promise that resolves to `true` if media stream access is enabled.
     */
    isMediaStreamEnabledAsync() {
        return this.get<boolean>("isMediaStreamEnabled");
    }

    /**
     * Checks whether certificate errors are ignored.
     *
     * @returns A promise that resolves to `true` if certificate errors are ignored.
     */
    isIgnoreCertificateErrorsEnabledAsync() {
        return this.get<boolean>("isIgnoreCertificateErrorsEnabled");
    }

    /**
     * Checks whether browser permissions are granted automatically.
     *
     * @returns A promise that resolves to `true` if permissions are granted.
     */
    getGrantBrowserPermissionsAsync() {
        return this.get<boolean>("grantBrowserPermissions");
    }

    /**
     * Checks whether smooth scrolling is enabled.
     *
     * @returns A promise that resolves to `true` if smooth scrolling is enabled.
     */
    isSmoothScrollingEnabledAsync() {
        return this.get<boolean>("isSmoothScrollingEnabled");
    }

    /**
     * Retrieves the browser control initialization parameters.
     *
     * @returns A promise that resolves to the initialization parameters string, or `null` if not set.
     */
    getBrowserControlInitParametersAsync() {
        return this.get<string | null>("browserControlInitParameters");
    }

    /**
     * Enables or disables the browser context menu.
     *
     * @param enabled - Whether to enable the context menu. Defaults to `true`.
     */
    enableContextMenu(enabled = true) {
        return this.post("enableContextMenu", {enabled});
    }

    /**
     * Enables or disables media autoplay.
     *
     * @param enabled - Whether to enable media autoplay. Defaults to `true`.
     */
    enableMediaAutoplay(enabled = true) {
        return this.post("enableMediaAutoplay", {enabled});
    }

    /**
     * Sets the custom user agent string.
     *
     * @param userAgent - The user agent string to use, or `null` to reset to default.
     */
    setUserAgent(userAgent: string | null) {
        return this.post("setUserAgent", {userAgent});
    }

    /**
     * Sets the path to the WebView2 installation (Windows only).
     *
     * @param path - The file system path to the WebView2 loader DLL or installation directory.
     */
    win32SetWebView2Path(path: string) {
        return this.post("win32SetWebView2Path", {path});
    }

    /**
     * Clears all browser auto-fill data (saved passwords, addresses, etc.).
     */
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
