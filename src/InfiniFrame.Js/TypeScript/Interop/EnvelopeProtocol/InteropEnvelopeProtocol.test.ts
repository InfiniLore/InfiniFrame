// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
// @ts-ignore
import fs from "node:fs";
// @ts-ignore
import path from "node:path";
import {describe, expect, it} from "vitest";
import {createEnvelopeMessage, InteropMessageMaxSizeBytes, parseIncomingMessage} from "./InteropEnvelopeProtocol";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
type CreateVector = {
    name: string;
    id: string;
    data: string | null;
    expectedMessage: string;
};

type ParseVector = {
    name: string;
    message: string;
    success: boolean;
    messageId?: string;
    payload?: string | null;
    isLegacyProtocol?: boolean;
    errorContains?: string;
};

type GoldenVectors = {
    createVectors: CreateVector[];
    parseVectors: ParseVector[];
};

function loadGoldenVectors(): GoldenVectors {
    // @ts-ignore
    const filePath = path.resolve(__dirname, "./interop-envelope-golden-vectors.json");
    return JSON.parse(fs.readFileSync(filePath, "utf8")) as GoldenVectors;
}

describe("InteropEnvelopeProtocol", () => {
    const vectors = loadGoldenVectors();

    it("createEnvelopeMessage follows golden vectors", () => {
        for (const vector of vectors.createVectors) {
            const actual = createEnvelopeMessage(vector.id, vector.data);
            expect(actual, vector.name).toBe(vector.expectedMessage);
        }
    });

    it("parseIncomingMessage follows golden vectors", () => {
        for (const vector of vectors.parseVectors) {
            const result = parseIncomingMessage(vector.message);
            if ("error" in result) {
                expect(vector.success, vector.name).toBe(false);
                expect(result.error.includes(vector.errorContains ?? ""), vector.name).toBe(true);
                continue;
            }

            expect(vector.success, vector.name).toBe(true);
            expect(result.messageId, vector.name).toBe(vector.messageId);
            expect(result.payload ?? null, vector.name).toBe(vector.payload ?? null);
            expect(Boolean(result.isLegacyProtocol), vector.name).toBe(Boolean(vector.isLegacyProtocol));
        }
    });

    it("rejects oversize messages", () => {
        const message = "a".repeat(InteropMessageMaxSizeBytes + 1);
        const result = parseIncomingMessage(message);
        expect("error" in result).toBe(true);
        if ("error" in result) {
            expect(result.error).toContain("exceeds max size");
        }
    });
});
