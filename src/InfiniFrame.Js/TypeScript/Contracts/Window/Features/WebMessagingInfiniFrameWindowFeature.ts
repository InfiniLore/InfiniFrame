/**
 * Web messaging feature contract. Defines the JS API for sending web messages to the native host.
 * @module WebMessagingInfiniFrameWindowFeature
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Web messaging feature API for the InfiniFrame window.
 * Provides methods to send messages to the native host via the web message channel.
 */
export interface WebMessagingInfiniFrameWindowFeature {
    /**
     * Sends a web message to the native host.
     * @param message - The message string to send.
     */
    sendWebMessage(message: string): void
}
