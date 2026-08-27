/**
 * Page navigation feature. Provides URL, HTML, and raw-string page loading.
 *
 * @module PageNavigationInfiniFrameWindowFeature
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {PageNavigationInfiniFrameWindowFeature as Contract} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Provides navigation capabilities including loading URIs, local paths,
 * raw HTML strings, and querying the current URL/URI.
 */
export class PageNavigationInfiniFrameWindowFeature extends InfiniFrameWindowFeature implements Contract {
    /**
     * Creates a new page navigation feature instance.
     */
    constructor() {
        super("pageNavigation");
    }

    /**
     * Navigates the window to the specified URI.
     *
     * @param uri - The URI to load (e.g. `https://example.com`).
     */
    loadUri(uri: string) {
        return this.post("loadUri", {uri});
    }

    /**
     * Navigates the window to a local file path.
     *
     * @param path - The absolute file system path to the HTML file.
     */
    loadPath(path: string) {
        return this.post("loadPath", {path});
    }

    /**
     * Attempts to navigate to the specified URI, returning whether navigation succeeded.
     *
     * @param uri - The URI to load.
     * @returns A promise that resolves to `true` if navigation was successful.
     */
    tryLoadUriAsync(uri: string) {
        return this.get<boolean>("tryLoadUri", {uri});
    }

    /**
     * Attempts to navigate to a local file path, returning whether navigation succeeded.
     *
     * @param path - The absolute file system path to the HTML file.
     * @returns A promise that resolves to `true` if navigation was successful.
     */
    tryLoadPathAsync(path: string) {
        return this.get<boolean>("tryLoadPath", {path});
    }

    /**
     * Loads raw HTML content directly into the window.
     *
     * @param content - The raw HTML string to render.
     */
    loadRawString(content: string) {
        return this.post("loadRawString", {content});
    }

    /**
     * Retrieves the current URL of the loaded page.
     *
     * @returns A promise that resolves to the URL string, or `null` if unavailable.
     */
    getCurrentUrlAsync() {
        return this.get<string | null>("getCurrentUrl");
    }

    /**
     * Retrieves the current URI of the loaded page.
     *
     * @returns A promise that resolves to the URI string, or `null` if unavailable.
     */
    getCurrentUriAsync() {
        return this.get<string | null>("getCurrentUri");
    }
}
