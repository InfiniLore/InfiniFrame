/**
 * JavaScript evaluation feature contract. Defines the JS API for executing arbitrary
 * JavaScript code in the host's JavaScript engine.
 * @module JavaScriptInfiniFrameWindowFeature
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * JavaScript evaluation feature API for the InfiniFrame window.
 * Provides methods to execute scripts in the host's JavaScript runtime.
 */
export interface JavaScriptInfiniFrameWindowFeature {
    /**
     * Evaluates a JavaScript expression in the host's JavaScript engine.
     * @param script - The JavaScript source code to execute.
     * @returns A promise resolving to the stringified result, or null if the expression has no value.
     */
    evalAsync(script: string): Promise<string | null>
}
