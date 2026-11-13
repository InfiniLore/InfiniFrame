/**
 * Page navigation feature contract. Defines the JS API for navigating the window's web content
 * to URIs, file paths, or raw HTML strings.
 * @module PageNavigationInfiniFrameWindowFeature
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Page navigation feature API for the InfiniFrame window.
 * Provides methods to load content from various sources and query the current URL.
 */
export interface PageNavigationInfiniFrameWindowFeature {
    /**
     * Navigates the window to the specified URI.
     * @param uri - The URI to navigate to (e.g. "https://example.com").
     */
    loadUri(uri: string): void;

    /**
     * Navigates the window to a local file path.
     * @param path - The file system path to load.
     */
    loadPath(path: string): void;

    /**
     * Attempts to navigate to the specified URI, returning success status.
     * @param uri - The URI to navigate to.
     * @returns A promise resolving to true if navigation succeeded.
     */
    tryLoadUriAsync(uri: string): Promise<boolean>;

    /**
     * Attempts to navigate to a local file path, returning success status.
     * @param path - The file system path to load.
     * @returns A promise resolving to true if navigation succeeded.
     */
    tryLoadPathAsync(path: string): Promise<boolean>;

    /**
     * Loads raw HTML content directly into the window.
     * @param content - The HTML string to render.
     */
    loadRawString(content: string): void;

    /**
     * Gets the current URL of the window's web content.
     * @returns A promise resolving to the URL string, or null if unavailable.
     */
    getCurrentUrlAsync(): Promise<string | null>;

    /**
     * Gets the current URI of the window's web content.
     * @returns A promise resolving to the URI string, or null if unavailable.
     */
    getCurrentUriAsync(): Promise<string | null>;
}
