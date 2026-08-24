// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
import {SendToHostMessageIds} from "../Contracts";
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
export abstract class InfiniFrameWindowFeature {
    protected constructor(private readonly featureName: string) {
    }

    protected post(command: string, args?: unknown): void {
        window.infiniframe.messaging.sendMessageToHost(
            SendToHostMessageIds.windowFeatureRequest,
            {command: this.command(command), args}
        );
    }

    protected async get<T>(command: string, args?: unknown): Promise<T> {
        const response = await window.infiniframe.messaging.getMessageFromHostAsync(this.command(command), args);
        return JSON.parse(response) as T;
    }

    private command(command: string): string {
        return `__infiniframe:window:features:${this.featureName}:${command}`;
    }
}
