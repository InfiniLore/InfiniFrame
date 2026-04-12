import {defineConfig} from "vitest/config";

export default defineConfig({
    test: {
        environment: "jsdom",
        include: ["TypeScript/**/*.test.ts"],
        clearMocks: true,
        restoreMocks: true,
        coverage: {
            provider: "v8",
            reporter: ["text", "lcov"]
        }
    }
});
