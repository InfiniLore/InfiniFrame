/**
 * Abstract base class for all window feature implementations.
 *
 * @module InfiniFrameWindowFeature
 */

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
import {SendToHostMessageIds} from "../Contracts";
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Base class for window features that communicate with the C# host via the envelope protocol.
 *
 * Subclasses call {@link post} for fire-and-forget commands or {@link get} for request/response
 * queries that return deserialized JSON payloads.
 */
export abstract class InfiniFrameWindowFeature {
    /**
     * @param featureName - The identifier used to build the fully-qualified command namespace
     * (e.g. `"browser"`, `"size"`, `"decorations"`).
     */
    protected constructor(private readonly featureName: string) {
    }

    /**
     * Sends a fire-and-forget command to the C# host.
     *
     * @param command - The command name (e.g. `"setTitle"`, `"setHeight"`).
     * @param args - Optional arguments serialized as part of the envelope payload.
     */
    protected post(command: string, args?: unknown): void {
        window.infiniframe.messaging.sendMessageToHost(
            SendToHostMessageIds.windowFeatureRequest,
            {command: this.command(command), args}
        );
    }

    /**
     * Sends a request to the C# host and awaits the JSON response.
     *
     * @typeParam T - The expected return type after JSON deserialization.
     * @param command - The command name (e.g. `"getSize"`, `"isFullScreen"`).
     * @param args - Optional arguments serialized as part of the envelope payload.
     * @returns A promise that resolves with the deserialized response of type `T`.
     */
    protected async get<T>(command: string, args?: unknown): Promise<T> {
        const response = await window.infiniframe.messaging.getMessageFromHostAsync(this.command(command), args);
        return JSON.parse(response) as T;
    }

    /**
     * Builds the fully-qualified command string by combining the feature name and command.
     *
     * @param command - The short command name.
     * @returns The fully-qualified command identifier.
     */
    private command(command: string): string {
        return `__infiniframe:window:features:${this.featureName}:${command}`;
    }
}
