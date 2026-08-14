import {defineConfig} from "vitest/config";

export default defineConfig({
    test: {
        environment: "jsdom",
        include: ["TypeScript/**/*.test.ts"],
        clearMocks: true,
        restoreMocks: true,
        coverage: {
            provider: "v8",
            reporter: ["text", "lcov"],
            include: ["TypeScript/**/*.ts"],
            exclude: [
                "TypeScript/Contracts/**",
                "TypeScript/Window/Features/index.ts",
                "TypeScript/Utils/index.ts"
            ],
            thresholds: {
                lines: 85,
                branches: 65,
                functions: 90,
                statements: 84
            }
        }
    }
});
